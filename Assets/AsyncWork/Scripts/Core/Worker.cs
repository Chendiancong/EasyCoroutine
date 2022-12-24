using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AsyncWork.Core
{
    public class Worker : WorkerBase, IAwaitable
    {
        public Worker()
        {
            Callback = new WorkerCallback();
            Start();
        }

        public Worker(Action<WorkerResolve> action)
        {
            Callback = new WorkerCallback();
            action(InternalResolve);
            Start();
        }

        public Worker(Action<WorkerResolve, WorkerReject> action)
        {
            Callback = new WorkerCallback();
            action(InternalResolve, InternalReject);
            Start();
        }

        public WorkerAwaiter GetAwaiter()
        {
            return new WorkerAwaiter(this);
        }

        ICustomAwaiter IAwaitable.GetAwaiter() => GetAwaiter();

        public void Resolve() => InternalResolve();

        public void Reject(int errCode) => InternalReject(errCode);

        private void InternalResolve()
        {
            WorkerStatus status = Status;
            if (status == WorkerStatus.Succeed || status == WorkerStatus.Failed)
                return;
            Status = WorkerStatus.Succeed;
            (Callback as WorkerCallback).OnFullfilled();
            if (continuations != null)
            {
                continuations();
                continuations = null;
            }
        }

        private void InternalReject(int errCode)
        {
            WorkerStatus status = Status;
            if (status == WorkerStatus.Succeed || status == WorkerStatus.Failed)
                return;
            Status = WorkerStatus.Failed;
            (Callback as WorkerCallback).OnRejected(errCode);
        }

        public struct WorkerAwaiter : ICustomAwaiter
        {
            private Worker mWorker;

            public bool IsCompleted
            {
                get
                {
                    if (mWorker == null)
                        return true;
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
}
