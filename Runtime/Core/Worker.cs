using System;
using System.Collections.Generic;

namespace EasyCoroutine
{
    public class Worker : WorkerBase, IAwaitable, IWorkerLike, IThenable
    {
        public delegate void SimpleExecutor(Action resolver);
        public delegate void Executor(Action resolver, Action<Exception> rejector);
        public delegate void FullExecutor(Action resolver, Action<IWorkerLike> chainResolve, Action<Exception> rejecter);

        private List<IInvokable> m_fullfilled = new List<IInvokable>();
        private List<IInvokable<Exception>> m_rejected = new List<IInvokable<Exception>>();

        public Worker() { }

        public Worker(SimpleExecutor executor) : this()
        {
            executor(InternalResolve);
        }

        public Worker(Executor executor) : this()
        {
            executor(InternalResolve, InternalReject);
        }

        public Worker(FullExecutor executor) : this()
        {
            executor(InternalResolve, InternalChainResolve, InternalReject);
        }

        public WorkerAwaiter GetAwaiter()
        {
            return new WorkerAwaiter(this);
        }

        ICustomAwaiter IAwaitable.GetAwaiter() => GetAwaiter();

        #region IWorkerLike implementaions
        void IWorkerLike.Resolve() => InternalResolve();

        void IWorkerLike.Resolve(IWorkerLike prevWorker) => InternalChainResolve(prevWorker);

        void IWorkerLike.Reject(Exception e) => InternalReject(e);

        void IWorkerLike.Reject(string reason)
            =>InternalReject(new WorkerException(reason));

        public void OnFullfilled(Action onFullfilled)
        {
            WorkerNext next = new WorkerNext(null, onFullfilled);
            m_fullfilled.Add(next);
        }

        public void OnRejected(Action<Exception> onRejected)
        {
            WorkerRejecter rejecter = new WorkerRejecter(null, onRejected);
            m_rejected.Add(rejecter);
        }

        #endregion

        #region IThenable implementations
        public IThenable Then(Action onFullfilled)
        {
            WorkerDefer defer = WorkerDefer.Create();
            WorkerNext next = new WorkerNext(defer, onFullfilled);
            m_fullfilled.Add(next);
            return defer.Worker as Worker;
        }

        public IThenable<NextResult> Then<NextResult>(Func<NextResult> onFullfilled)
        {
            var defer = WorkerDefer<NextResult>.Create();
            var next = new WorkerNextWithOutput<NextResult>(defer, onFullfilled);
            m_fullfilled.Add(next);
            return defer.Worker as Worker<NextResult>;
        }

        public IThenable Catch(Action<Exception> onReject)
        {
            var defer = WorkerDefer.Create();
            var reject = new WorkerRejecter(defer, onReject);
            m_rejected.Add(reject);
            return defer.Worker as Worker;
        }

        public IThenable<NextResult> Catch<NextResult>(Func<Exception, NextResult> onReject)
        {
            var defer = WorkerDefer<NextResult>.Create();
            var reject = new WorkerRejecter<NextResult>(defer, onReject);
            m_rejected.Add(reject);
            return defer.Worker as Worker<NextResult>;
        }

        #endregion

        public override void Reset()
        {
            if (Status == WorkerStatus.Running)
                InternalResolve();
            base.Reset();
        }

        protected void InternalResolve()
        {
            ConfirmResolve();
        }

        protected void InternalChainResolve(IWorkerLike prevWorker)
        {
            prevWorker.OnFullfilled(() => ConfirmResolve());
        }

        private void ConfirmResolve()
        {
            try
            {
                WorkerStatus status = Status;
                if (status == WorkerStatus.Succeed || status == WorkerStatus.Failed)
                    return;
                Status = WorkerStatus.Succeed;
                InternalContinue();
                for (int i = 0, len = m_fullfilled.Count; i < len; ++i)
                    m_fullfilled[i].Invoke();
            }
            catch { throw; }
            finally
            {
                if (continuations != null)
                    continuations = null;
                m_fullfilled.Clear();
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

        private void InternalContinue() {
            if (continuations != null) {
                continuations();
            }
        }

        public struct WorkerAwaiter : ICustomAwaiter
        {
            private Worker mWorker;

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
    }

    public class PooledWorker : Worker, IPoolable
    {
        #region IPoolable implementations
        protected bool isPoolObj = false;
        void IPoolable.OnCreate() => OnPoolCreate();

        void IPoolable.OnRestore() => OnPoolRestore();

        void IPoolable.OnReuse() => OnPoolReuse();

        #endregion

        protected virtual void OnPoolCreate()
        {
            isPoolObj = true;
        }

        protected virtual void OnPoolReuse() { }

        protected virtual void OnPoolRestore()
        {
            if (Status == WorkerStatus.Running)
                InternalResolve();
            Reset();
        }

        protected static void DisposeMe<Instance>(Instance ins)
            where Instance : PooledWorker, new()
        {
            if (ins.isPoolObj)
                FactoryMgr.Restore(ins);
        }

        protected static void ResolveMe<Instance>(Instance ins)
            where Instance : PooledWorker, new()
        {
            ins.InternalResolve();
            if (ins.isPoolObj)
                FactoryMgr.Restore(ins);
        }

        protected static void RejectMe<Instance>(Instance ins, Exception e)
            where Instance : PooledWorker, new()
        {
            ins.InternalReject(e);
            if (ins.isPoolObj)
                FactoryMgr.Restore(ins);
        }

        protected static void RejectMe<Instance>(Instance ins, string reason)
            where Instance : PooledWorker, new()
            => RejectMe(ins, new Exception(reason));
    }
}
