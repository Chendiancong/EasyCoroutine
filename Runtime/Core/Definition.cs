using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace EasyCoroutine
{
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

    public interface IWorkerCallback { }

    public interface IAwaitable
    {
        ICustomAwaiter GetAwaiter();
    }

    public interface IAwaitable<TResult>
    {
        ICustomAwaiter<TResult> GetAwaiter();
    }


    public interface IInstructionCompletable
    {
        void OnComplete(YieldInstruction instruction);
    }

    public interface ICustomInstructionCompletable
    {
        void OnComplete(CustomYieldInstruction instruction);
    }

    public interface IInstructionWaitable
    {
        void WaitFor<T>(T instruction, IInstructionCompletable completable) where T : YieldInstruction;
        void WaitFor<T>(T instruction, ICustomInstructionCompletable completable) where T : CustomYieldInstruction;
    }
}