using System;
using System.Collections.Generic;

namespace EasyCoroutine
{
    public interface IPoolable
    {
        /// <summary>
        /// 池对象创建时
        /// </summary>
        void OnCreate();
        /// <summary>
        /// 池对象重用时
        /// </summary>
        void OnReuse();
        /// <summary>
        /// 池对象回收时
        /// </summary>
        void OnRestore();
    }

    public interface ICustomFactory
    {
        object Create();
    }

    public interface ICustomFactoryWithPool : ICustomFactory
    {
        void Restore(object obj);
    }

    public class Factory<T> : ICustomFactory
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

        object ICustomFactory.Create() => Create();
    }

    public class FactoryWithPool<T> : Factory<T>, ICustomFactoryWithPool
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

        object ICustomFactory.Create() => Create();

        public void Restore(T obj)
        {
            obj.OnRestore();
            mFreeKeys.Push(obj.GetHashCode());
        }

        void ICustomFactoryWithPool.Restore(object obj) => Restore(obj as T);
    }

    public static class FactoryMgr
    {
        private static Dictionary<Type, ICustomFactory> mFactories;
        private static Dictionary<Type, ICustomFactoryWithPool> mFactoryWithPools;

        static FactoryMgr()
        {
            mFactories = new Dictionary<Type, ICustomFactory>();
            mFactoryWithPools = new Dictionary<Type, ICustomFactoryWithPool>();
        }

        public static ObjClass Create<ObjClass>()
            where ObjClass : class, new()
        {
            Type type = typeof(ObjClass);
            var _ = GetFactoryAttr(type);
            ICustomFactory factory;
            if (!mFactories.TryGetValue(type, out factory))
            {
                factory = new Factory<ObjClass>(() => new ObjClass());
                mFactories[type] = factory;
            }
            return factory.Create() as ObjClass;
        }

        public static ObjClass PoolCreate<ObjClass>()
            where ObjClass : class, IPoolable, new()
        {
            Type type = typeof(ObjClass);
            var _ = GetFactoryAttr(type);
            ICustomFactoryWithPool factory;
            if (!mFactoryWithPools.TryGetValue(type, out factory))
            {
                factory = new FactoryWithPool<ObjClass>(() => new ObjClass());
                mFactoryWithPools[type] = factory;
            }
            return factory.Create() as ObjClass;
        }

        public static bool Restore<ObjClass>(ObjClass ins)
            where ObjClass : class, IPoolable, new()
        {
            Type type = typeof(ObjClass);
            var _ = GetFactoryAttr(type);
            ICustomFactoryWithPool factory;
            if (mFactoryWithPools.TryGetValue(type, out factory))
            {
                factory.Restore(ins);
                return true;
            }
            else
                return false;
        }

        private static FactoryableClassAttribute GetFactoryAttr(Type type)
        {
            var attr = FactoryableClassAttribute.GetAttrByType(type);
            if (attr is null)
                throw new Exception($"{type.Name} missing attribute {typeof(FactoryableClassAttribute).Name}");
            return attr;
        }
    }
}