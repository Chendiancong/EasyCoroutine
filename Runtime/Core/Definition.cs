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

    public interface IWorkerThenable
    {
        /// <summary>
        /// worker完成时调用
        /// </summary>
        void Then(Action action);
        /// <summary>
        /// worker抛出异常时调用
        /// </summary>
        void Catch(Action<WorkerException> action);
    }

    public interface IWorkerThenable<TResult> : IWorkerThenable
    {
        /// <summary>
        /// worker完成时调用
        /// </summary>
        void Then(Action<TResult> action);
    }

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