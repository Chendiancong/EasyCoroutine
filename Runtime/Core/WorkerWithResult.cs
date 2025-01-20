using System;
using System.Collections.Generic;

namespace EasyCoroutine
{
    public class Worker<TResult> : WorkerBase, IAwaitable<TResult>, IWorkerLike<TResult>
    {
        public delegate void SimpleExecutor(Action<TResult> resolver);
        public delegate void Executor(Action<TResult> resolver, Action<Exception> rejecter);

        public WorkerCallback<TResult> Callback { get; private set; }
        private TResult m_result;
        private WorkerExecution m_execution = new WorkerExecution();
        private List<IInvokable<TResult>> m_nextJobs = new List<IInvokable<TResult>>();
        private List<IInvokable<Exception>> m_rejectJobs = new List<IInvokable<Exception>>();

        public Worker()
        {
            Callback = new WorkerCallback<TResult>();
        }

        public Worker(SimpleExecutor executor)
        {
            Callback = new WorkerCallback<TResult>();
            m_execution.executor1 = executor;
        }

        public Worker(Executor executor)
        {
            Callback = new WorkerCallback<TResult>();
            m_execution.executor2 = executor;
        }

        public WorkerAwaiter GetAwaiter()
        {
            return new WorkerAwaiter(this);
        }

        ICustomAwaiter<TResult> IAwaitable<TResult>.GetAwaiter() => GetAwaiter();

        public void Resolve(TResult result) => InternalResolve(result);

        public void Reject(Exception e) => InternalReject(e);

        public void Reject(string reason) => InternalReject(new Exception(reason));

        public void AddNextJob(IInvokable<TResult> invokable)
            => m_nextJobs.Add(invokable);
        
        public void AddRejectJob(IInvokable<Exception> invokable)
            => m_rejectJobs.Add(invokable);

        protected void ResetWorker()
        {
            if (Status == WorkerStatus.Running)
                InternalResolve(default);
            Reset();
        }

        private void InternalResolve(TResult result)
        {
            m_result = result;
            try
            {
                WorkerStatus status = Status;
                if (status == WorkerStatus.Succeed || status == WorkerStatus.Failed)
                    return;
                Status = WorkerStatus.Succeed;
                Callback.OnFullfiled(result);
                if (continuations != null)
                    continuations();
                for (int i = 0, len = m_nextJobs.Count; i < len; ++i)
                    m_nextJobs[i].Invoke(result);
            }
            catch { throw; }
            finally
            {
                if (continuations != null)
                    continuations = null;
                m_nextJobs.Clear();
            }
        }

        private void InternalReject(Exception e)
        {
            try
            {
                WorkerStatus status = Status;
                if (status == WorkerStatus.Succeed || status == WorkerStatus.Failed)
                    return;
                Status = WorkerStatus.Failed;
                Callback.OnException(WorkerException.FromException(e));
                for (int i = 0, len = m_rejectJobs.Count; i < len; ++i)
                    m_rejectJobs[i].Invoke(e);
            }
            catch { throw; }
            finally
            {
                if (continuations != null)
                    continuations = null;
                m_rejectJobs.Clear();
            }
        }

        public struct WorkerAwaiter : ICustomAwaiter<TResult>
        {
            private Worker<TResult> mWorker;

            public bool IsCompleted
            {
                get
                {
                    mWorker.Start();

                    ref WorkerExecution wAction = ref mWorker.m_execution;
                    if (wAction.executor1 != null)
                        wAction.executor1(mWorker.Resolve);
                    else if (wAction.executor2 != null)
                        wAction.executor2(mWorker.Resolve, mWorker.Reject);
                    wAction.Clear();

                    WorkerStatus status = mWorker.Status;
                    return status == WorkerStatus.Succeed ||
                        status == WorkerStatus.Failed;
                }
            }

            public WorkerAwaiter(Worker<TResult> worker)
            {
                mWorker = worker;
            }

            public TResult GetResult()
            {
                return mWorker.m_result;
            }

            public void OnCompleted(Action continuation)
            {
                mWorker.continuations += continuation;
            }
        }

        private struct WorkerExecution
        {
            public SimpleExecutor executor1;
            public Executor executor2;

            public void Clear()
            {
                executor1 = null;
                executor2 = null;
            }
        }
    }
}