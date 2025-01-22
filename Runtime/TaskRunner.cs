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
            // RunTask().ContinueWith(_ => Debug.Log("RunTask ok!"));
            WaitInstruction.Create(new WaitForSeconds(1f))
                .Then(() => {
                    Debug.Log("Wait 1s");
                    return WaitInstruction.Create(new WaitForSeconds(2f));
                })
                .Then(obj => {
                    Debug.Log("wait 2s");
                    Debug.Log(obj);
                    var loader = new BundleAssetLoader<GameObject>
                    {
                        path = $"{Application.streamingAssetsPath}{Path.DirectorySeparatorChar}sphere.prefab.asset",
                        assetName = "Sphere",
                        autoUnloadBundle = true
                    };
                    return WaitBundleAsset<GameObject>.Create(loader);
                })
                .Then(obj => {
                    Debug.Log("task ok");
                    Debug.Log(obj);
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
