using UnityEngine;
using System.IO;
using System.Threading.Tasks;
using System;

namespace EasyCoroutine
{

    public class TaskRunner : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
        }

        public void Doit()
        {
            // RunTask().ContinueWith(_ => Debug.Log("RunTask ok!")).ContinueWith(_ => 5).ContinueWith(task => Debug.Log($"Task Result is {task.Result}"));

            WaitInstruction.Create(new WaitForSeconds(1f))
                .Then(() => {
                    Debug.Log("wait 1s");
                    return WaitInstruction.Create(new WaitForSeconds(2f));
                })
                .Then(() => {
                    Debug.Log("wait 2s");
                    return WaitInstruction.Create(new WaitForEndOfFrame());
                })
                .Then(() => {
                    Debug.Log("wait one frame");
                    var loader = new BundleAssetLoader<GameObject>
                    {
                        path = Path.Combine(Application.streamingAssetsPath, "sphere.prefab.asset"),
                        assetName = "Sphere",
                        autoUnloadBundle = true
                    };
                    return WaitBundleAsset<GameObject>.Create(loader);
                })
                .Then(worker => {
                    Debug.Log($"loaded {worker.GetResult()}");
                    var asset = worker.GetResult().asset;
                    Instantiate(asset);
                })
                .Then(() => {
                    Debug.Log("worker is finished");
                });
        }

        public async Task RunTask()
        {
            Debug.Log("wait 1s");
            await new WaitForSeconds(1f);
            Debug.Log("wait fixed update");
            await new WaitForFixedUpdate();
            Debug.Log("wait end of frame");
            await new WaitForEndOfFrame();
            var loader = new BundleAssetLoader<GameObject>
            {
                path = $"{Application.streamingAssetsPath}{Path.DirectorySeparatorChar}sphere.prefab.asset",
                assetName = "Sphere",
                autoUnloadBundle = true
            };
            try {
                var result = await loader;
                Debug.Log($"loaded {result.asset.name}");
                Instantiate(result.asset);
            }
            catch (Exception e)
            {
                Debug.Log("Exception");
                Debug.Log(e);
                throw;
            }
        }
    }
}
