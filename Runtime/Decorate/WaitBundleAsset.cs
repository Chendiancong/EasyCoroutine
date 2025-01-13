using UnityEngine;

namespace EasyCoroutine
{
    [FactoryableClass]
    public class WaitBundleAsset<T> : WorkerDecorator<BundleAssetResult<T>>, IInstructionCompletable, IPoolable
        where T : UnityEngine.Object
    {
        // public readonly static FactoryWithPool<WaitBundleAsset<T>> factory =
        //     new FactoryWithPool<WaitBundleAsset<T>>(() => new WaitBundleAsset<T>());

        static WaitBundleAsset() { }

        private IInstructionWaitable mWaitable;
        private BundleAssetLoader<T> mLoader;
        private bool mIsPool;
        private AssetBundle mCurBundle;

        public WaitBundleAsset<T> SetLoader(BundleAssetLoader<T> loader)
        {
            mLoader = loader;
            return this;
        }

        public void OnComplete(YieldInstruction instruction)
        {
            if (instruction is AssetBundleCreateRequest)
                HandleCreateRequest(instruction as AssetBundleCreateRequest);
            else if (instruction is AssetBundleRequest)
                HandleRequest(instruction as AssetBundleRequest);
            else
                InternalResolve(default);
        }

        public void OnCreate()
        {
            mIsPool = true;
        }

        public void OnRestore()
        {
            if (worker.Status == WorkerStatus.Running)
                worker.Resolve(default);
            worker.Reset();
        }

        public void OnReuse()
        {
        }

        public WaitBundleAsset<T> Start(AssetBundleCreateRequest createReq, IInstructionWaitable waitable)
        {
            if (worker.Status == WorkerStatus.Waiting)
            {
                worker.Start();
                mWaitable = waitable;
                waitable.WaitFor(createReq, this);
            }
            return this;
        }

        public WaitBundleAsset<T> Start(AssetBundleRequest req, IInstructionWaitable waitable)
        {
            if (worker.Status == WorkerStatus.Waiting)
            {
                worker.Start();
                mWaitable = waitable;
                waitable.WaitFor(req, this);
            }
            return this;
        }


        public WaitBundleAsset(AssetBundleCreateRequest createReq, IInstructionWaitable waitable) : base()
        {
            Start(createReq, waitable);
        }

        public WaitBundleAsset(AssetBundleRequest req, IInstructionWaitable waitable) : base()
        {
            Start(req, waitable);
        }

        public WaitBundleAsset() : base() { }

        private void HandleCreateRequest(AssetBundleCreateRequest request)
        {
            if (!request.isDone) {
                InternalReject(new WorkerException($"Load {mLoader.path} failed"));
                return;
            }
            AssetBundle bundle = request.assetBundle;
            mCurBundle = bundle;
            AssetBundleRequest req;
            if (string.IsNullOrEmpty(mLoader.assetName))
                req = bundle.LoadAllAssetsAsync();
            else
                req = bundle.LoadAssetAsync(mLoader.assetName);
            mWaitable.WaitFor(req, this);
        }

        private void HandleRequest(AssetBundleRequest request)
        {
            T targetAsset;
            if (string.IsNullOrEmpty(mLoader.assetName))
                targetAsset = request.allAssets.Length > 0 ? request.allAssets[0] as T : null;
            else
                targetAsset = request.asset as T;
            if (mLoader.autoUnloadBundle)
                mCurBundle?.Unload(false);
            AssetBundle bundle = mCurBundle;
            mCurBundle = null;

            InternalResolve(new BundleAssetResult<T> {
                asset = targetAsset,
                bundleValid = !mLoader.autoUnloadBundle,
                assetBundle = bundle
            });
        }

        private void InternalResolve(BundleAssetResult<T> result)
        {
            worker.Resolve(result);
            if (mIsPool)
                FactoryMgr.Restore(this);
        }

        private void InternalReject(WorkerException exception)
        {
            try { worker.Reject(exception); }
            catch { throw; }
            finally
            {
                if (mIsPool)
                    FactoryMgr.Restore(this);
            }
        }
    }

    /// <summary>
    /// 单一AssetBundle资源请求
    /// </summary>
    public struct BundleAssetLoader<Asset> : IAwaitable<BundleAssetResult<Asset>>
        where Asset : UnityEngine.Object
    {
        /// <summary>
        /// bundle的url
        /// </summary>
        public string path;
        /// <summary>
        /// bundle中资源的名称
        /// </summary>
        public string assetName;
        /// <summary>
        /// 是否释放asset bundle
        /// </summary>
        public bool autoUnloadBundle;

        public Worker<BundleAssetResult<Asset>>.WorkerAwaiter GetAwaiter() =>
            Awaiter.Load(this).GetAwaiter();

        ICustomAwaiter<BundleAssetResult<Asset>> IAwaitable<BundleAssetResult<Asset>>.GetAwaiter() =>
            GetAwaiter();
    }

    /// <summary>
    /// AssetBundleLoader资源加载结果
    /// </summary>
    /// <typeparam name="Asset">资源类型</typeparam>
    public struct BundleAssetResult<Asset>
        where Asset : UnityEngine.Object
    {
        /// <summary>
        /// 资源对象
        /// </summary>
        public Asset asset;
        /// <summary>
        /// asset bundle是否有效，如果请求中设置autoUnloadBundle为false的话，那么assetBundle属性
        /// 就是有效的
        /// </summary>
        public bool bundleValid;
        /// <summary>
        /// asseBundle对象，当bundleValid为true的时候可以访问
        /// </summary>
        public AssetBundle assetBundle;
    }
}