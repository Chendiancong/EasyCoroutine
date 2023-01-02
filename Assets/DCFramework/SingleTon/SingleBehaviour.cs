using UnityEngine;

namespace DCFramework
{
    public abstract class SingleBehaviour<T> : MonoBehaviour
        where T : SingleBehaviour<T>
    {
        private static T mInstance;
        private readonly static object lockObj = new object();

        public static T Instance
        {
            get
            {
                if (mInstance == null)
                {
                    lock(lockObj)
                    {
                        if (mInstance == null)
                        {
                            GameObject go = new GameObject($"{typeof(T).Name}_singleton");
                            go.SetActive(true);
                            go.AddComponent<T>();
                        }
                    }
                }
                return mInstance;
            }
        }

        protected virtual void Awake()
        {
            mInstance = this as T;
        }

        protected virtual void OnDestroy()
        {
            mInstance = null;
        }
    }
}