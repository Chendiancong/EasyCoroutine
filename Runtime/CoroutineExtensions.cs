using UnityEngine;

namespace EasyCoroutine
{
    public static class CoroutineExtensions
    {
        public static Worker.WorkerAwaiter GetAwaiter(this YieldInstruction instruction) =>
            WaitInstruction.Create(instruction).GetAwaiter();

        public static Worker.WorkerAwaiter GetAwaiter(this CustomYieldInstruction instruction) =>
            WaitInstruction.Create(instruction).GetAwaiter();
    }
}