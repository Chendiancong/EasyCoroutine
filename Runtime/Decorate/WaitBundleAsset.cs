using UnityEngine;

namespace EasyCoroutine
{
    [FactoryableClass]
    public class WaitBundleAsset<T> : WorkerDecorator<BundleAssetResult<T>>, IInstructionCompletable
        where T : UnityEngine.Object
    {
        private IInstructionWaitable m_waitable;
        private BundleAssetLoader<T> m_loader;
        private AssetBundle m_curBundle;

        public static WaitBundleAsset<T> Create(BundleAssetLoader<T> loader)
        {
            AssetBundleCreateRequest abr = AssetBundle.LoadFromFileAsync(loader.path);
            return FactoryMgr.PoolCreate<WaitBundleAsset<T>>()
                .SetLoader(loader)
                .Start(abr, WorkerRunnerBehaviour.Instance);
        }

        public WaitBundleAsset() { }

        public WaitBundleAsset<T> SetLoader(BundleAssetLoader<T> loader)
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
                InternalResolve(default);
        }

        public WaitBundleAsset<T> Start(AssetBundleCreateRequest createReq, IInstructionWaitable waitable)
        {
            if (worker.Status == WorkerStatus.Waiting)
            {
                worker.Start();
                m_waitable = waitable;
                waitable.WaitFor(createReq, this);
            }
            return this;
        }

        public WaitBundleAsset<T> Start(AssetBundleRequest req, IInstructionWaitable waitable)
        {
            if (worker.Status == WorkerStatus.Waiting)
            {
                worker.Start();
                m_waitable = waitable;
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

        private void HandleCreateRequest(AssetBundleCreateRequest request)
        {
            if (!request.isDone) {
                InternalReject(new WorkerException($"Load {m_loader.path} failed"));
                return;
            }
            AssetBundle bundle = request.assetBundle;
            m_curBundle = bundle;
            AssetBundleRequest req;
            if (string.IsNullOrEmpty(m_loader.assetName))
                req = bundle.LoadAllAssetsAsync();
            else
                req = bundle.LoadAssetAsync(m_loader.assetName);
            m_waitable.WaitFor(req, this);
        }

        private void HandleRequest(AssetBundleRequest request)
        {
            T targetAsset;
            if (string.IsNullOrEmpty(m_loader.assetName))
                targetAsset = request.allAssets.Length > 0 ? request.allAssets[0] as T : null;
            else
                targetAsset = request.asset as T;
            if (m_loader.autoUnloadBundle)
                m_curBundle?.Unload(false);
            AssetBundle bundle = m_curBundle;
            m_curBundle = null;

            InternalResolve(new BundleAssetResult<T> {
                asset = targetAsset,
                bundleValid = !m_loader.autoUnloadBundle,
                assetBundle = bundle
            });
        }

        private void InternalResolve(BundleAssetResult<T> result)
        {
            worker.Resolve(result);
            DisposeMe(this);
        }

        private void InternalReject(WorkerException exception)
        {
            try { worker.Reject(exception); }
            catch { throw; }
            finally { DisposeMe(this); }
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
            WaitBundleAsset<Asset>.Create(this).GetAwaiter();

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