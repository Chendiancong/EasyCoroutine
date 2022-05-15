using System;
using UnityEngine;

namespace AsyncWork
{
    public static class AssetBundleExtension
    {
        public static AssetBundleCreateRequestAwaiter GetAwaiter(this AssetBundleCreateRequest request)
        {
            AwaiterConstructInfo info = new AwaiterConstructInfo()
            {
                execMode = AwaiterExecMode.Coroutine,
                instruction = request,
            };
            return new AssetBundleCreateRequestAwaiter(ref info);
        }

        public static AssetBundleRequestAwaiter<UnityEngine.Object> GetAwaiter(this AssetBundleRequest request)
        {
            AwaiterConstructInfo info = new AwaiterConstructInfo()
            {
                execMode = AwaiterExecMode.Coroutine,
                instruction = request
            };
            return new AssetBundleRequestAwaiter<UnityEngine.Object>(ref info);
        }

        public static AssetBundleRequestWrapper<T> Wrap<T>(this AssetBundleRequest request)
            where T : UnityEngine.Object
        {
            return new AssetBundleRequestWrapper<T>()
            {
                request = request
            };
        }

        public static AssetBundleRequestAwaiter<T> GetAwaiter<T>(this AssetBundleRequestWrapper<T> wrapper)
            where T : UnityEngine.Object
        {
            AwaiterConstructInfo info = new AwaiterConstructInfo()
            {
                execMode = AwaiterExecMode.Coroutine,
                instruction = wrapper.request
            };
            return new AssetBundleRequestAwaiter<T>(ref info);
        }

        public class AssetBundleCreateRequestAwaiter : CustomAwaiter<AssetBundle>
        {
            private AssetBundleCreateRequest mRequest;
            private AssetBundle mResult;
            public AssetBundleCreateRequestAwaiter(ref AwaiterConstructInfo info): base(ref info)
            {
                try
                {
                    mRequest = info.instruction as AssetBundleCreateRequest;
                }
                catch (Exception e)
                {
                    mRequest = null;
                    Debug.LogError($"{e.Message}");
                    Debug.LogError(e.StackTrace);
                }
            }

            public override bool IsDone()
            {
                return mRequest != null ?
                    mRequest.isDone :
                    true;
            }

            public override void Start() { }

            public override void SetupResult() =>
                mResult = mRequest != null ? mRequest.assetBundle : null;

            public override AssetBundle GetResult() => mResult;
        }

        public class AssetBundleRequestAwaiter<T> : CustomAwaiter<T>
            where T : UnityEngine.Object
        {
            private AssetBundleRequest mRequest;
            private T mResult;

            public AssetBundleRequestAwaiter(ref AwaiterConstructInfo info): base(ref info)
            {
                try
                {
                    mRequest = info.instruction as AssetBundleRequest;
                }
                catch (Exception e)
                {
                    mRequest = null;
                    Debug.LogError($"{e.Message}");
                    Debug.LogError(e.StackTrace);
                }
            }

            public override bool IsDone()
            {
                return mRequest != null ?
                    mRequest.isDone :
                    true;
            }

            public override void Start() { }

            public override void SetupResult() =>
                mResult = mRequest != null ? (T)mRequest.asset : default(T);

            public override T GetResult() => mResult;
        }

        public struct AssetBundleRequestWrapper<T>
            where T : UnityEngine.Object
        {
            public AssetBundleRequest request;
        }
    }
}