using System;

namespace EasyCoroutine
{
    /// <summary>
    /// 自由控制的异步对象
    /// </summary>
    [FactoryableClass]
    public class WaitPromise : WorkerDecorator, IPoolable
    {
        public delegate void Executor(Action resolve, Action<WorkerException> reject);

        private bool mIsPool = false;

        public WaitPromise(Executor executor)
        {
            executor(PromiseResolve, PromiseReject);
        }

        public WaitPromise() { }

        public void Resolve() => PromiseResolve();

        public void Reject(string msg) => PromiseReject(new WorkerException(msg));

        public void Reject(WorkerException ex) => PromiseReject(ex);

        public void OnCreate()
        {
            mIsPool = true;
        }

        public void OnReuse() { }

        public void OnRestore()
        {
            ResetWorker();
        }

        private void PromiseResolve()
        {
            worker.Resolve();
            if (mIsPool)
                FactoryMgr.Restore(this);
        }

        private void PromiseReject(WorkerException ex)
        {
            try { worker.Reject(ex); }
            catch { throw; }
            finally
            {
                if (mIsPool)
                    FactoryMgr.Restore(this);
            }
        }
    }

    /// <summary>
    /// 自由控制且具备返回值的的异步对象
    /// </summary>
    [FactoryableClass]
    public class WaitPromise<Result> : WorkerDecorator<Result>, IPoolable
    {
        public delegate void Executor(Action<Result> resolve, Action<WorkerException> reject);

        private bool mIsPool = false;

        public WaitPromise(Executor executor)
        {
            executor(PromiseResolve, PromiseReject);
        }

        public WaitPromise() { }


        public void OnCreate()
        {
            mIsPool = true;
        }

        public void OnReuse() { }

        public void OnRestore()
        {
            ResetWorker();
        }

        private void PromiseResolve(Result result)
        {
            worker.Resolve(result);
            if (mIsPool)
                FactoryMgr.Restore(this);
        }

        private void PromiseReject(WorkerException ex)
        {
            try { worker.Reject(ex); }
            catch { throw; }
            finally
            {
                if (mIsPool)
                    FactoryMgr.Restore(this);
            }
        }
    }
}