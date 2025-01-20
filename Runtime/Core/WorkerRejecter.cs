using System;

namespace EasyCoroutine
{
    public struct WorkerRejecter : IInvokable<Exception>
    {
        private IWorkerLike m_nextWorker;
        private Action<Exception> m_catcher;

        public WorkerRejecter(IWorkerLike nextWorker, Action<Exception> catcher)
        {
            m_nextWorker = nextWorker;
            m_catcher = catcher;
        }

        void IInvokable<Exception>.Invoke(Exception exception)
        {
            try
            {
                m_catcher(exception);
                m_nextWorker.Resolve();
            }
            catch (Exception _ex)
            {
                m_nextWorker.Reject(_ex);
            }
        }
    }

    public struct WorkerRejecter<Output> : IInvokable<Exception>
    {
        private IWorkerLike<Output> m_nextWorker;
        private Func<Exception, Output> m_catcher;

        public WorkerRejecter(IWorkerLike<Output> nextWorker, Func<Exception, Output> catcher)
        {
            m_nextWorker = nextWorker;
            m_catcher = catcher;
        }

        void IInvokable<Exception>.Invoke(Exception exception)
        {
            try
            {
                Output o = m_catcher(exception);
                m_nextWorker.Resolve(o);
            }
            catch (Exception _ex)
            {
                m_nextWorker.Reject(_ex);
            }
        }
    }
}