using System;
using UnityEngine;

namespace AsyncWork
{
    public static class CoroutineExtension
    {
        private static AwaiterConstructInfo mRunInFixedUpdate =
            new AwaiterConstructInfo() { execMode = AwaiterExecMode.UnityFixedUpdate };
        private static AwaiterConstructInfo mRunInUpdate =
            new AwaiterConstructInfo() { execMode = AwaiterExecMode.UnityUpdate };
        private static AwaiterConstructInfo mRunInLateUpdate =
            new AwaiterConstructInfo() { execMode = AwaiterExecMode.UnityLateUpdate };
        private static AwaiterConstructInfo mYieldable =
            new AwaiterConstructInfo() { };

        public static YieldAwaiter GetAwaiter(this WaitForSeconds waitForSeconds)
        {
            mYieldable.instruction = waitForSeconds;
            return new YieldAwaiter(ref mYieldable);
        }

        public static CustomYieldAwaiter GetAwaiter(this WaitForSecondsRealtime waitForSecondsRealtime)
        {
            mYieldable.customInstruction = waitForSecondsRealtime;
            return new CustomYieldAwaiter(ref mYieldable);
        }

        public static YieldAwaiter GetAwaiter(this WaitForEndOfFrame waitForEndOfFrame)
        {
            mYieldable.instruction = waitForEndOfFrame;
            return new YieldAwaiter(ref mYieldable);
        }

        public static YieldAwaiter GetAwaiter(this WaitForFixedUpdate waitForFixedUpdate)
        {
            mYieldable.instruction = waitForFixedUpdate;
            return new YieldAwaiter(ref mYieldable);
        }

        public static CustomYieldAwaiter GetAwaiter(this WaitUntil waitUntil)
        {
            mYieldable.customInstruction = waitUntil;
            return new CustomYieldAwaiter(ref mYieldable);
        }

        public static CustomYieldAwaiter GetAwaiter(this WaitWhile waitWhile)
        {
            mYieldable.customInstruction = waitWhile;
            return new CustomYieldAwaiter(ref mYieldable);
        }

        public class YieldAwaiter : CustomAwaiterNoResult, IAwaiterYieldable
        {
            public YieldInstruction Instruction { get; set; }

            public YieldAwaiter(ref AwaiterConstructInfo info) : base(ref info)
            { }

            public override void Start() { }
        }

        public class CustomYieldAwaiter : CustomAwaiterNoResult, IAwaiterCustomYieldable
        {
            public CustomYieldInstruction CustomInstruction { get; set; }

            public CustomYieldAwaiter(ref AwaiterConstructInfo info) : base(ref info)
            { }

            public override void Start() { }
        }
    }
}