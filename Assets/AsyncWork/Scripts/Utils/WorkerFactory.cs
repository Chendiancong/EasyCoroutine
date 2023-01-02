using AsyncWork.Core;
using DCFramework;

namespace AsyncWork
{
    public class WorkerPool : FactoryWithPool<Worker>
    {
        public static readonly WorkerPool instance;

        static WorkerPool()
        {
            instance = new WorkerPool();
        }

        protected WorkerPool(): base(() => new Worker(), capacity: 20, autoFill: true)
        { }
    }

    public class WorkerPool<T> : FactoryWithPool<Worker<T>>
    {
        public static readonly WorkerPool<T> instance;

        static WorkerPool()
        {
            instance = new WorkerPool<T>();
        }

        protected WorkerPool() : base(() => new Worker<T>(), capacity: 20, autoFill: true)
        { }
    }
}