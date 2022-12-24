using System;

namespace AsyncWork.Core
{
    public class Worker<TResult> : WorkerBase, IAwaitable<TResult>
    {
        private TResult mResult;

        public Worker(Action<WorkerResolve<TResult>> action)
        {
            Callback = new WorkerCallback<TResult>();
            action(Resolve);
            Start();
        }

        public Worker(Action<WorkerResolve<TResult>, WorkerReject> action)
        {
            Callback = new WorkerCallback<TResult>();
            action(Resolve, Reject);
            Start();
        }

        public WorkerAwaiter GetAwaiter()
        {
            return new WorkerAwaiter(this);
        }

        ICustomAwaiter<TResult> IAwaitable<TResult>.GetAwaiter() => GetAwaiter();

        private void Resolve(TResult result)
        {
            mResult = result;
            (Callback as WorkerCallback<TResult>).OnFullfilled(result);
            if (continuations != null)
            {
                continuations();
                continuations = null;
            }
        }

        private void Reject(int errCode)
        {
            (Callback as WorkerCallback<TResult>).OnRejected(errCode);
        }

        public struct WorkerAwaiter : ICustomAwaiter<TResult>
        {
            private Worker<TResult> mWorker;

            public bool IsCompleted
            {
                get
                {
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
    }
}