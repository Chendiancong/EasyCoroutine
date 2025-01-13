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
            try
            {
                if (mFullfilled != null)
                    mFullfilled();
            } catch (WorkerException e)
            {
                OnRejected(e);
            } catch
            {
                throw;
            } finally
            {
                mFullfilled = null;
                mRejected = null;
            }
        }

        public void OnRejected(WorkerException e)
        {
            try
            {
                if (mRejected != null)
                    mRejected(e);
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
        public Action<TResult> mFullfilled;
        public Action<WorkerException> mRejected;

        public event Action<TResult> fullfilled
        {
            add => mFullfilled += value;
            remove => mFullfilled -= value;
        }

        public event Action<WorkerException> Rejected
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
            } catch (WorkerException e)
            {
                mRejected(e);
            } catch
            {
                throw;
            } finally
            {
                mFullfilled = null;
                mRejected = null;
            }
        }

        public void OnRejected(WorkerException e)
        {
            try
            {
                if (mRejected != null)
                    mRejected(e);
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