using UnityEngine;
using DCFramework;
using AsyncWork.Core;

namespace AsyncWork
{
    public class WorkerRunnerBehaviour : SingleBehaviour<WorkerRunnerBehaviour>, IInstructionWaitable
    {
        private WorkerRunner mRunner;

        protected override void Awake()
        {
            base.Awake();
            mRunner = new WorkerRunner(this);
        }

        public void WaitFor<T>(T instruction, IInstructionCompletable completable) where T : YieldInstruction
        {
            if (mRunner != null)
                mRunner.WaitFor(instruction, completable);
        }

        public void WaitFor<T>(T instruction, ICustomInstructionCompletable completable) where T : CustomYieldInstruction
        {
            if (mRunner != null)
                mRunner.WaitFor(instruction, completable);
        }
    }
}