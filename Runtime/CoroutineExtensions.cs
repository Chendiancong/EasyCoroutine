using UnityEngine;

namespace EasyCoroutine
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

        // public static Worker<BundleAssetResult<T>>.WorkerAwaiter GetAwaiter<T>(this BundleAssetLoader loader)
        //     where T : UnityEngine.Object
        //     => Awaiter.Load<T>(loader).GetAwaiter();

        // public static Worker<BundleAssetMultipleResult<T>>.WorkerAwaiter GetAwaiter<T>(this BundleAssetMultipleLoader loader)
        //     where T : UnityEngine.Object
        //     => Awaiter.Load<T>(loader).GetAwaiter();
    }
}