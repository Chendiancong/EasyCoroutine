using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AsyncWork.Core
{
    public delegate void WorkerResolve();
    public delegate void WorkerResolve<T>(T result);
    public delegate void WorkerReject(int errCode);

    public interface ICustomAwaiter : INotifyCompletion
    {
        bool IsCompleted { get; }
        void GetResult();
    }

    public interface ICustomAwaiter<T> : INotifyCompletion
    {
        bool IsCompleted { get; }
        T GetResult();
    }

    public interface IAwaitable
    {
        ICustomAwaiter GetAwaiter();
    }

    public interface IAwaitable<TResult>
    {
        ICustomAwaiter<TResult> GetAwaiter();
    }

    public interface IScheduleable
    {
        void Update();
    }

    public interface IWorkerCallback { }

    public interface IInstructionWaitable
    {
        void WaitFor(YieldInstruction instruction);
        void WaitFor(CustomYieldInstruction instruction);
    }

    public enum WorkerStatus
    {
        Waiting,
        Running,
        Succeed,
        Failed,
    }
}