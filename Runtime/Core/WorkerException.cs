using System;

namespace EasyCoroutine{
    public class WorkerException : Exception
    {
        public WorkerException() : base() { }

        public WorkerException(string msg) : base(msg) { }

        public WorkerException(string msg, Exception innerException) : base(msg, innerException) { }

        public static WorkerException FromException(Exception e)
        {
            if (e is WorkerException)
                return e as WorkerException;
            else
                return new WorkerException(e.Message, e);
        }
    }
}