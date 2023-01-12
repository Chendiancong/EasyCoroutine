using UnityEngine;

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
            RunTask();
        }

        public async void RunTask()
        {
            Debug.Log("wait 1s");
            await new WaitForSeconds(1f);
            Debug.Log("wait fixed update");
            await new WaitForFixedUpdate();
            Debug.Log("wait end of frame");
            await new WaitForEndOfFrame();
            var instruction = new WaitForSeconds(1f);
            for (int i = 0; i < 3; ++i)
            {
                Debug.Log("wait 1s");
                await instruction;
            }
            Debug.Log("wait 1s");
            await Awaiter.Wait(instruction);
            //Debug.Log("load asset");
            //string path = Application.streamingAssetsPath + Path.DirectorySeparatorChar + "sphere.prefab.asset";
            //GameObject asset = await Awaiter.Load<GameObject>(path)
            //                            .SetAssetName("Sphere")
            //                            .SetUnloadBundle(false);
            //Debug.Log($"loaded {asset.name}");
            //Instantiate(asset);
            Debug.Log("ok");
        }
    }
}
