using System;

namespace EasyCoroutine
{

    public class WorkerCallback : IWorkerCallback
    {
        private Action mFullfilled;
        private Action<WorkerException> mRejected;

        public event Action Fullfilled
        {
            add => mFullfilled += value;
            remove => mFullfilled -= value;
        }

        public event Action<WorkerException> Rejected
        {
            add => mRejected += value;
            remove => mRejected -= value;
        }

        public void OnFullfilled()
        {
            try { mFullfilled?.Invoke(); }
            catch { throw; }
            finally
            {
                mFullfilled = null;
                mRejected = null;
            }
        }

        public void OnException(WorkerException e)
        {
            try { mRejected?.Invoke(e); }
            catch { throw; }
            finally
            {
                mFullfilled = null;
                mRejected = null;
            }
        }
    }

    public class WorkerCallback<TResult> : IWorkerCallback
    {
        private Action<TResult> mFullfilled;
        private Action<WorkerException> mRejected;

        public event Action<TResult> Fullfilled
        {
            add => mFullfilled += value;
            remove => mFullfilled -= value;
        }

        public event Action<WorkerException> Rejected
        {
            add => mRejected += value;
            remove => mRejected -= value;
        }

        public void OnFullfiled(TResult result)
        {
            try { mFullfilled?.Invoke(result); }
            catch { throw; }
            finally
            {
                mFullfilled = null;
                mRejected = null;
            }
        }

        public void OnException(WorkerException e)
        {
            try { mRejected?.Invoke(e); }
            catch { throw; }
            finally
            {
                mFullfilled = null;
                mRejected = null;
            }
        }
    }
}