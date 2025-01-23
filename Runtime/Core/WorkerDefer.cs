using System;
using UnityEngine;

namespace EasyCoroutine
{
    public struct WorkerDefer : IWorkerLike
    {
        public Action Resolver { get; private set; }
        public Action<Exception> Rejecter { get; private set; }
        public IWorkerLike Worker { get; private set; }

        public static WorkerDefer Create()
        {
            var defer = new WorkerDefer();
            defer.Worker = new Worker((resolver, rejecter) => {
                defer.Resolver = resolver;
                defer.Rejecter = rejecter;
            });
            return defer;
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
            Worker.OnFullfilled(onFullfilled);
        }

        public void OnRejected(Action<Exception> onRejected)
        {
            Worker.OnRejected(onRejected);
        }
    }

    public struct WorkerDefer<Result> : IWorkerLike<Result>
    {
        public Action<Result> Resolver { get; private set; }
        public Action<Exception> Rejecter { get; private set; }
        public IWorkerLike<Result> Worker { get; private set; }

        public static WorkerDefer<Result> Create()
        {
            var defer = new WorkerDefer<Result>();
            defer.Worker = new Worker<Result>((resolver, rejecter) => {
                defer.Resolver = resolver;
                defer.Rejecter = rejecter;
            });
            return defer;
        }

        public Result GetResult() => Worker.GetResult();

        public void Resolve(Result result)
        {
            Resolver(result);
        }

        public void Reject(Exception exception)
        {
            Rejecter(exception);
        }

        public void Reject(string reason) => Reject(new Exception(reason));

        public void OnFullfilled(Action<Result> onFullfilled)
        {
            Worker.OnFullfilled(onFullfilled);
        }

        public void OnRejected(Action<Exception> onRejected)
        {
            Worker.OnRejected(onRejected);
        }
    }
}