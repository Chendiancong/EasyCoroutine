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

    public interface IAwaitable
    {
        ICustomAwaiter GetAwaiter();
    }

    public interface IAwaitable<TResult>
    {
        ICustomAwaiter<TResult> GetAwaiter();
    }

    public interface IThenable
    {
        IThenable Then(Action onFullfilled);
        IThenable<NextResult> Then<NextResult>(Func<NextResult> onFullfilled);
        IThenable Catch(Action<Exception> onReject);
        IThenable<NextResult> Catch<NextResult>(Func<Exception, NextResult> onReject);
    }

    public interface IThenable<Result> : IThenable
    {
        IThenable Then(Action<Result> onFullfilled);
        IThenable<NextResult> Then<NextResult>(Func<Result, NextResult> onFullfilled);
    }

    public interface IWorkerLike
    {
        void Resolve();
        void Reject(Exception e);
        void Reject(string reason);
        void OnFullfilled(Action onFullfilled);
        void OnRejected(Action<Exception> onRejected);
    }

    public interface IWorkerLike<Result>
    {
        void Resolve(IWorkerLike<Result> result);
        void Resolve(Result result);
        void Reject(Exception e);
        void Reject(string reason);
        void OnFullfilled(Action<Result> onFullfilled);
        void OnRejected(Action<Exception> onRejected);
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

    public interface IInvokable<Input>
    {
        void Invoke(Input input);
    }
}