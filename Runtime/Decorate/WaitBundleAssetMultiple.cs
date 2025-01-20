using UnityEngine;

namespace EasyCoroutine
{
    [FactoryableClass]
    public class WaitBundleAssetMultiple<T> : WorkerDecorator<BundleAssetMultipleResult<T>>, IInstructionCompletable
        where T : UnityEngine.Object
    {
        static WaitBundleAssetMultiple() { }

        private IInstructionWaitable m_waitable;
        private BundleAssetMultipleLoader<T> m_loader;
        private AssetBundle m_curBundle;

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
            m_loader = loader;
            return this;
        }

        public void OnComplete(YieldInstruction instruction)
        {
            if (instruction is AssetBundleCreateRequest)
                HandleCreateRequest(instruction as AssetBundleCreateRequest);
            else if (instruction is AssetBundleRequest)
                HandleRequest(instruction as AssetBundleRequest);
            else
                ResolveMe(this, default);
        }

        public WaitBundleAssetMultiple<T> Start(AssetBundleCreateRequest createReq, IInstructionWaitable waitable)
        {
            if (worker.Status == WorkerStatus.Waiting)
            {
                worker.Start();
                m_waitable = waitable;
                waitable.WaitFor(createReq, this);
            }
            return this;
        }

        public WaitBundleAssetMultiple<T> Start(AssetBundleRequest req, IInstructionWaitable waitable)
        {
            if (worker.Status == WorkerStatus.Waiting)
            {
                worker.Start();
                m_waitable = waitable;
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
                RejectMe(this, new WorkerException($"Load {m_loader.path} failed"));
                return;
            }
            AssetBundle bundle = request.assetBundle;
            m_curBundle = bundle;
            AssetBundleRequest req = bundle.LoadAllAssetsAsync();
            m_waitable.WaitFor(req, this);
        }

        private void HandleRequest(AssetBundleRequest req)
        {
            if (m_loader.autoUnloadBundle)
                m_curBundle?.Unload(false);
            AssetBundle bundle = m_curBundle;
            m_curBundle = null;
            ResolveMe(
                this,
                new BundleAssetMultipleResult<T>
                {
                    assets = req.allAssets as T[],
                    bundleValid = !m_loader.autoUnloadBundle,
                    assetBundle = bundle
                }
            );
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