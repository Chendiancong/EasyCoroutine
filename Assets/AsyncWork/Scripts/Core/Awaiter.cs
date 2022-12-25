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

        public static WaitForAssetBundle<T> Load<T>(string path)
            where T : UnityEngine.Object
        {
            return Load<T>(path, WorkerRunnerBehaviour.Instance);
        }

        public static WaitForAssetBundle<T> Load<T>(string path, IInstructionWaitable waitable)
            where T : UnityEngine.Object
        {
            AssetBundleCreateRequest ab = AssetBundle.LoadFromFileAsync(path);
            return new WaitForAssetBundle<T>(ab, waitable);
        }

        public static WaitForAssetBundleMultiple<T> LoadMultiple<T>(string path)
            where T : UnityEngine.Object
        {
            return LoadMultiple<T>(path, WorkerRunnerBehaviour.Instance);
        }

        public static WaitForAssetBundleMultiple<T> LoadMultiple<T>(string path, IInstructionWaitable waitable)
            where T : UnityEngine.Object
        {
            AssetBundleCreateRequest ab = AssetBundle.LoadFromFileAsync(path);
            return new WaitForAssetBundleMultiple<T>(ab, waitable);
        }
    }
}