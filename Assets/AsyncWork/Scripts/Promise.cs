using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Assets.AsyncWork.Scripts
{
    public interface IAwaiterJobResultHelper
    {
        YieldInstruction Instruction { get; }
        CustomYieldInstruction CustomInstruction { get; }
    }

    public struct CustomAwaiter : INotifyCompletion
    {
        private AwaiterJob mJob;

        public bool IsCompleted => mJob.IsOk;

        public void OnCompleted(Action continuation)
        {
            mJob.continuation = continuation;
            Schedule();
        }

        private void Schedule()
        {

        }
    }

    public struct CustomAwaiter<T> : INotifyCompletion
    {
        private AwaiterJob mJob;

        public bool IsCompleted => mJob.IsOk;

        public T GetResult()
        {
            throw new Exception();
        }

        public void OnCompleted(Action continuation)
        {
            mJob.continuation = continuation;
        }
    }

    public class AwaiterJob : IAwaiterJobResultHelper
    {
        public Action continuation;
        public AwaiterExecMode execMode = AwaiterExecMode.Default;
        public AwaiterJobStatus status = AwaiterJobStatus.None;
        public Action<IAwaiterJobResultHelper> OnSetupResult;

        public YieldInstruction Instruction { get; protected set; }
        public CustomYieldInstruction CustomInstruction { get; protected set; }

        public bool IsRunning => status == AwaiterJobStatus.Running;
        public bool IsOk => status == AwaiterJobStatus.Ok;
        public bool IsDone => status == AwaiterJobStatus.Ok ||
            status == AwaiterJobStatus.Error;

        public void OnComplete()
        {
            if (OnSetupResult != null)
                OnSetupResult(this);
            continuation();
        }

        public void OnError()
        {

        }
    }

    public struct AwaiterCreateMsg
    {
        public AwaiterExecMode execMode;
        public YieldInstruction instruction;
        public CustomYieldInstruction customInstruction;
        public Action<IAwaiterJobResultHelper> OnSetupResult;
    }

    public enum AwaiterExecMode
    {
        Default,
        Thread,
        UnityFixedUpdate,
        UnityUpdate,
        UnityLateUpdate,
        Coroutine,
    }

    public enum AwaiterJobStatus
    {
        None,
        Running,
        Error,
        Ok,
    }
}
