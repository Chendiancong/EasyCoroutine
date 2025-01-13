using System;

namespace EasyCoroutine{
    public class WorkerException : Exception
    {
        public WorkerException() : base() { }

        public WorkerException(string msg) : base(msg) { }
    }
}