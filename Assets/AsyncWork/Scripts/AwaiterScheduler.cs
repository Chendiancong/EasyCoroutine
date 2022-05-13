using System;
using System.Collections.Generic;
using UnityEngine;

namespace AsyncWork
{
    public class AwaiterScheduler : MonoBehaviour
    {
        private List<IAwaiter> mFixedUpdateAwaiters;
        private List<IAwaiter> mUpdateAwaiters;
        private List<IAwaiter> mLateUpdateAwaiters;

        private static AwaiterScheduler mInstance;
        public static AwaiterScheduler Instance
        {
            get
            {
                if (mInstance == null)
                {
                    GameObject go = new GameObject("AwaiterScheduler");
                    DontDestroyOnLoad(go);
                    mInstance = go.AddComponent<AwaiterScheduler>();
                }
                return mInstance;
            }
        }

        public void ScheduleAwaiter(IAwaiter awaiter)
        {
            switch (awaiter.ScheduleType)
            {
                case kAwaiterScheduleType.Normal:
                    {
                        switch ((awaiter as IAwaiterScheduleable).ExecMode)
                        {
                            case kAwaiterExecMode.ThreadPool:
                                break;
                            case kAwaiterExecMode.UnityFixedUpdate:
                                mFixedUpdateAwaiters.Add(awaiter);
                                break;
                            case kAwaiterExecMode.Default:
                            case kAwaiterExecMode.UnityUpdate:
                                mUpdateAwaiters.Add(awaiter);
                                break;
                            case kAwaiterExecMode.UnityLateUpdate:
                                mLateUpdateAwaiters.Add(awaiter);
                                break;
                        }
                    }
                    break;
                case kAwaiterScheduleType.Coroutine:
                    break;
            }
        }

        private void Awake()
        {
            mFixedUpdateAwaiters = new List<IAwaiter>();
            mUpdateAwaiters = new List<IAwaiter>();
            mLateUpdateAwaiters = new List<IAwaiter>();
        }

        private void OnDestroy()
        {
            if (mInstance == this)
                mInstance = null;
        }

        private void FixedUpdate() =>
            CheckAwaiters(mFixedUpdateAwaiters);

        private void Update() =>
            CheckAwaiters(mUpdateAwaiters);

        private void LateUpdate() =>
            CheckAwaiters(mLateUpdateAwaiters);

        private void CheckAwaiters(List<IAwaiter> awaiters)
        {
            int doneNum = 0;
            int idx = 0;
            int len = awaiters.Count;
            if (len == 0)
                return;
            try
            {
                for (; idx < len; ++idx)
                {
                    IAwaiter awaiter = awaiters[idx];
                    if (awaiter.IsDone)
                    {
                        awaiter.BeforeContinue();
                        if (!(awaiter is IAwaiterNoResult))
                            awaiter.SetupResult();
                        awaiter.Continue();
                        awaiters[idx] = null;
                        ++doneNum;
                    }
                }
            } catch (Exception e)
            {
                Debug.LogError($"Awaiter Error {e.Message}");
                Debug.LogError(e.StackTrace);
                // 出现异常后直接清除这个awaiter
                awaiters[idx] = null;
                ++doneNum;
            }
            if (doneNum > 0)
                awaiters.RemoveAll(CheckIsNull);
        }

        private bool CheckIsNull(IAwaiter awaiter) => awaiter == null;
    }
}