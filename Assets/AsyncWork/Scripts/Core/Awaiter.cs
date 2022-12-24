using UnityEngine;

namespace AsyncWork.Core
{
    public static class Awaiter
    {
        public static WaitForInstruction Wait(YieldInstruction instruction, MonoBehaviour runner)
        {
            return new WaitForInstruction(instruction, runner);
        }
    }
}