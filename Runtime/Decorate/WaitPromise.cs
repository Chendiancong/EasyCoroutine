using System;

namespace EasyCoroutine
{
    /// <summary>
    /// 自由控制的异步对象
    /// </summary>
    public class WaitPromise : WorkerDecorator
    {
        public delegate void Executor(Action resolve, Action<Exception> reject);

        public WaitPromise(Executor executor)
        {
            executor(OnResolve, OnReject);
        }

        private void OnResolve()
        {
        }

        private void OnReject(Exception ex)
        {

        }
    }

    /// <summary>
    /// 自由控制且具备返回值的的异步对象
    /// </summary>
    public class WaitPromise<Result> : WorkerDecorator<Result>
    {
        public delegate void Executor(Action<Result> resolve, Action<Exception> reject);

        public WaitPromise(Executor executor)
        {
            executor(OnResolve, OnReject);
        }

        private void OnResolve(Result result)
        {

        }

        private void OnReject(Exception ex)
        {

        }
    }
}