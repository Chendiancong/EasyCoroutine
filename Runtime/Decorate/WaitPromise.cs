using System;

namespace EasyCoroutine
{
    /// <summary>
    /// 自由控制的异步对象
    /// </summary>
    [FactoryableClass]
    public class WaitPromise : Worker, IPoolable
    {
        private bool mIsPool = false;

        public void OnCreate()
        {
            mIsPool = true;
        }

        public void OnReuse() { }

        public void OnRestore()
        {
            Reset();
        }
    }

    /// <summary>
    /// 自由控制且具备返回值的的异步对象
    /// </summary>
    [FactoryableClass]
    public class WaitPromise<Result> : Worker<Result>, IPoolable
    {
        private bool mIsPool = false;

        public void OnCreate()
        {
            mIsPool = true;
        }

        public void OnReuse() { }

        public void OnRestore()
        {
            ResetWorker();
        }
    }
}