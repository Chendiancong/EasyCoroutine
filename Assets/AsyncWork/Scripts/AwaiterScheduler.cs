using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace AsyncWork
{
    public class AwaiterScheduler : MonoBehaviour
    {
        private List<ScheduleRecord> mFixedUpdateAwaiters;
        private List<ScheduleRecord> mUpdateAwaiters;
        private List<ScheduleRecord> mLateUpdateAwaiters;

        private const int kStartState = 1;

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
            switch (awaiter.ExecMode)
            {
                case AwaiterExecMode.Default:
                    break;
                case AwaiterExecMode.ThreadPool:
                    ThreadPool.QueueUserWorkItem(ThreadJob, awaiter);
                    break;
                case AwaiterExecMode.UnityFixedUpdate:
                    mFixedUpdateAwaiters.Add(new ScheduleRecord() { awaiter = awaiter });
                    break;
                case AwaiterExecMode.UnityUpdate:
                    mUpdateAwaiters.Add(new ScheduleRecord() { awaiter = awaiter });
                    break;
                case AwaiterExecMode.UnityLateUpdate:
                    mLateUpdateAwaiters.Add(new ScheduleRecord() { awaiter = awaiter });
                    break;
                case AwaiterExecMode.Coroutine:
                    StartCoroutine(WaitForInstruction(awaiter));
                    break;
            }
        }

        private IEnumerator WaitForInstruction(IAwaiter awaiter)
        {
            awaiter.Start();
        COROUTINE_START:
            if (awaiter.Instruction != null)
                yield return awaiter.Instruction;
            else if (awaiter.CustomInstruction != null)
                yield return awaiter.CustomInstruction;
            if (!awaiter.IsDone())
                goto COROUTINE_START;
            awaiter.BeforeContinue();
            if (!(awaiter is IAwaiterNoResult))
                awaiter.SetupResult();
            awaiter.Continue();
        }

        private void ThreadJob(object state)
        {
            IAwaiter awaiter = (IAwaiter)state;
            awaiter.Start();
            while (!awaiter.IsDone()) { }
            awaiter.BeforeContinue();
            if (!(awaiter is IAwaiterNoResult))
                awaiter.SetupResult();
            awaiter.Continue();
        }

        private void Awake()
        {
            mFixedUpdateAwaiters = new List<ScheduleRecord>();
            mUpdateAwaiters = new List<ScheduleRecord>();
            mLateUpdateAwaiters = new List<ScheduleRecord>();
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

        private void CheckAwaiters(List<ScheduleRecord> awaiters)
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
                    var record = awaiters[idx];
                    if ((record.state & kStartState) == 0)
                    {
                        record.awaiter.Start();
                        record.state |= kStartState;
                    }
                    IAwaiter awaiter = record.awaiter;
                    if (awaiter.IsDone())
                    {
                        awaiter.BeforeContinue();
                        if (!(awaiter is IAwaiterNoResult))
                            awaiter.SetupResult();
                        awaiter.Continue();
                        awaiters[idx] = new ScheduleRecord();
                        ++doneNum;
                    }
                }
            } catch (Exception e)
            {
                Debug.LogError($"Awaiter Error {e.Message}");
                Debug.LogError(e.StackTrace);
                // 出现异常后直接清除这个awaiter
                awaiters[idx] = new ScheduleRecord();
                ++doneNum;
            }
            if (doneNum > 0)
                awaiters.RemoveAll(CheckIsNull);
        }

        private bool CheckIsNull(ScheduleRecord record) => record.awaiter == null;

        private struct ScheduleRecord
        {
            public IAwaiter awaiter;
            public int state;
        }
    }
}