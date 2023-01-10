using System;
using DCMisc;

namespace AsyncWork.Core
{
    public class Worker<TResult> : WorkerBase, IAwaitable<TResult>, IPoolable
    {
        private TResult mResult;
        private WorkerAction mWorkerAction = new WorkerAction();

        public Worker()
        {
            Callback = new WorkerCallback<TResult>();
        }

        public Worker(Action<Action<TResult>> action)
        {
            Callback = new WorkerCallback<TResult>();
            mWorkerAction.action1 = action;
        }

        public Worker(Action<Action<TResult>, Action<WorkerException>> action)
        {
            Callback = new WorkerCallback<TResult>();
            mWorkerAction.action2 = action;
        }

        public WorkerAwaiter GetAwaiter()
        {
            return new WorkerAwaiter(this);
        }

        ICustomAwaiter<TResult> IAwaitable<TResult>.GetAwaiter() => GetAwaiter();

        public void OnCreate() { }

        public void OnReuse() { }

        public void OnRestore() { }

        public void Resolve(TResult result) => InternalResolve(result);

        public void Reject(WorkerException e) => InternalReject(e);

        private void InternalResolve(TResult result)
        {
            mResult = result;
            try
            {
                WorkerStatus status = Status;
                if (status == WorkerStatus.Succeed || status == WorkerStatus.Failed)
                    return;
                Status = WorkerStatus.Succeed;
                (Callback as WorkerCallback<TResult>).OnFullfilled(result);
                if (continuations != null)
                    continuations();
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
                (Callback as WorkerCallback<TResult>).OnRejected(e);
            }
            catch { throw; }
            finally
            {
                if (continuations != null)
                    continuations = null;
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

                    ref WorkerAction wAction = ref mWorker.mWorkerAction;
                    if (wAction.action1 != null)
                        wAction.action1(mWorker.Resolve);
                    else if (wAction.action2 != null)
                        wAction.action2(mWorker.Resolve, mWorker.Reject);
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
                return mWorker.mResult;
            }

            public void OnCompleted(Action continuation)
            {
                mWorker.continuations += continuation;
            }
        }

        private struct WorkerAction
        {
            public Action<Action<TResult>> action1;
            public Action<Action<TResult>, Action<WorkerException>> action2;

            public void Clear()
            {
                action1 = null;
                action2 = null;
            }
        }
    }
}