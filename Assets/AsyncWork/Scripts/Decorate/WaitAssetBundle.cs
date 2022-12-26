using UnityEngine;
using AsyncWork.Core;

namespace AsyncWork
{
    public class WaitAssetBundle<T> : WorkerDecorator<T>, IInstructionCompletable
        where T : UnityEngine.Object
    {
        private IInstructionWaitable mWaitable;
        private string mAssetName = "";
        private bool mUnloadBundle = true;
        private AssetBundle mCurBundle;

        public WaitAssetBundle<T> SetAssetName(string name)
        {
            mAssetName = name.Trim();
            return this;
        }

        public WaitAssetBundle<T> SetUnloadBundle(bool flag)
        {
            mUnloadBundle = flag;
            return this;
        }

        public void OnComplete(YieldInstruction instruction)
        {
            if (instruction is AssetBundleCreateRequest)
                HandleCreateRequest(instruction as AssetBundleCreateRequest);
            else if (instruction is AssetBundleRequest)
                HandleRequest(instruction as AssetBundleRequest);
            else
                worker.Resolve(null);
        }

        public WaitAssetBundle(AssetBundleCreateRequest createReq, IInstructionWaitable waitable) : base()
        {
            mWaitable = waitable;
            waitable.WaitFor(createReq, this);
        }

        public WaitAssetBundle(AssetBundleRequest req, IInstructionWaitable waitable) : base()
        {
            mWaitable = waitable;
            waitable.WaitFor(req, this);
        }

        private void HandleCreateRequest(AssetBundleCreateRequest request)
        {
            AssetBundle bundle = request.assetBundle;
            mCurBundle = bundle;
            AssetBundleRequest req;
            if (string.IsNullOrEmpty(mAssetName))
                req = bundle.LoadAllAssetsAsync();
            else
                req = bundle.LoadAssetAsync(mAssetName);
            mWaitable.WaitFor(req, this);
        }

        private void HandleRequest(AssetBundleRequest request)
        {
            if (string.IsNullOrEmpty(mAssetName))
                worker.Resolve(request.allAssets.Length > 0 ? request.allAssets[0] as T : null);
            else
                worker.Resolve(request.asset as T);
            if (mUnloadBundle && mCurBundle != null)
                mCurBundle.Unload(false);
            mCurBundle = null;
        }
    }
}