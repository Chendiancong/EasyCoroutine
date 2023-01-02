using System;
using System.Reflection;

namespace DCFramework
{
    public class SingleTon<T>
        where T : class, new()
    {
        public static readonly T instance = new T();

        /// <summary>
        /// ��̬���캯����ֹbeforeInit��instance�ڱ����ʵ�ʱ��Ž��г�ʼ��
        /// </summary>
        static SingleTon() { }

        protected SingleTon() { }
    }
}