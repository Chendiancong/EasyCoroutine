using UnityEngine;

namespace EasyCoroutine
{
    [FactoryableClass]
    public class WaitBundleAssetMultiple<T> : WorkerDecorator<BundleAssetMultipleResult<T>>, IInstructionCompletable
        where T : UnityEngine.Object
    {
        public readonly static FactoryWithPool<WaitBundleAssetMultiple<T>> factory =
            new FactoryWithPool<WaitBundleAssetMultiple<T>>(() => new WaitBundleAssetMultiple<T>());

        static WaitBundleAssetMultiple() { }

        private IInstructionWaitable mWaitable;
        private BundleAssetMultipleLoader<T> mLoader;
        private AssetBundle mCurBundle;

        public static WaitBundleAssetMultiple<T> Create(BundleAssetMultipleLoader<T> loader)
        {
            AssetBundleCreateRequest abr = AssetBundle.LoadFromFileAsync(loader.path);
            return FactoryMgr.PoolCreate<WaitBundleAssetMultiple<T>>()
                .SetLoader(loader)
                .Start(abr, WorkerRunnerBehaviour.Instance);
        }

        public WaitBundleAssetMultiple() { }

        public WaitBundleAssetMultiple<T> SetLoader(BundleAssetMultipleLoader<T> loader)
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

        public WaitBundleAssetMultiple<T> Start(AssetBundleCreateRequest createReq, IInstructionWaitable waitable)
        {
            if (worker.Status == WorkerStatus.Waiting)
            {
                worker.Start();
                mWaitable = waitable;
                waitable.WaitFor(createReq, this);
            }
            return this;
        }

        public WaitBundleAssetMultiple<T> Start(AssetBundleRequest req, IInstructionWaitable waitable)
        {
            if (worker.Status == WorkerStatus.Waiting)
            {
                worker.Start();
                mWaitable = waitable;
                waitable.WaitFor(req, this);
            }
            return this;
        }

        public WaitBundleAssetMultiple(AssetBundleCreateRequest createReq, IInstructionWaitable waitable) : base()
        {
            Start(createReq, waitable);
        }

        public WaitBundleAssetMultiple(AssetBundleRequest req, IInstructionWaitable waitable) : base()
        {
            Start(req, waitable);
        }

        private void HandleCreateRequest(AssetBundleCreateRequest request)
        {
            if (!request.isDone) {
                InternalReject(new WorkerException($"Load {mLoader.path} failed"));
                return;
            }
            AssetBundle bundle = request.assetBundle;
            mCurBundle = bundle;
            AssetBundleRequest req = bundle.LoadAllAssetsAsync();
            mWaitable.WaitFor(req, this);
        }

        private void HandleRequest(AssetBundleRequest req)
        {
            if (mLoader.autoUnloadBundle)
                mCurBundle?.Unload(false);
            AssetBundle bundle = mCurBundle;
            mCurBundle = null;
            InternalResolve(new BundleAssetMultipleResult<T>
            {
                assets = req.allAssets as T[],
                bundleValid = !mLoader.autoUnloadBundle,
                assetBundle = bundle
            });
        }

        private void InternalResolve(BundleAssetMultipleResult<T> result)
        {
            worker.Resolve(result);
            DisposeMe(this);
        }

        private void InternalReject(WorkerException e)
        {
            try { worker.Reject(e); }
            catch { throw; }
            finally { DisposeMe(this); }
        }
    }

    /// <summary>
    /// 复合AssetBundle资源请求
    /// </summary>
    public struct BundleAssetMultipleLoader<Asset> : IAwaitable<BundleAssetMultipleResult<Asset>>
        where Asset : UnityEngine.Object
    {
        /// <summary>
        /// bundle的Url
        /// </summary>
        public string path;
        /// <summary>
        /// 是否自动释放asset bundle
        /// </summary>
        public bool autoUnloadBundle;

        public Worker<BundleAssetMultipleResult<Asset>>.WorkerAwaiter GetAwaiter() =>
            WaitBundleAssetMultiple<Asset>.Create(this).GetAwaiter();

        ICustomAwaiter<BundleAssetMultipleResult<Asset>> IAwaitable<BundleAssetMultipleResult<Asset>>.GetAwaiter() =>
            GetAwaiter();
    }

    /// <summary>
    /// AssetBundleMultipleLoader资源加载结果
    /// </summary>
    /// <typeparam name="Asset">资源类型</typeparam>
    public struct BundleAssetMultipleResult<Asset>
        where Asset : UnityEngine.Object
    {
        /// <summary>
        /// 资源列表
        /// </summary>
        public Asset[] assets;
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