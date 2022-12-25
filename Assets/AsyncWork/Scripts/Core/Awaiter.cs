using UnityEngine;

namespace AsyncWork.Core
{
    public static class Awaiter
    {
        public static WaitInstruction Wait(YieldInstruction instruction)
        {
            return Wait(instruction, WorkerRunnerBehaviour.Instance);
        }

        public static WaitInstruction Wait(YieldInstruction instruction, IInstructionWaitable runner)
        {
            return new WaitInstruction(instruction, runner);
        }

        public static WaitInstruction Wait(CustomYieldInstruction instruction)
        {
            return Wait(instruction, WorkerRunnerBehaviour.Instance);
        }

        public static WaitInstruction Wait(CustomYieldInstruction instruction, IInstructionWaitable runner)
        {
            return new WaitInstruction(instruction, runner);
        }

        public static WaitAssetBundle<T> Load<T>(string path)
            where T : UnityEngine.Object
        {
            return Load<T>(path, WorkerRunnerBehaviour.Instance);
        }

        public static WaitAssetBundle<T> Load<T>(string path, IInstructionWaitable waitable)
            where T : UnityEngine.Object
        {
            AssetBundleCreateRequest ab = AssetBundle.LoadFromFileAsync(path);
            return new WaitAssetBundle<T>(ab, waitable);
        }

        public static WaitAssetBundleMultiple<T> LoadMultiple<T>(string path)
            where T : UnityEngine.Object
        {
            return LoadMultiple<T>(path, WorkerRunnerBehaviour.Instance);
        }

        public static WaitAssetBundleMultiple<T> LoadMultiple<T>(string path, IInstructionWaitable waitable)
            where T : UnityEngine.Object
        {
            AssetBundleCreateRequest ab = AssetBundle.LoadFromFileAsync(path);
            return new WaitAssetBundleMultiple<T>(ab, waitable);
        }
    }
}