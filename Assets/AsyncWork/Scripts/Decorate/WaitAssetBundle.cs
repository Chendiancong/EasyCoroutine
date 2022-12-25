using System.Text;
using UnityEngine;
using AsyncWork.Core;

namespace AsyncWork
{
    public class WaitAssetBundle<T> : IAwaitable<T>, IInstructionCompletable
        where T : UnityEngine.Object
    {
        private Worker<T> mWorker;
        private IInstructionWaitable mWaitable;
        private string assetName = "";

        public Worker<T> Worker => mWorker;

        public Worker<T>.WorkerAwaiter GetAwaiter()
        {
            return mWorker.GetAwaiter();
        }

        ICustomAwaiter<T> IAwaitable<T>.GetAwaiter() => GetAwaiter();

        public WaitAssetBundle<T> SetAssetName(string name)
        {
            assetName = name.Trim();
            return this;
        }

        public void OnComplete(YieldInstruction instruction)
        {
            if (instruction is AssetBundleCreateRequest)
                HandleCreateRequest(instruction as AssetBundleCreateRequest);
            else if (instruction is AssetBundleRequest)
                HandleRequest(instruction as AssetBundleRequest);
            else
                mWorker.Resolve(null);
        }

        public WaitAssetBundle(AssetBundleCreateRequest createReq, IInstructionWaitable waitable)
        {
            mWorker = new Worker<T>();
            mWaitable = waitable;
            waitable.WaitFor(createReq, this);
        }

        public WaitAssetBundle(AssetBundleRequest req, IInstructionWaitable waitable)
        {
            mWorker = new Worker<T>();
            mWaitable = waitable;
            waitable.WaitFor(req, this);
        }

        private void HandleCreateRequest(AssetBundleCreateRequest request)
        {
            AssetBundle bundle = request.assetBundle;
            AssetBundleRequest req;
            if (string.IsNullOrEmpty(assetName))
                req = bundle.LoadAllAssetsAsync();
            else
                req = bundle.LoadAssetAsync(assetName);
            mWaitable.WaitFor(req, this);
        }

        private void HandleRequest(AssetBundleRequest request)
        {
            if (string.IsNullOrEmpty(assetName))
                mWorker.Resolve(request.allAssets.Length > 0 ? request.allAssets[0] as T : null);
            else
                mWorker.Resolve(request.asset as T);
        }
    }
}