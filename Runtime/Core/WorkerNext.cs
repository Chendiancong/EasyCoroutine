namespace EasyCoroutine
{
    public struct WorkerNext : IInvokable
    {
        private Worker m_nextWorker;
        

        void IInvokable.Invoke()
        {
        }
    }
}