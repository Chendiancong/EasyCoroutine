using System;

namespace EasyCoroutine
{
    public abstract class WorkerDecorator : IAwaitable
    {
        protected Worker worker;
        protected Type type;

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

    public abstract class WorkerDecorator<Result> : IAwaitable<Result>
    {
        protected Worker<Result> worker;
        protected Type type;

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
    }
}