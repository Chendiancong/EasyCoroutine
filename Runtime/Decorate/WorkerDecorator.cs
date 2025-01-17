using System;

namespace EasyCoroutine
{
    public class WorkerDecorator : IAwaitable, IPoolable
    {
        protected Worker worker;
        protected Type type;
        protected bool isPoolObj;

        public Worker Worker => worker;

        public WorkerDecorator()
        {
            worker = new Worker();
            type = GetType();
        }

        public Worker.WorkerAwaiter GetAwaiter()
        {
            return worker.GetAwaiter();
        }

        ICustomAwaiter IAwaitable.GetAwaiter() => GetAwaiter();

        public void ResetWorker()
        {
            worker.Reset();
        }

        protected void DisposeMe<T>(T ins)
            where T : WorkerDecorator, new()
        {
            if (ins.isPoolObj)
                FactoryMgr.Restore(ins);
        }

        void IPoolable.OnCreate()
        {
            isPoolObj = true;
        }

        void IPoolable.OnReuse()
        {
        }

        void IPoolable.OnRestore()
        {
            if (worker.Status == WorkerStatus.Running)
                worker.Resolve();
            worker.Reset();
        }
    }

    public class WorkerDecorator<Result> : IAwaitable<Result>, IPoolable
    {
        protected Worker<Result> worker;
        protected Type type;
        protected bool isPoolObj;

        public Worker<Result> Worker => worker;

        public WorkerDecorator()
        {
            worker = new Worker<Result>();
            type = GetType();
        }

        public Worker<Result>.WorkerAwaiter GetAwaiter()
        {
            return worker.GetAwaiter();
        }

        ICustomAwaiter<Result> IAwaitable<Result>.GetAwaiter() => GetAwaiter();

        protected void ResetWorker()
        {
            worker.Reset();
        }

        protected void DisposeMe<T>(T ins)
            where T: WorkerDecorator<Result>, new()
        {
            if (ins.isPoolObj)
                FactoryMgr.Restore(ins);
        }

        void IPoolable.OnCreate()
        {
            isPoolObj = true;
        }

        void IPoolable.OnReuse()
        {
        }

        void IPoolable.OnRestore()
        {
            if (worker.Status == WorkerStatus.Running)
                worker.Resolve(default);
            worker.Reset();
        }
    }
}