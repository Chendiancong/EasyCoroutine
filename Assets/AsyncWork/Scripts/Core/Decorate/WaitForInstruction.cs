using System.Collections;
using UnityEngine;

namespace AsyncWork.Core
{
    public class WaitForInstruction: IAwaitable
    {
        private Worker mWorker;

        public Worker Worker => mWorker;

        public Worker.WorkerAwaiter GetAwaiter()
        {
            return mWorker.GetAwaiter();
        }

        ICustomAwaiter IAwaitable.GetAwaiter() => GetAwaiter();

        public WaitForInstruction(YieldInstruction instruction, MonoBehaviour runner)
        {
            mWorker = new Worker();
            runner.StartCoroutine(CoForYieldInstruction(instruction));
        }

        public WaitForInstruction(CustomYieldInstruction instruction, MonoBehaviour runner)
        {
            mWorker = new Worker();
            runner.StartCoroutine(CoForCustomYieldInstruction(instruction));
        }

        private IEnumerator CoForYieldInstruction(YieldInstruction instruction)
        {
            yield return instruction;
            mWorker.Resolve();
        }

        private IEnumerator CoForCustomYieldInstruction(CustomYieldInstruction instruction)
        {
            yield return instruction;
            mWorker.Resolve();
        }
    }
}