using System;

namespace EasyCoroutine
{
    public class Worker : WorkerBase, IAwaitable, IPoolable
    {
        private WorkerAction mWorkerAction = new WorkerAction();

        public Worker()
        {
            Callback = new WorkerCallback();
        }

        public Worker(Action<Action> action)
        {
            Callback = new WorkerCallback();
            mWorkerAction.action1 = action;
        }

        public Worker(Action<Action, Action<WorkerException>> action)
        {
            Callback = new WorkerCallback();
            mWorkerAction.action2 = action;
        }

        public WorkerAwaiter GetAwaiter()
        {
            return new WorkerAwaiter(this);
        }

        ICustomAwaiter IAwaitable.GetAwaiter() => GetAwaiter();

        public void OnCreate() { }

        public void OnReuse() { }

        public void OnRestore() { }

        public void Resolve() => InternalResolve();

        public void Reject(WorkerException e) => InternalReject(e);

        private void InternalResolve()
        {
            try
            {
                WorkerStatus status = Status;
                if (status == WorkerStatus.Succeed || status == WorkerStatus.Failed)
                    return;
                Status = WorkerStatus.Succeed;
                (Callback as WorkerCallback).OnFullfilled();
                InternalContinue();
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
                (Callback as WorkerCallback).OnRejected(e);
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

                    ref WorkerAction wAction = ref mWorker.mWorkerAction;
                    if (wAction.action1 != null)
                        wAction.action1(mWorker.InternalResolve);
                    else if (wAction.action2 != null)
                        wAction.action2(mWorker.InternalResolve, mWorker.InternalReject);
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

        private struct WorkerAction
        {
            public Action<Action> action1;
            public Action<Action, Action<WorkerException>> action2;
            public void Clear()
            {
                action1 = null;
                action2 = null;
            }
        }
    }
}
