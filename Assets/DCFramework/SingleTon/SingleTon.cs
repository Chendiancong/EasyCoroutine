using System;
using System.Reflection;

namespace DCFramework
{
    public class SingleTon<T>
        where T : class, new()
    {
        public static readonly T instance = new T();

        /// <summary>
        /// 静态构造函数防止beforeInit，instance在被访问的时候才进行初始化
        /// </summary>
        static SingleTon() { }

        protected SingleTon() { }
    }
}