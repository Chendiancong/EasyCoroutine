using UnityEngine;
using AsyncWork.Core;

namespace AsyncWork
{
    public class WaitInstruction: IAwaitable, IInstructionCompletable, ICustomInstructionCompletable
    {
        private Worker mWorker;

        public Worker Worker => mWorker;

        public Worker.WorkerAwaiter GetAwaiter()
        {
            return mWorker.GetAwaiter();
        }

        ICustomAwaiter IAwaitable.GetAwaiter() => GetAwaiter();

        public void OnComplete(YieldInstruction instruction)
        {
            mWorker.Resolve();
        }

        public void OnComplete(CustomYieldInstruction instruction)
        {
            mWorker.Resolve();
        }

        public WaitInstruction(YieldInstruction instruction, IInstructionWaitable runner)
        {
            mWorker = new Worker();
            runner.WaitFor(instruction, this);
        }

        public WaitInstruction(CustomYieldInstruction instruction, IInstructionWaitable runner)
        {
            mWorker = new Worker();
            runner.WaitFor(instruction, this);
        }
    }
}