namespace DCFramework.SingleTon
{
    public class SingleTon<T>
        where T : new()
    {
        public static readonly T Instance = new T();

        /// <summary>
        /// 静态构造函数防止beforeInit，instance在被访问的时候才进行初始化
        /// </summary>
        static SingleTon() { }

        protected SingleTon() { }
    }
}