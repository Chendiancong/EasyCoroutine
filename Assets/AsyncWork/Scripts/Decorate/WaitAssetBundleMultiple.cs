using UnityEngine;
using AsyncWork.Core;

namespace AsyncWork
{
    public class WaitAssetBundleMultiple<T> : WorkerDecorator<T[]>, IInstructionCompletable
        where T : UnityEngine.Object
    {
        private IInstructionWaitable mWaitable;

        public void OnComplete(YieldInstruction instruction)
        {
            if (instruction is AssetBundleCreateRequest)
                HandleCreateRequest(instruction as AssetBundleCreateRequest);
            else if (instruction is AssetBundleRequest)
                HandleRequest(instruction as AssetBundleRequest);
            else
                worker.Resolve(null);
        }

        public WaitAssetBundleMultiple(AssetBundleCreateRequest createReq, IInstructionWaitable waitable) : base()
        {
            mWaitable = waitable;
            waitable.WaitFor(createReq, this);
        }

        public WaitAssetBundleMultiple(AssetBundleRequest req, IInstructionWaitable waitable) : base()
        {
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
            worker.Resolve(req.allAssets as T[]);
        }
    }
}