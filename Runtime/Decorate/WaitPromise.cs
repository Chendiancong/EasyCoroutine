using System;

namespace EasyCoroutine
{
    /// <summary>
    /// 自由控制的异步对象
    /// </summary>
    [FactoryableClass]
    public class WaitPromise : Worker, IWorkerLike
    {
        public void Resolve() => InternalResolve();

        public void Reject(Exception exception) => InternalReject(exception);

        public void Reject(string reason) => InternalReject(new Exception(reason));
    }

    /// <summary>
    /// 自由控制且具备返回值的的异步对象
    /// </summary>
    [FactoryableClass]
    public class WaitPromise<Result> : Worker<Result>, IWorkerLike<Result>
    {
        public void Resolve(Result result) => InternalResolve(result);

        public void Reject(Exception exception) => InternalReject(exception);

        public void Reject(string reason) => Reject(new Exception(reason));
    }
}