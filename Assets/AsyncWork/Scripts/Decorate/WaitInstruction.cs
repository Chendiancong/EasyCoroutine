using UnityEngine;
using AsyncWork.Core;

namespace AsyncWork
{
    public class WaitInstruction: WorkerDecorator, IInstructionCompletable, ICustomInstructionCompletable
    {
        public void OnComplete(YieldInstruction instruction)
        {
            worker.Resolve();
        }

        public void OnComplete(CustomYieldInstruction instruction)
        {
            worker.Resolve();
        }

        public WaitInstruction(YieldInstruction instruction, IInstructionWaitable runner) : base()
        {
            runner.WaitFor(instruction, this);
        }

        public WaitInstruction(CustomYieldInstruction instruction, IInstructionWaitable runner) : base()
        {
            runner.WaitFor(instruction, this);
        }
    }
}