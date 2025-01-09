using UnityEngine;

namespace EasyCoroutine
{
    public class WaitBundleAssetMultiple<T> : WorkerDecorator<BundleAssetMultipleResult<T>>, IInstructionCompletable, IPoolable
        where T : UnityEngine.Object
    {
        public readonly static FactoryWithPool<WaitBundleAssetMultiple<T>> factory =
            new FactoryWithPool<WaitBundleAssetMultiple<T>>(() => new WaitBundleAssetMultiple<T>());

        static WaitBundleAssetMultiple() { }

        private IInstructionWaitable mWaitable;
        private BundleAssetMultipleLoader mLoader;
        private bool mIsPool;
        private AssetBundle mCurBundle;

        public WaitBundleAssetMultiple<T> SetLoader(BundleAssetMultipleLoader loader)
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
            {
                worker.Resolve(default);
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
                worker.Resolve(default);
            worker.Reset();
        }

        public void OnReuse()
        {

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

        public WaitBundleAssetMultiple() { }

        private void HandleCreateRequest(AssetBundleCreateRequest request)
        {
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
            worker.Resolve(new BundleAssetMultipleResult<T>
            {
                assets = req.allAssets as T[],
                bundleValid = !mLoader.autoUnloadBundle,
                assetBundle = bundle
            });
        }
    }

    /// <summary>
    /// 复合AssetBundle资源请求
    /// </summary>
    public struct BundleAssetMultipleLoader
    {
        /// <summary>
        /// bundle的Url
        /// </summary>
        public string path;
        /// <summary>
        /// 是否自动释放asset bundle
        /// </summary>
        public bool autoUnloadBundle;
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