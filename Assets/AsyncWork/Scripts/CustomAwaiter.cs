using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AsyncWork
{
    #region Interface
    public interface IAwaiter : INotifyCompletion
    {
        /// <summary>
        /// 异步对象的调度类型
        /// </summary>
        kAwaiterScheduleType ScheduleType { get; }
        /// <summary>
        /// 是否为同步执行，这是await语句会动态调用的属性
        /// 如果为true，则为直接执行
        /// 如果为false，则调用OnCompleted，并通过它的参数，亦即一个委托继续执行异步函数
        /// </summary>
        bool IsCompleted { get; }
        /// <summary>
        /// 异步任务是否已完成
        /// </summary>
        bool IsDone { get; }
        /// <summary>
        /// 当异步任务完成的时候
        /// </summary>
        void BeforeContinue();
        /// <summary>
        /// 继续进行异步函数
        /// </summary>
        void Continue();
        /// <summary>
        /// 设置result
        /// </summary>
        void SetupResult();
    }

    public interface IAwaiterScheduleable
    {
        /// <summary>
        /// 执行模式，如果该属性默认值不为Default
        /// 就采用默认值作为最终的模式
        /// 否则就根据构造函数传入参数来决定
        /// </summary>
        kAwaiterExecMode ExecMode { get; set; }
    }

    public interface IAwaiterYieldable
    {
        /// <summary>
        /// 协程yield对象
        /// </summary>
        YieldInstruction Instruction { get; set; }
    }

    public interface IAwaiterCustomYieldable
    {
        /// <summary>
        /// 自定义yield对象
        /// </summary>
        CustomYieldInstruction Instruction { get; set; }
    }

    public interface IAwaiterResult<T>
    {
        /// <summary>
        /// 获取异步任务的结果，这是await语句会动态调用的方法
        /// </summary>
        T GetResult();
    }

    public interface IAwaiterNoResult { }
    #endregion

    public abstract class CustomAwaiter<T> : IAwaiter, IAwaiterResult<T>
    {
        private Action mContinuation;

        public kAwaiterScheduleType ScheduleType { get; protected set; }
        public bool IsCompleted { get; protected set; }
        public virtual bool IsDone { get; set; }

        public void OnCompleted(Action continuation)
        {
            mContinuation = continuation;
            Schedule();
        }

        public abstract void BeforeContinue();

        public void Continue()
        {
            if (mContinuation != null)
                mContinuation();
            mContinuation = null;
        }

        public CustomAwaiter(ref AwaiterConstructInfo info)
        {
            IsCompleted = info.isSync;
            if (this is IAwaiterScheduleable)
            {
                var target = this as IAwaiterScheduleable;
                if (target.ExecMode != kAwaiterExecMode.Default)
                    target.ExecMode = info.execMode;
                ScheduleType = kAwaiterScheduleType.Normal;
            }
            else if (this is IAwaiterYieldable)
            {
                (this as IAwaiterYieldable).Instruction = info.instruction;
                ScheduleType = kAwaiterScheduleType.Coroutine;
            }
            else if (this is IAwaiterCustomYieldable)
            {
                (this as IAwaiterCustomYieldable).Instruction = info.customInstruction;
                ScheduleType = kAwaiterScheduleType.Coroutine;
            }
        }

        public void Schedule()
        {
            AwaiterScheduler.Instance.ScheduleAwaiter(this);
        }
        public abstract T GetResult();

        public abstract void SetupResult();
    }

    public abstract class CustomAwaiterNoResult : CustomAwaiter<int>, IAwaiterNoResult
    {
        public CustomAwaiterNoResult(ref AwaiterConstructInfo info): base(ref info) { }

        public override int GetResult() => 0;

        public override void SetupResult() { }
    }

    /// <summary>
    /// 异步对象的调度类型
    /// </summary>
    public enum kAwaiterScheduleType
    {
        None,
        /// <summary>
        /// 引擎生命周期函数（FixedUpdate，Update，LateUpdate）中调度
        /// 或者在线程池中调度
        /// </summary>
        Normal,
        /// <summary>
        /// 在协程中调度
        /// </summary>
        Coroutine,
    }

    /// <summary>
    /// 异步任务的运行模式
    /// </summary>
    public enum kAwaiterExecMode
    {
        Default,
        /// <summary>
        /// 通过线程池选择线程执行
        /// </summary>
        ThreadPool,
        /// <summary>
        /// 在FixedUpdate中执行
        /// </summary>
        UnityFixedUpdate,
        /// <summary>
        /// 在Update中执行
        /// </summary>
        UnityUpdate,
        /// <summary>
        /// 在LateUpdate中执行
        /// </summary>
        UnityLateUpdate,
    }

    public struct AwaiterConstructInfo
    {
        /// <summary>
        /// 是否立即执行
        /// </summary>
        public bool isSync;
        /// <summary>
        /// 异步任务的运行模式，仅当isSync == true且调度类型为kAwaiterScheduleType.Normal时有用
        /// 默认为UnityUpdate
        /// 如果该属性默认值不为Default
        /// 就采用默认值作为最终的模式
        /// 否则就根据构造函数传入参数来决定
        /// </summary>
        public kAwaiterExecMode execMode;
        /// <summary>
        /// 协程的yield对象
        /// </summary>
        public YieldInstruction instruction;
        /// <summary>
        /// 自定义协程yield对象
        /// </summary>
        public CustomYieldInstruction customInstruction;
    }
}
