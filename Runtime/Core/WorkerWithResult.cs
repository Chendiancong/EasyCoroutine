using System;
using System.Collections.Generic;
using UnityEngine;

namespace EasyCoroutine
{
    public class Worker<Result> : WorkerBase, IAwaitable<Result>, IWorkerLike<Result>, IThenable<Result>
    {
        public delegate void SimpleExecutor(Action<Result> resolver);
        public delegate void Executor(Action<Result> resolver, Action<Exception> rejecter);

        public WorkerCallback<Result> Callback { get; private set; }
        private Result m_result;
        private WorkerExecution m_execution = new WorkerExecution();
        private List<IInvokable<Result>> m_nextJobs = new List<IInvokable<Result>>();
        private List<IInvokable<Exception>> m_rejectJobs = new List<IInvokable<Exception>>();

        public Worker()
        {
            Callback = new WorkerCallback<Result>();
        }

        public Worker(SimpleExecutor executor)
        {
            Callback = new WorkerCallback<Result>();
            m_execution.executor1 = executor;
        }

        public Worker(Executor executor)
        {
            Callback = new WorkerCallback<Result>();
            m_execution.executor2 = executor;
        }

        public WorkerAwaiter GetAwaiter()
        {
            return new WorkerAwaiter(this);
        }

        ICustomAwaiter<Result> IAwaitable<Result>.GetAwaiter() => GetAwaiter();


        public void AddNextJob(IInvokable<Result> invokable)
            => m_nextJobs.Add(invokable);
        
        public void AddRejectJob(IInvokable<Exception> invokable)
            => m_rejectJobs.Add(invokable);

#region IWorkerLike implementations
        void IWorkerLike<Result>.Resolve(Result result) => InternalResolve(result);

        void IWorkerLike<Result>.Reject(Exception e) => InternalReject(e);

        void IWorkerLike<Result>.Reject(string reason) => InternalReject(new Exception(reason));
#endregion

#region IThenable implementations
        public IThenable Then(Action<Result> onFullfilled)
        {
            throw new NotImplementedException();
        }

        public IThenable<NextResult> Then<NextResult>(Func<Result, NextResult> onFullfilled)
        {
            throw new NotImplementedException();
        }

        public IThenable Then(Action onFullfilled)
        {
            throw new NotImplementedException();
        }

        public IThenable<NextResult> Then<NextResult>(Func<NextResult> onFullfilled)
        {
            throw new NotImplementedException();
        }

        public IThenable Catch(Action<Exception> onReject)
        {
            throw new NotImplementedException();
        }

        public IThenable<NextResult> Catch<NextResult>(Func<Exception, NextResult> onReject)
        {
            throw new NotImplementedException();
        }
#endregion

        protected void ResetWorker()
        {
            if (Status == WorkerStatus.Running)
                InternalResolve(default);
            Reset();
        }

        protected void InternalResolve(Result result)
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

        public struct WorkerAwaiter : ICustomAwaiter<Result>
        {
            private Worker<Result> mWorker;

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

            public WorkerAwaiter(Worker<Result> worker)
            {
                mWorker = worker;
            }

            public Result GetResult()
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

    public class PooledWorker<Result> : Worker<Result>, IPoolable
    {
        protected bool isPoolObj = false;

        #region IPoolable implementation
        void IPoolable.OnCreate() => OnPoolCreate();

        void IPoolable.OnReuse() => OnPoolReuse();

        void IPoolable.OnRestore() => OnPoolRestore();
        #endregion

        protected virtual void OnPoolCreate()
        {
            isPoolObj = true;
        }

        protected virtual void OnPoolReuse() {}

        protected virtual void OnPoolRestore()
        {
            if (Status == WorkerStatus.Running)
                InternalResolve(default);
            Reset();
        }

        protected static void DisposeMe<Instance>(Instance ins)
            where Instance : PooledWorker<Instance>, new()
        {
            if (ins.isPoolObj)
                FactoryMgr.Restore(ins);
        }

        protected static void ResolveMe<Instance>(Instance ins, Result result)
            where Instance : PooledWorker<Result>, new()
        {
            ins.InternalResolve(result);
            if (ins.isPoolObj)
                FactoryMgr.Restore(ins);
        }

        protected static void RejectMe<Instance>(Instance ins, Exception exception)
            where Instance : PooledWorker<Result>, new()
        {
            ins.InternalReject(exception);
            if (ins.isPoolObj)
                FactoryMgr.Restore(ins);
        }

        protected static void RejectMe<Instance>(Instance ins, string reason)
            where Instance : PooledWorker<Result>, new()
            => RejectMe(ins, new Exception(reason));
    }
}