using UnityEngine;

namespace AsyncWork.Core
{
    public static class Awaiter
    {
        public static WaitForInstruction Wait(YieldInstruction instruction)
        {
            return Wait(instruction, WorkerRunnerBehaviour.Instance);
        }

        public static WaitForInstruction Wait(YieldInstruction instruction, IInstructionWaitable runner)
        {
            return new WaitForInstruction(instruction, runner);
        }

        public static WaitForInstruction Wait(CustomYieldInstruction instruction)
        {
            return Wait(instruction, WorkerRunnerBehaviour.Instance);
        }

        public static WaitForInstruction Wait(CustomYieldInstruction instruction, IInstructionWaitable runner)
        {
            return new WaitForInstruction(instruction, runner);
        }
    }
}