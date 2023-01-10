using UnityEngine;
using AsyncWork.Core;

namespace AsyncWork
{
    public static class CoroutineExtensions
    {
        public static Worker.WorkerAwaiter GetAwaiter(this WaitForSeconds instruction) =>
            Awaiter.Wait(instruction).GetAwaiter();

        public static Worker.WorkerAwaiter GetAwaiter(this WaitForSecondsRealtime instruction) =>
            Awaiter.Wait(instruction).GetAwaiter();

        public static Worker.WorkerAwaiter GetAwaiter(this WaitForFixedUpdate instruction) =>
            Awaiter.Wait(instruction).GetAwaiter();

        public static Worker.WorkerAwaiter GetAwaiter(this WaitForEndOfFrame instruction) =>
            Awaiter.Wait(instruction).GetAwaiter();

        public static Worker.WorkerAwaiter GetAwaiter(this WaitUntil instruction) =>
            Awaiter.Wait(instruction).GetAwaiter();

        public static Worker.WorkerAwaiter GetAwaiter(this WaitWhile instruction) =>
            Awaiter.Wait(instruction).GetAwaiter();

        public static Worker<T>.WorkerAwaiter GetAwaiter<T>(this AssetBundleCreateRequest request)
            where T : UnityEngine.Object
            => new WaitAssetBundle<T>(request, WorkerRunnerBehaviour.Instance).GetAwaiter();

        public static Worker<T>.WorkerAwaiter GetAwaiter<T>(this AssetBundleRequest request)
            where T : UnityEngine.Object
            => new WaitAssetBundle<T>(request, WorkerRunnerBehaviour.Instance).GetAwaiter();
    }
}