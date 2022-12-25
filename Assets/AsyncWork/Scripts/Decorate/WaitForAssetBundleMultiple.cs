using UnityEngine;
using AsyncWork.Core;

namespace AsyncWork
{
    public class WaitForAssetBundleMultiple<T> : IAwaitable<T[]>, IInstructionCompletable
        where T : UnityEngine.Object
    {
        private Worker<T[]> mWorker;
        private IInstructionWaitable mWaitable;

        public Worker<T[]>.WorkerAwaiter GetAwaiter()
        {
            return mWorker.GetAwaiter();
        }

        ICustomAwaiter<T[]> IAwaitable<T[]>.GetAwaiter() => GetAwaiter();

        public void OnComplete(YieldInstruction instruction)
        {
            if (instruction is AssetBundleCreateRequest)
                HandleCreateRequest(instruction as AssetBundleCreateRequest);
            else if (instruction is AssetBundleRequest)
                HandleRequest(instruction as AssetBundleRequest);
            else
                mWorker.Resolve(null);
        }

        public WaitForAssetBundleMultiple(AssetBundleCreateRequest createReq, IInstructionWaitable waitable)
        {
            mWorker = new Worker<T[]>();
            mWaitable = waitable;
            waitable.WaitFor(createReq, this);
        }

        public WaitForAssetBundleMultiple(AssetBundleRequest req, IInstructionWaitable waitable)
        {
            mWorker = new Worker<T[]>();
            mWaitable = waitable;
            waitable.WaitFor(req, this);

        }

        private void HandleCreateRequest(AssetBundleCreateRequest request)
        {
            AssetBundle bundle = request.assetBundle;
            AssetBundleRequest req = bundle.LoadAllAssetsAsync();
            mWaitable.WaitFor(req, this);
        }

        private void HandleRequest(AssetBundleRequest req)
        {
            mWorker.Resolve(req.allAssets as T[]);
        }
    }
}