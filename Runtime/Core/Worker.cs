using System;

namespace EasyCoroutine
{
    public class Worker : WorkerBase, IAwaitable
    {
        public delegate void SimpleExecutor(Action resolver);
        public delegate void Executor(Action resolver, Action<WorkerException> rejector);

        private WorkerExecution m_execution = new WorkerExecution();

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

        public void Reject(WorkerException e) => InternalReject(e);

        public void Reject(string msg) => InternalReject(new WorkerException(msg));

        public override void Reset()
        {
            if (Status == WorkerStatus.Running)
                InternalResolve();
            base.Reset();
        }

        private void InternalResolve()
        {
            try
            {
                WorkerStatus status = Status;
                if (status == WorkerStatus.Succeed || status == WorkerStatus.Failed)
                    return;
                Status = WorkerStatus.Succeed;
                InternalContinue();
                (Callback as WorkerCallback).OnFullfilled();
            }
            catch { throw; }
            finally
            {
                if (continuations != null)
                    continuations = null;
            }
        }

        private void InternalReject(WorkerException e)
        {
            try
            {
                WorkerStatus status = Status;
                if (status == WorkerStatus.Succeed || status == WorkerStatus.Failed)
                    return;
                Status = WorkerStatus.Failed;
                (Callback as WorkerCallback).OnException(e);
            }
            catch { throw; }
            finally
            {
                if (continuations != null)
                    continuations = null;
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
