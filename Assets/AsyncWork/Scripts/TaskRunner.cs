using System.Threading.Tasks;
using UnityEngine;

namespace AsyncWork
{
    public class TaskRunner : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            //RunTask().Wait();
            RunTask();
        }

        private async void RunTask()
        {
            await Task.Delay(1000);
            LogThreadId("RunTask_1");
            await Task.Delay(1000);
            LogThreadId("RunTask_2");
            LogThreadId($"{await RunTask2()}");
        }

        private async Task<string> RunTask2()
        {
            await Task.Delay(1000);
            LogThreadId("RunTask2_1");
            return "abc";
        }

        private void LogThreadId(string msg)
        {
            Debug.Log($"{msg}:{System.Threading.Thread.CurrentThread.ManagedThreadId}");
        }
    }
}
