using System;
using UnityEngine;

namespace AsyncWork
{
    public static class CoroutineExtension
    {
        private static AwaiterConstructInfo mRunInFixedUpdate =
            new AwaiterConstructInfo() { execMode = kAwaiterExecMode.UnityFixedUpdate };
        private static AwaiterConstructInfo mRunInUpdate =
            new AwaiterConstructInfo() { execMode = kAwaiterExecMode.UnityUpdate };
        private static AwaiterConstructInfo mRunInLateUpdate =
            new AwaiterConstructInfo() { execMode = kAwaiterExecMode.UnityLateUpdate };

        public static DelayAwaiter GetAwaiter(this WaitForSeconds _)
        {
            return new DelayAwaiter(ref mRunInFixedUpdate, TimeSpan.FromSeconds(1));
        }

        public class DelayAwaiter : CustomAwaiterNoResult, IAwaiterScheduleable
        {
            public float mTargetTime;

            public kAwaiterExecMode ExecMode { get; set; }

            public DelayAwaiter(ref AwaiterConstructInfo info, TimeSpan time) : base(ref info)
            {
                mTargetTime = Time.time + (float)time.TotalSeconds;
            }

            public override bool IsDone
            {
                get => Time.time >= mTargetTime;
            }

            public override void BeforeContinue() { }
        }

        public class YieldAwaiter : CustomAwaiterNoResult
        {
            public YieldAwaiter(ref AwaiterConstructInfo info, YieldInstruction yieldInstruction) : base(ref info)
            {
            }

            public override void BeforeContinue()
            {
                throw new NotImplementedException();
            }
        }
    }
}