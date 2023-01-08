using System;
using System.Collections.Generic;

namespace DCFramework
{
    public interface IPoolable
    {
        void OnCreate();
        void OnReuse();
        void OnRestore();
    }

    public class Factory<T>
        where T : class, new()
    {
        protected Func<T> builder;

        public Factory(Func<T> builder)
        {
            this.builder = builder;
        }

        public virtual T Create()
        {
            return builder();
        }
    }

    public class FactoryWithPool<T> : Factory<T>
        where T : class, IPoolable, new()
    {
        private Dictionary<int, T> mPoolObjs;
        private Stack<int> mFreeKeys;

        public FactoryWithPool(Func<T> builder, int capacity = 4, bool autoFill = false) : base(builder)
        {
            mPoolObjs = new Dictionary<int, T>(capacity);
            mFreeKeys = new Stack<int>(capacity);

            if (autoFill)
            {
                for (int i = 0; i < capacity; ++i)
                {
                    T obj = builder();
                    obj.OnCreate();
                    int key = obj.GetHashCode();
                    mPoolObjs[key] = obj;
                    mFreeKeys.Push(key);
                }
            }
        }

        public override T Create()
        {
            T result;
            if (mFreeKeys.Count > 0)
            {
                int nextKey = mFreeKeys.Pop();
                result = mPoolObjs[nextKey];
                result.OnReuse();
            }
            else
            {
                result = base.Create();
                result.OnCreate();
                mPoolObjs[result.GetHashCode()] = result;
            }
            return result;
        }

        public void Restore(T obj)
        {
            obj.OnRestore();
            mFreeKeys.Push(obj.GetHashCode());
        }
    }
}