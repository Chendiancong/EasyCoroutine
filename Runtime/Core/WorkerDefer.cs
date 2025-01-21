using System;

namespace EasyCoroutine
{
    public class WorkerDefer : IWorkerLike
    {
        public Action Resolver { get; private set; }
        public Action<Exception> Rejecter { get; private set; }
        public Worker Worker { get; private set; }

        public WorkerDefer()
        {
            Worker = new Worker((resolver, rejecter) => {
                Resolver = resolver;
                Rejecter = rejecter;
            });
        }

        public void Resolve()
        {
            Resolver.Invoke();
        }

        public void Reject(Exception exception)
        {
            Rejecter.Invoke(exception);
        }

        public void Reject(string reason) => Reject(new Exception(reason));
    }

    public class WorkerDefer<Result> : IWorkerLike<Result>
    {
        public Action<Result> Resolver { get; private set; }
        public Action<Exception> Rejecter { get; private set; }
        public Worker<Result> Worker { get; private set; }

        public WorkerDefer()
        {
            Worker = new Worker<Result>((resolver, rejecter) => {
                Resolver = resolver;
                Rejecter = rejecter;
            });
        }

        public void Resolve(Result result)
        {
            Resolver(result);
        }

        public void Reject(Exception exception)
        {
            Rejecter(exception);
        }

        public void Reject(string reason) => Reject(new Exception(reason));
    }
}