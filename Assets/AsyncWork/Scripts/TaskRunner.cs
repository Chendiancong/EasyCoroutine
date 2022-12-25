using System.IO;
using System.Collections;
using UnityEngine;
using System.Threading.Tasks;

namespace AsyncWork
{

    public class TaskRunner : MonoBehaviour
    {
        private YieldInstruction mInstruction;
        private IEnumerator mCoroutine;
        // Start is called before the first frame update
        void Start()
        {
            mCoroutine = RunTask2();
        }

        private int mTimes = 0;
        public void Doit()
        {
            //if (mTimes == 0)
            //{
            //    mInstruction = new WaitForSeconds(1f);
            //    ++mTimes;
            //    StartCoroutine(mCoroutine);
            //}
            //else
            //{
            //    mInstruction = new WaitForSeconds(2f);
            //    ++mTimes;
            //    StartCoroutine(mCoroutine);
            //}
            RunTask();
        }

        public async void RunTask()
        {
            //Debug.Log("wait 1s...");
            //await new WaitForSeconds(1f);
            //Debug.Log("wait fixed update");
            //await new WaitForFixedUpdate();
            //Debug.Log("wati end of frame");
            //await new WaitForEndOfFrame();
            //var ab = await AssetBundle.LoadFromFileAsync(Application.streamingAssetsPath + Path.DirectorySeparatorChar + "sphere.prefab.asset");
            //Debug.Log(ab);
            //var objs = await ab.LoadAssetAsync("Sphere").WrapMultiple<GameObject>();
            //foreach (var obj in objs)
            //    Instantiate(obj);

            //Debug.Log("wait 1s");
            //await Core.Awaiter.Wait(new WaitForSeconds(1f));
            //Debug.Log("load asset");
            //var asset = await Core.Awaiter.Load<GameObject>(Application.streamingAssetsPath + Path.DirectorySeparatorChar + "sphere.prefab.asset").SetAssetName("Sphere");
            //Debug.Log($"loaded {asset.name}");
            //Instantiate(asset);
            //Debug.Log("ok");

            var ins = new WaitForSeconds(1f);
            for (int i = 0; i < 5; ++i)
            {
                Debug.Log("Wait 1s...");
                await ins;
                //await Core.Awaiter.Wait(ins);
            }
            Debug.Log("ok");
        }

        public IEnumerator RunTask2()
        {
            while (true)
            {
                Debug.Log("RunTask2 start");
                if (mInstruction != null)
                {
                    yield return mInstruction;
                }
                Debug.Log("RunTask2 ok");
                StopCoroutine(mCoroutine);
                Debug.Log("RunTask2 Stopped");
                yield return 0;
            }
        }
    }
}
