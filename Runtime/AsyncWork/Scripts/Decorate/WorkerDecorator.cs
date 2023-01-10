using System;
using AsyncWork.Core;

namespace AsyncWork
{
    public abstract class WorkerDecorator : IAwaitable
    {
        protected Worker worker;
        protected Type type;
        protected bool isPool;

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
    }

    public abstract class WorkerDecorator<T> : IAwaitable<T>
    {
        protected Worker<T> worker;
        protected Type type;
        protected bool isPool;

        public Worker<T> Worker => worker;

        public WorkerDecorator()
        {
            worker = new Worker<T>();
            type = GetType();
        }

        public Worker<T>.WorkerAwaiter GetAwaiter()
        {
            return worker.GetAwaiter();
        }

        ICustomAwaiter<T> IAwaitable<T>.GetAwaiter() => GetAwaiter();
    }
}