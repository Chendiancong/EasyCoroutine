using UnityEngine;

namespace EasyCoroutine
{
    [FactoryableClass]
    public class WaitInstruction: PooledWorker, IInstructionCompletable, ICustomInstructionCompletable
    {
        public static WaitInstruction Create(YieldInstruction instruction)
        {
            return FactoryMgr.PoolCreate<WaitInstruction>()
                .Start(instruction, WorkerRunnerBehaviour.Instance);
        }

        public static WaitInstruction Create(CustomYieldInstruction instrction)
        {
            return FactoryMgr.PoolCreate<WaitInstruction>()
                .Start(instrction, WorkerRunnerBehaviour.Instance);
        }

        public void OnComplete(YieldInstruction instruction)
        {
            ResolveMe(this);
        }

        public void OnComplete(CustomYieldInstruction instruction)
        {
            ResolveMe(this);
        }

        public WaitInstruction Start(YieldInstruction instruction, IInstructionWaitable runner)
        {
            if (Status == WorkerStatus.Waiting)
            {
                Start();
                runner.WaitFor(instruction, this);
            }
            return this;
        }

        public WaitInstruction Start(CustomYieldInstruction instruction, IInstructionWaitable runner)
        {
            if (Status == WorkerStatus.Waiting)
            {
                Start();
                runner.WaitFor(instruction, this);
            }
            return this;
        }

        public WaitInstruction(YieldInstruction instruction, IInstructionWaitable runner) : base()
        {
            Start(instruction, runner);
        }

        public WaitInstruction(CustomYieldInstruction instruction, IInstructionWaitable runner) : base()
        {
            Start(instruction, runner);
        }

        public WaitInstruction() : base() { }
    }
}