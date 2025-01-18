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
        IWorkerThenable Then(Action onFullfilled);
        /// <summary>
        /// worker完成时调用
        /// </summary>
        IWorkerThenable<NextResult> Then<NextResult>(Func<NextResult> onFullfilled);
        /// <summary>
        /// worker抛出异常时调用
        /// </summary>
        IWorkerThenable Catch(Action<WorkerException> catcher);
    }

    public interface IWorkerThenable<Result> : IWorkerThenable
    {
        /// <summary>
        /// worker完成时调用
        /// </summary>
        IWorkerThenable Then(Action<Result> onFullfilled);
        /// <summary>
        /// worker完成时调用
        /// </summary>
        IWorkerThenable<NextResult> Then<NextResult>(Func<Result, NextResult> onFullfilled);
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

    public interface IInvokable
    {
        void Invoke();
    }

    public interface Invokable<Arg>
    {
        void Invoke(Arg arg);
    }
}