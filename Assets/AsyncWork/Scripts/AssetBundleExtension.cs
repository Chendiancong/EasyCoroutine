using System;
using UnityEngine;

namespace AsyncWork
{
    public static class AssetBundleExtension
    {
        public class AssetBundleCreateRequestAwaiter : CustomAwaiter<AssetBundle>, IAwaiterScheduleable
        {
            private AssetBundleCreateRequest mRequest;
            public AssetBundleCreateRequestAwaiter(ref AwaiterConstructInfo info): base(ref info)
            {
                try
                {
                    mRequest = info.instruction as AssetBundleCreateRequest;
                }
                catch (Exception e)
                {
                    Debug.LogError($"{e.Message}");
                    Debug.LogError(e.StackTrace);
                }
                finally
                {
                    mRequest = null;
                }
            }

            public override bool IsDone()
            {
                return mRequest != null ?
                    mRequest.isDone :
                    true;
            }

            public override void Start() { }

            public override void SetupResult() { }

            public override AssetBundle GetResult()
            {
                return mRequest != null ?
                    mRequest.assetBundle :
                    null;
            }
        }

        public class AssetBundleRequestAwaiter<T> : CustomAwaiter<T>, IAwaiterScheduleable
            where T : UnityEngine.Object
        {
            private AssetBundleRequest mRequest;

            public AssetBundleRequestAwaiter(ref AwaiterConstructInfo info): base(ref info)
            {
                try
                {
                    mRequest = info.instruction as AssetBundleRequest;
                }
                catch (Exception e)
                {
                    Debug.LogError($"{e.Message}");
                    Debug.LogError(e.StackTrace);
                }
                finally
                {
                    mRequest = null;
                }
            }

            public override bool IsDone()
            {
                return mRequest != null ?
                    mRequest.isDone :
                    true;
            }

            public override void Start() { }

            public override void SetupResult() { }

            public override T GetResult()
            {
                return mRequest != null ?
                    (T)mRequest.asset :
                    default(T);

            }
        }
    }
}