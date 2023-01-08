using UnityEngine;
using AsyncWork.Core;
using DCFramework;

namespace AsyncWork
{
    public class WaitAssetBundle<T> : WorkerDecorator<T>, IInstructionCompletable, IPoolable
        where T : UnityEngine.Object
    {
        public readonly static FactoryWithPool<WaitAssetBundle<T>> factory =
            new FactoryWithPool<WaitAssetBundle<T>>(() => new WaitAssetBundle<T>());

        static WaitAssetBundle() { }

        private IInstructionWaitable mWaitable;
        private string mAssetName = "";
        private bool mUnloadBundle = true;
        private bool mIsPool;
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

        public WaitAssetBundle<T> Start(AssetBundleCreateRequest createReq, IInstructionWaitable waitable)
        {
            if (worker.Status == WorkerStatus.Waiting)
            {
                worker.Start();
                mWaitable = waitable;
                waitable.WaitFor(createReq, this);
            }
            return this;
        }

        public WaitAssetBundle<T> Start(AssetBundleRequest req, IInstructionWaitable waitable)
        {
            if (worker.Status == WorkerStatus.Waiting)
            {
                worker.Start();
                mWaitable = waitable;
                waitable.WaitFor(req, this);
            }
            return this;
        }


        public WaitAssetBundle(AssetBundleCreateRequest createReq, IInstructionWaitable waitable) : base()
        {
            Start(createReq, waitable);
        }

        public WaitAssetBundle(AssetBundleRequest req, IInstructionWaitable waitable) : base()
        {
            Start(req, waitable);
        }

        public WaitAssetBundle() : base() { }

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