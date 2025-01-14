using UnityEngine;

namespace EasyCoroutine
{
    public class WorkerRunnerBehaviour : SingleBehaviour<WorkerRunnerBehaviour>, IInstructionWaitable
    {
        private WorkerRunner mRunner;

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

        protected override void Awake()
        {
            base.Awake();
            mRunner = new WorkerRunner(this);
        }
    }
}