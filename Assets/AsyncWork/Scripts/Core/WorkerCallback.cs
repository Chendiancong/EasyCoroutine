using System;

namespace AsyncWork.Core
{
    public class WorkerCallback : IWorkerCallback
    {
        private WorkerResolve mFullfilled;
        private WorkerReject mRejected;

        public event WorkerResolve Fullfilled
        {
            add => mFullfilled += value;
            remove => mFullfilled -= value;
        }

        public event WorkerReject Rejected
        {
            add => mRejected += value;
            remove => mRejected -= value;
        }

        public void OnFullfilled()
        {
            try
            {
                if (mFullfilled != null)
                    mFullfilled();
            } catch
            {
                throw;
            } finally
            {
                mFullfilled = null;
                mRejected = null;
            }
        }

        public void OnRejected(int errCode)
        {
            try
            {
                if (mRejected != null)
                    mRejected(errCode);
            } catch
            {
                throw;
            } finally
            {
                mFullfilled = null;
                mRejected = null;
            }
        }
    }

    public class WorkerCallback<TResult> : IWorkerCallback
    {
        public WorkerResolve<TResult> mFullfilled;
        public WorkerReject mRejected;

        public event WorkerResolve<TResult> fullfilled
        {
            add => mFullfilled += value;
            remove => mFullfilled -= value;
        }

        public event WorkerReject Rejected
        {
            add => mRejected += value;
            remove => mRejected -= value;
        }

        public void OnFullfilled(TResult result)
        {
            try
            {
                if (mFullfilled != null)
                    mFullfilled(result);
            } catch
            {
                throw;
            } finally
            {
                mFullfilled = null;
                mRejected = null;
            }
        }

        public void OnRejected(int errCode)
        {
            try
            {
                if (mRejected != null)
                    mRejected(errCode);
            } catch
            {
                throw;
            } finally
            {
                mFullfilled = null;
                mRejected = null;
            }
        }
    }
}