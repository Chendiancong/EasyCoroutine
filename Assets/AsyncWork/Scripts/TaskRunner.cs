using System.IO;
using UnityEngine;

namespace AsyncWork
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
            Debug.Log("wait 1s...");
            await new WaitForSeconds(1f);
            Debug.Log("wait fixed update");
            await new WaitForFixedUpdate();
            Debug.Log("wati end of frame");
            await new WaitForEndOfFrame();
            var ab = await AssetBundle.LoadFromFileAsync(Application.streamingAssetsPath + Path.DirectorySeparatorChar + "sphere.prefab.asset");
            Debug.Log(ab);
            var objs = await ab.LoadAssetAsync("Sphere").WrapMultiple<GameObject>();
            foreach (var obj in objs)
                Instantiate(obj);
        }

    }
}
