using System;
using System.Collections;
using System.Threading.Tasks;
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
            await new WaitForSeconds(1f);
            Debug.Log("Hello1");
            await new WaitForSeconds(1f);
            Debug.Log("Hello2");
            await new WaitForSeconds(1f);
            Debug.Log("Hello3");
        }
    }
}
