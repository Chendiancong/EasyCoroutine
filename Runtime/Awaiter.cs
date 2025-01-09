using UnityEngine;

namespace EasyCoroutine
{
    public static class Awaiter
    {
        public static WaitInstruction Wait(YieldInstruction instruction)
        {
            return Wait(instruction, WorkerRunnerBehaviour.Instance);
        }

        public static WaitInstruction Wait(YieldInstruction instruction, IInstructionWaitable runner)
        {
            return WaitInstruction
                .factory
                .Create()
                .Start(instruction, runner);
        }

        public static WaitInstruction Wait(CustomYieldInstruction instruction)
        {
            return Wait(instruction, WorkerRunnerBehaviour.Instance);
        }

        public static WaitInstruction Wait(CustomYieldInstruction instruction, IInstructionWaitable runner)
        {
            return WaitInstruction
                .factory
                .Create()
                .Start(instruction, runner);
        }

        public static WaitBundleAsset<T> Load<T>(BundleAssetLoader loader)
            where T : UnityEngine.Object
            => Load<T>(loader, WorkerRunnerBehaviour.Instance);

        public static WaitBundleAsset<T> Load<T>(BundleAssetLoader loader, IInstructionWaitable runner)
            where T : UnityEngine.Object
        {
            AssetBundleCreateRequest abr = AssetBundle.LoadFromFileAsync(loader.path);
            return WaitBundleAsset<T>
                .factory
                .Create()
                .SetLoader(loader)
                .Start(abr, runner);
        }

        public static WaitBundleAssetMultiple<T> Load<T>(BundleAssetMultipleLoader loader)
            where T : UnityEngine.Object
            => Load<T>(loader, WorkerRunnerBehaviour.Instance);

        public static WaitBundleAssetMultiple<T> Load<T>(BundleAssetMultipleLoader loader, IInstructionWaitable runner)
            where T : UnityEngine.Object
        {
            AssetBundleCreateRequest abr = AssetBundle.LoadFromFileAsync(loader.path);
            return WaitBundleAssetMultiple<T>
                .factory
                .Create()
                .SetLoader(loader)
                .Start(abr, runner);
        }
    }
}