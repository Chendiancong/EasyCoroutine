using System;

namespace EasyCoroutine
{

    public class WorkerCallback : IWorkerCallback
    {
        private Action m_fullfilled;
        private Action<WorkerException> m_rejected;

        public event Action Fullfilled
        {
            add => m_fullfilled += value;
            remove => m_fullfilled -= value;
        }

        public event Action<WorkerException> Rejected
        {
            add => m_rejected += value;
            remove => m_rejected -= value;
        }

        public void OnFullfilled()
        {
            try { m_fullfilled?.Invoke(); }
            catch { throw; }
            finally
            {
                m_fullfilled = null;
                m_rejected = null;
            }
        }

        public void OnException(WorkerException e)
        {
            try { m_rejected?.Invoke(e); }
            catch { throw; }
            finally
            {
                m_fullfilled = null;
                m_rejected = null;
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