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

        public static YieldAwaiterNoResult GetAwaiter(this WaitForSeconds waitForSeconds)
        {
            mYieldable.instruction = waitForSeconds;
            return new YieldAwaiterNoResult(ref mYieldable);
        }

        public static CustomYieldAwaiterNoResult GetAwaiter(this WaitForSecondsRealtime waitForSecondsRealtime)
        {
            mYieldable.customInstruction = waitForSecondsRealtime;
            return new CustomYieldAwaiterNoResult(ref mYieldable);
        }

        public static YieldAwaiterNoResult GetAwaiter(this WaitForEndOfFrame waitForEndOfFrame)
        {
            mYieldable.instruction = waitForEndOfFrame;
            return new YieldAwaiterNoResult(ref mYieldable);
        }

        public static YieldAwaiterNoResult GetAwaiter(this WaitForFixedUpdate waitForFixedUpdate)
        {
            mYieldable.instruction = waitForFixedUpdate;
            return new YieldAwaiterNoResult(ref mYieldable);
        }

        public static CustomYieldAwaiterNoResult GetAwaiter(this WaitUntil waitUntil)
        {
            mYieldable.customInstruction = waitUntil;
            return new CustomYieldAwaiterNoResult(ref mYieldable);
        }

        public static CustomYieldAwaiterNoResult GetAwaiter(this WaitWhile waitWhile)
        {
            mYieldable.customInstruction = waitWhile;
            return new CustomYieldAwaiterNoResult(ref mYieldable);
        }

        public class YieldAwaiterNoResult : CustomAwaiterNoResult, IAwaiterYieldable
        {
            public YieldAwaiterNoResult(ref AwaiterConstructInfo info) : base(ref info)
            { }

            public override void Start() { }
        }

        public class CustomYieldAwaiterNoResult : CustomAwaiterNoResult, IAwaiterCustomYieldable
        {
            public CustomYieldAwaiterNoResult(ref AwaiterConstructInfo info) : base(ref info)
            { }

            public override void Start() { }
        }
    }
}