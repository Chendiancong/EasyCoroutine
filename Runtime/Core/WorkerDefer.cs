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

        public void OnFullfilled(Action onFullfilled)
        {
            (Worker as IWorkerLike).OnFullfilled(onFullfilled);
        }

        public void OnRejected(Action<Exception> onRejected)
        {
            (Worker as IWorkerLike).OnRejected(onRejected);
        }
    }

    public class WorkerDefer<Result> : IWorkerLike<Result>
    {
        public Action<Result> Resolver { get; private set; }
        public Action<IWorkerLike<Result>> ChainResolver { get; private set; }
        public Action<Exception> Rejecter { get; private set; }
        public Worker<Result> Worker { get; private set; }

        public WorkerDefer()
        {
            Worker = new Worker<Result>((resolver, chainResolver, rejecter) => {
                Resolver = resolver;
                ChainResolver = chainResolver;
                Rejecter = rejecter;
            });
        }

        public void Resolve(Result result)
        {
            Resolver(result);
        }

        public void Resolve(IWorkerLike<Result> workerLike)
        {
            ChainResolver(workerLike);
        }

        public void Reject(Exception exception)
        {
            Rejecter(exception);
        }

        public void Reject(string reason) => Reject(new Exception(reason));

        public void OnFullfilled(Action<Result> onFullfilled)
        {
            (Worker as IWorkerLike<Result>).OnFullfilled(onFullfilled);
        }

        public void OnRejected(Action<Exception> onRejected)
        {
            (Worker as IWorkerLike<Result>).OnRejected(onRejected);
        }
    }
}