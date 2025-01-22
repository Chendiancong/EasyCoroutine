using System;
using System.Collections.Generic;

namespace EasyCoroutine
{
    public class Worker<Result> : WorkerBase, IAwaitable<Result>, IWorkerLike<Result>, IThenable<Result>
    {
        public delegate void SimpleExecutor(Action<Result> resolver);
        public delegate void Executor(Action<Result> resolver, Action<Exception> rejecter);
        public delegate void FullExecutor(Action<Result> resolver, Action<IWorkerLike<Result>> chainResolver, Action<Exception> rejecter);

        public WorkerCallback<Result> Callback { get; private set; }
        private Result m_result;
        private List<IInvokable<Result>> m_fullfilled = new List<IInvokable<Result>>();
        private List<IInvokable<Exception>> m_rejected = new List<IInvokable<Exception>>();

        public Worker()
        {
            Callback = new WorkerCallback<Result>();
        }

        public Worker(SimpleExecutor executor)
        {
            Callback = new WorkerCallback<Result>();
            executor(InternalResolve);
        }

        public Worker(Executor executor)
        {
            Callback = new WorkerCallback<Result>();
            executor(InternalResolve, InternalReject);
        }

        public Worker(FullExecutor executor)
        {
            Callback = new WorkerCallback<Result>();
            executor(InternalResolve, InternalResolve, InternalReject);
        }

        public WorkerAwaiter GetAwaiter()
        {
            return new WorkerAwaiter(this);
        }

        ICustomAwaiter<Result> IAwaitable<Result>.GetAwaiter() => GetAwaiter();

        #region IWorkerLike implementations
        void IWorkerLike<Result>.Resolve(IWorkerLike<Result> result) => InternalResolve(result);
        
        void IWorkerLike<Result>.Resolve(Result result) => InternalResolve(result);

        void IWorkerLike<Result>.Reject(Exception e) => InternalReject(e);

        void IWorkerLike<Result>.Reject(string reason) => InternalReject(new Exception(reason));

        void IWorkerLike<Result>.OnFullfilled(Action<Result> onFullfilled)
        {
            WorkerNextWithInput<Result> next = new WorkerNextWithInput<Result>(null, onFullfilled);
            m_fullfilled.Add(next);
        }

        void IWorkerLike<Result>.OnRejected(Action<Exception> onRejected)
        {
            WorkerRejecter rejecter = new WorkerRejecter(null, onRejected);
            m_rejected.Add(rejecter);
        }
        #endregion

        #region IThenable implementations
        public IThenable Then(Action<Result> onFullfilled)
        {
            var defer = new WorkerDefer();
            var next = new WorkerNextWithInput<Result>(defer, onFullfilled);
            m_fullfilled.Add(next);
            return defer.Worker;
        }

        public IThenable<NextResult> Then<NextResult>(Func<Result, NextResult> onFullfilled)
        {
            var defer = new WorkerDefer<NextResult>();
            var next = new WorkerNextWithInputOutput<Result, NextResult>(defer, onFullfilled);
            m_fullfilled.Add(next);
            return defer.Worker;
        }

        public IThenable Then(Action onFullfilled)
        {
            var defer = new WorkerDefer();
            var next = new WorkerNextWithInput<Result>(defer, _ => onFullfilled());
            m_fullfilled.Add(next);
            return defer.Worker;
        }

        public IThenable<NextResult> Then<NextResult>(Func<NextResult> onFullfilled)
        {
            var defer = new WorkerDefer<NextResult>();
            var next = new WorkerNextWithInputOutput<Result, NextResult>(defer, _ => onFullfilled());
            m_fullfilled.Add(next);
            return defer.Worker;
        }

        public IThenable Catch(Action<Exception> onReject)
        {
            var defer = new WorkerDefer();
            var reject = new WorkerRejecter(defer, onReject);
            m_rejected.Add(reject);
            return defer.Worker;
        }

        public IThenable<NextResult> Catch<NextResult>(Func<Exception, NextResult> onReject)
        {
            var defer = new WorkerDefer<NextResult>();
            var reject = new WorkerRejecter<NextResult>(defer, onReject);
            m_rejected.Add(reject);
            return defer.Worker;
        }
        #endregion

        protected void ResetWorker()
        {
            if (Status == WorkerStatus.Running)
                InternalResolve((Result)default);
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
                for (int i = 0, len = m_fullfilled.Count; i < len; ++i)
                    m_fullfilled[i].Invoke(result);
            }
            catch { throw; }
            finally
            {
                if (continuations != null)
                    continuations = null;
                m_fullfilled.Clear();
            }
        }

        protected void InternalResolve(IWorkerLike<Result> prevWorker)
        {
            prevWorker.OnFullfilled(ret => InternalResolve(ret));
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
                for (int i = 0, len = m_rejected.Count; i < len; ++i)
                    m_rejected[i].Invoke(e);
            }
            catch { throw; }
            finally
            {
                if (continuations != null)
                    continuations = null;
                m_rejected.Clear();
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
                InternalResolve((Result)default);
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