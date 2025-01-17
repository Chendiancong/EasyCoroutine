using UnityEngine;

namespace EasyCoroutine
{
    [FactoryableClass]
    public class WaitInstruction: WorkerDecorator, IInstructionCompletable, ICustomInstructionCompletable
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
            worker.Resolve();
            DisposeMe(this);
        }

        public void OnComplete(CustomYieldInstruction instruction)
        {
            worker.Resolve();
            DisposeMe(this);
        }

        public WaitInstruction Start(YieldInstruction instruction, IInstructionWaitable runner)
        {
            if (worker.Status == WorkerStatus.Waiting)
            {
                worker.Start();
                runner.WaitFor(instruction, this);
            }
            return this;
        }

        public WaitInstruction Start(CustomYieldInstruction instruction, IInstructionWaitable runner)
        {
            if (worker.Status == WorkerStatus.Waiting)
            {
                worker.Start();
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