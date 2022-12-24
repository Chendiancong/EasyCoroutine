using System;
using UnityEngine;

namespace AsyncWork
{
    public static partial class CoroutineExtension
    {
        private static AwaiterConstructInfo mRunInFixedUpdate =
            new AwaiterConstructInfo() { execMode = AwaiterExecMode.UnityFixedUpdate };
        private static AwaiterConstructInfo mRunInUpdate =
            new AwaiterConstructInfo() { execMode = AwaiterExecMode.UnityUpdate };
        private static AwaiterConstructInfo mRunInLateUpdate =
            new AwaiterConstructInfo() { execMode = AwaiterExecMode.UnityLateUpdate };
        private static AwaiterConstructInfo mYieldable =
            new AwaiterConstructInfo() { execMode = AwaiterExecMode.Coroutine };

        public static YieldAwaiterNoResult GetAwaiter(this WaitForSeconds waitForSeconds)
        {
            mYieldable.instruction = waitForSeconds;
            mYieldable.customInstruction = null;
            return new YieldAwaiterNoResult(ref mYieldable);
        }

        public static CustomYieldAwaiterNoResult GetAwaiter(this WaitForSecondsRealtime waitForSecondsRealtime)
        {
            mYieldable.instruction = null;
            mYieldable.customInstruction = waitForSecondsRealtime;
            return new CustomYieldAwaiterNoResult(ref mYieldable);
        }

        public static YieldAwaiterNoResult GetAwaiter(this WaitForEndOfFrame waitForEndOfFrame)
        {
            mYieldable.instruction = waitForEndOfFrame;
            mYieldable.customInstruction = null;
            return new YieldAwaiterNoResult(ref mYieldable);
        }

        public static YieldAwaiterNoResult GetAwaiter(this WaitForFixedUpdate waitForFixedUpdate)
        {
            mYieldable.instruction = waitForFixedUpdate;
            mYieldable.customInstruction = null;
            return new YieldAwaiterNoResult(ref mYieldable);
        }

        public static CustomYieldAwaiterNoResult GetAwaiter(this WaitUntil waitUntil)
        {
            mYieldable.instruction = null;
            mYieldable.customInstruction = waitUntil;
            return new CustomYieldAwaiterNoResult(ref mYieldable);
        }

        public static CustomYieldAwaiterNoResult GetAwaiter(this WaitWhile waitWhile)
        {
            mYieldable.instruction = null;
            mYieldable.customInstruction = waitWhile;
            return new CustomYieldAwaiterNoResult(ref mYieldable);
        }

        public class YieldAwaiterNoResult : CustomAwaiterNoResult
        {
            public YieldAwaiterNoResult(ref AwaiterConstructInfo info) : base(ref info)
            { }

            public override void Start() { }
        }

        public class CustomYieldAwaiterNoResult : CustomAwaiterNoResult
        {
            public CustomYieldAwaiterNoResult(ref AwaiterConstructInfo info) : base(ref info)
            { }

            public override void Start() { }
        }
    }

    public static partial class CoroutineExtension
    {

    }
}