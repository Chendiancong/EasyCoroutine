using System;
using System.Collections.Generic;

namespace EasyCoroutine
{
    public class Worker : WorkerBase, IAwaitable, IWorkerLike
    {
        public delegate void SimpleExecutor(Action resolver);
        public delegate void Executor(Action resolver, Action<Exception> rejector);

        public WorkerCallback Callback { get; private set; }
        private WorkerExecution m_execution = new WorkerExecution();
        private List<IInvokable> m_nextJobs = new List<IInvokable>();
        private List<IInvokable<Exception>> m_rejectJobs = new List<IInvokable<Exception>>();

        public Worker()
        {
            Callback = new WorkerCallback();
        }

        public Worker(SimpleExecutor executor) : this()
        {
            m_execution.executor1 = executor;
        }

        public Worker(Executor executor) : this()
        {
            m_execution.executor2 = executor;
        }

        public WorkerAwaiter GetAwaiter()
        {
            return new WorkerAwaiter(this);
        }

        ICustomAwaiter IAwaitable.GetAwaiter() => GetAwaiter();

        public void Resolve() => InternalResolve();

        public void Reject(Exception e) => InternalReject(e);

        public void Reject(string reason)
            =>InternalReject(new WorkerException(reason));

        public void AddNextJob(IInvokable invokable)
            => m_nextJobs.Add(invokable);

        public void AddRejectJob(IInvokable<Exception> invokable)
            => m_rejectJobs.Add(invokable);

        public override void Reset()
        {
            if (Status == WorkerStatus.Running)
                InternalResolve();
            base.Reset();
        }

        protected void InternalResolve()
        {
            try
            {
                WorkerStatus status = Status;
                if (status == WorkerStatus.Succeed || status == WorkerStatus.Failed)
                    return;
                Status = WorkerStatus.Succeed;
                Callback.OnFullfilled();
                InternalContinue();
                for (int i = 0, len = m_nextJobs.Count; i < len; ++i)
                    m_nextJobs[i].Invoke();
            }
            catch { throw; }
            finally
            {
                if (continuations != null)
                    continuations = null;
                m_nextJobs.Clear();
            }
        }

        protected void InternalReject(string reason) => InternalReject(new WorkerException(reason));

        protected void InternalReject(Exception e)
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

        private void InternalContinue() {
            if (continuations != null)
                continuations();
        }

        public struct WorkerAwaiter : ICustomAwaiter
        {
            private Worker mWorker;

            public bool IsCompleted
            {
                get
                {
                    mWorker.Start();

                    ref WorkerExecution wAction = ref mWorker.m_execution;
                    if (wAction.executor1 != null)
                        wAction.executor1(mWorker.InternalResolve);
                    else if (wAction.executor2 != null)
                        wAction.executor2(mWorker.InternalResolve, mWorker.InternalReject);
                    wAction.Clear();

                    WorkerStatus status = mWorker.Status;
                    return status == WorkerStatus.Succeed ||
                        status == WorkerStatus.Failed;
                }
            }

            public WorkerAwaiter(Worker worker)
            {
                mWorker = worker;
            }

            public void OnCompleted(Action continuation)
            {
                mWorker.continuations += continuation;
            }

            public void GetResult()
            {

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
