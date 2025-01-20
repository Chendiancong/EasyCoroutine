using System;

namespace EasyCoroutine
{
    /// <summary>
    /// 自由控制的异步对象
    /// </summary>
    [FactoryableClass]
    public class WaitPromise : WorkerDecorator, IWorkerLike
    {
        public void Resolve() => worker.Resolve();

        public void Reject(Exception exception) => worker.Reject(exception);

        public void Reject(string reason) => Reject(new Exception(reason));
    }

    /// <summary>
    /// 自由控制且具备返回值的的异步对象
    /// </summary>
    [FactoryableClass]
    public class WaitPromise<Result> : WorkerDecorator<Result>, IWorkerLike<Result>
    {
        public void Resolve(Result result) => worker.Resolve(result);

        public void Reject(Exception exception) => worker.Reject(exception);

        public void Reject(string reason) => Reject(new Exception(reason));
    }
}