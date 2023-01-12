using System;

namespace EasyCoroutine
{
    public abstract class WorkerBase
    {
        protected static int workerId = 0;

        protected Action continuations;
        protected int id = ++workerId;

        public WorkerStatus Status { get; protected set; } = WorkerStatus.Waiting;
        public IWorkerCallback Callback { get; protected set; }

        public virtual void Start()
        {
            if (Status == WorkerStatus.Waiting)
                Status = WorkerStatus.Running;
        }

        public virtual void Reset()
        {
            Status = WorkerStatus.Waiting;
        }
    }
}