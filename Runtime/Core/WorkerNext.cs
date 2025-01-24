using System;

namespace EasyCoroutine
{
    public struct WorkerNext : IInvokable
    {
        private IWorkerLike m_nextWorker;
        private Action m_nextAction;

        public WorkerNext(IWorkerLike nextWorker, Action nextAction)
        {
            m_nextWorker = nextWorker;
            m_nextAction = nextAction;
        }

        void IInvokable.Invoke()
        {
            m_nextAction?.Invoke();
            m_nextWorker?.Resolve();
        }
    }

    public struct WorkerNextWithOutput<Output> : IInvokable
    {
        private IWorkerLike<Output> m_nextWorker;
        private Func<Output> m_nextAction;

        public WorkerNextWithOutput(IWorkerLike<Output> nextWorker, Func<Output> nextAction)
        {
            m_nextWorker = nextWorker;
            m_nextAction = nextAction;
        }

        void IInvokable.Invoke()
        {
            Output o = m_nextAction();
            m_nextWorker?.Resolve(o);
        }
    }

    public struct WorkerNextWithInput<Input> : IInvokable<Input>
    {
        private IWorkerLike m_nextWorker;
        private Action<Input> m_nextAction;

        public WorkerNextWithInput(IWorkerLike nextWorker, Action<Input> nextAction)
        {
            m_nextWorker = nextWorker;
            m_nextAction = nextAction;
        }

        void IInvokable<Input>.Invoke(Input input)
        {
            m_nextAction(input);
            m_nextWorker?.Resolve();
        }
    }

    public struct WorkerNextWithInputOutput<Input, Output> : IInvokable<Input>
    {
        private IWorkerLike<Output> m_nextWorker;
        private Func<Input, Output> m_nextAction;

        public WorkerNextWithInputOutput(IWorkerLike<Output> nextWorker, Func<Input, Output> nextAction)
        {
            m_nextWorker = nextWorker;
            m_nextAction = nextAction;
        }

        void IInvokable<Input>.Invoke(Input input)
        {
            Output o = m_nextAction(input);
            m_nextWorker.Resolve(o);
        }
    }

    public struct ChainWorkerNext : IInvokable<IWorkerLike>
    {
        private Worker m_nextWorker;
        private Action<IWorkerLike> m_nextAction;

        void IInvokable<IWorkerLike>.Invoke(IWorkerLike input)
        {
        }
    }

    public struct ChainWorkerNextWithOutput<Output> : IInvokable<IWorkerLike<Output>>
    {
        void IInvokable<IWorkerLike<Output>>.Invoke(IWorkerLike<Output> input)
        {
        }
    }
}