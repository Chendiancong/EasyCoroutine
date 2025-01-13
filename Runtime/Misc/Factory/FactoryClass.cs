using System;
using System.Collections.Generic;
using System.Reflection;

namespace EasyCoroutine
{
    /// <summary>
    /// 工厂类特性，只有具备该特性的类型才能被FactoryMgr创建或者回收
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class FactoryableClassAttribute : Attribute
    {
        private static Dictionary<string, FactoryableClassAttribute> mType2Attrs;

        static FactoryableClassAttribute()
        {
            mType2Attrs = new Dictionary<string, FactoryableClassAttribute>();
        }

        public static FactoryableClassAttribute GetAttrByType(Type type)
        {
            FactoryableClassAttribute attr;
            if (!mType2Attrs.TryGetValue(type.Name, out attr))
            {
                attr = type.GetCustomAttribute<FactoryableClassAttribute>();
                if (attr is not null)
                    mType2Attrs[type.Name] = attr;
            }
            return attr;
        }
    }
}