using System;
using UnityEngine;

namespace EasyCoroutine
{
    public static class CoroutineExtensions
    {
        public static Worker.WorkerAwaiter GetAwaiter(this YieldInstruction instruction) =>
            WaitInstruction.Create(instruction).GetAwaiter();

        public static Worker.WorkerAwaiter GetAwaiter(this CustomYieldInstruction instruction) =>
            WaitInstruction.Create(instruction).GetAwaiter();

        public static IThenable Then(this YieldInstruction instruction, Action nextAction) =>
            WaitInstruction.Create(instruction).Then(nextAction);

        public static IThenable Then(this CustomYieldInstruction instruction, Action nextAction) =>
            WaitInstruction.Create(instruction).Then(nextAction);
    }
}