using UnityEngine;
using AsyncWork.Core;
using DCMisc;

namespace AsyncWork
{
    public class WaitAssetBundleMultiple<T> : WorkerDecorator<T[]>, IInstructionCompletable, IPoolable
        where T : UnityEngine.Object
    {
        public readonly static FactoryWithPool<WaitAssetBundleMultiple<T>> factory =
            new FactoryWithPool<WaitAssetBundleMultiple<T>>(() => new WaitAssetBundleMultiple<T>());

        static WaitAssetBundleMultiple() { }

        private IInstructionWaitable mWaitable;
        private bool mUnloadBundle = true;
        private bool mIsPool;
        private AssetBundle mCurBundle;

        public void OnComplete(YieldInstruction instruction)
        {
            if (instruction is AssetBundleCreateRequest)
                HandleCreateRequest(instruction as AssetBundleCreateRequest);
            else if (instruction is AssetBundleRequest)
                HandleRequest(instruction as AssetBundleRequest);
            else
            {
                worker.Resolve(null);
                if (mIsPool)
                    factory.Restore(this);
            }
        }

        public void OnCreate()
        {
            mIsPool = true;
        }

        public void OnRestore()
        {
            if (worker.Status == WorkerStatus.Running)
                worker.Resolve(null);
            worker.Reset();
        }

        public void OnReuse()
        {

        }

        public WaitAssetBundleMultiple<T> Start(AssetBundleCreateRequest createReq, IInstructionWaitable waitable)
        {
            if (worker.Status == WorkerStatus.Waiting)
            {
                worker.Start();
                mWaitable = waitable;
                waitable.WaitFor(createReq, this);
            }
            return this;
        }

        public WaitAssetBundleMultiple<T> Start(AssetBundleRequest req, IInstructionWaitable waitable)
        {
            if (worker.Status == WorkerStatus.Waiting)
            {
                worker.Start();
                mWaitable = waitable;
                waitable.WaitFor(req, this);
            }
            return this;
        }

        public WaitAssetBundleMultiple(AssetBundleCreateRequest createReq, IInstructionWaitable waitable) : base()
        {
            Start(createReq, waitable);
        }

        public WaitAssetBundleMultiple(AssetBundleRequest req, IInstructionWaitable waitable) : base()
        {
            Start(req, waitable);
        }

        public WaitAssetBundleMultiple() { }

        private void HandleCreateRequest(AssetBundleCreateRequest request)
        {
            AssetBundle bundle = request.assetBundle;
            mCurBundle = bundle;
            AssetBundleRequest req = bundle.LoadAllAssetsAsync();
            mWaitable.WaitFor(req, this);
        }

        private void HandleRequest(AssetBundleRequest req)
        {
            worker.Resolve(req.allAssets as T[]);
            if (mUnloadBundle && mCurBundle != null)
                mCurBundle.Unload(false);
            mCurBundle = null;
        }
    }
}