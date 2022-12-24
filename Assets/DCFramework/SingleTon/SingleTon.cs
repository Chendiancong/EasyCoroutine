namespace DCFramework.SingleTon
{
    public class SingleTon<T>
        where T : new()
    {
        public static readonly T Instance = new T();

        /// <summary>
        /// ��̬���캯����ֹbeforeInit��instance�ڱ����ʵ�ʱ��Ž��г�ʼ��
        /// </summary>
        static SingleTon() { }

        protected SingleTon() { }
    }
}