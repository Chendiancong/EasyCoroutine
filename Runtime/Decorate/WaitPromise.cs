using System;

namespace EasyCoroutine
{
    /// <summary>
    /// 自由控制的异步对象
    /// </summary>
    [FactoryableClass]
    public class WaitPromise : WorkerDecorator
    {
        public void Resolve() => worker.Resolve();

        public void Reject(WorkerException exception) => worker.Reject(exception);
    }

    /// <summary>
    /// 自由控制且具备返回值的的异步对象
    /// </summary>
    [FactoryableClass]
    public class WaitPromise<Result> : WorkerDecorator<Result>
    {
        public void Resolve(Result result) => worker.Resolve(result);

        public void Reject(WorkerException exception) => worker.Reject(exception);
    }
}