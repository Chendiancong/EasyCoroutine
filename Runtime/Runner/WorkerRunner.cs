using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyCoroutine
{
    public class WorkerRunner : IInstructionWaitable
    {
        private int mId = 0;
        private Stack<int> mFreeIds = new Stack<int>(4);
        private WorkerRunnerItem[] mItems = new WorkerRunnerItem[4];
        private int mLen = 0;
        private MonoBehaviour mHost;

        public MonoBehaviour Host => mHost;

        public WorkerRunner(MonoBehaviour host)
        {
            mHost = host;
        }

        public void WaitFor<T>(T instruction, IInstructionCompletable completable)
            where T : YieldInstruction
        {
            ref WorkerRunnerItem item = ref GetFreeItem();
            item.SetupInstruction(instruction);
            item.SetupCompletable(completable);
            mHost.StartCoroutine(item.target);
        }

        public void WaitFor<T>(T instruction, ICustomInstructionCompletable completable)
            where T : CustomYieldInstruction
        {
            ref WorkerRunnerItem item = ref GetFreeItem();
            item.SetupInstruction(instruction);
            item.SetupCompletable(completable);
            mHost.StartCoroutine(item.target);
        }

        private ref WorkerRunnerItem GetFreeItem()
        {
            int nextId;
            if (mFreeIds.Count > 0)
                nextId = mFreeIds.Pop();
            else
                nextId = mId++;
            if (nextId >= mLen)
            {
                if (mLen == mItems.Length)
                {
                    WorkerRunnerItem[] items = new WorkerRunnerItem[mLen * 2];
                    Array.Copy(mItems, items, mLen);
                    mItems = items;
                }

                WorkerRunnerItem newItem = new WorkerRunnerItem(nextId, CoroutineWaitInstruction(nextId));
                mItems[nextId] = newItem;
                mLen = nextId + 1;
            }

            return ref mItems[nextId];
        }

        private IEnumerator CoroutineWaitInstruction(int id)
        {
            while (true)
            {
                WorkerRunnerItem item = mItems[id];
                switch (item.type)
                {
                    case 1:
                        yield return item.instruction;
                        break;
                    case 2:
                        yield return item.customInstruction;
                        break;
                    default:
                        break;
                }

                switch (item.type)
                {
                    case 1:
                        item.completable.OnComplete(item.instruction);
                        break;
                    case 2:
                        item.customCompletable.OnComplete(item.customInstruction);
                        break;
                    default:
                        mHost.StopCoroutine(item.target);
                        mFreeIds.Push(id);
                        goto WAIT_NEXT;
                }
                mItems[id].Clear();

                WAIT_NEXT:
                yield return 0;
            }
        }

        private struct WorkerRunnerItem
        {
            public IEnumerator target;
            public int id;
            public int type;
            public YieldInstruction instruction;
            public CustomYieldInstruction customInstruction;
            public IInstructionCompletable completable;
            public ICustomInstructionCompletable customCompletable;

            public WorkerRunnerItem(int id, IEnumerator target)
            {
                this.target = target;
                this.id = id;
                type = 0;
                instruction = null;
                customInstruction = null;
                completable = null;
                customCompletable = null;
            }

            public void SetupInstruction(YieldInstruction instruction)
            {
                this.instruction = instruction;
                customInstruction = null;
                type = 1;
            }

            public void SetupInstruction(CustomYieldInstruction instruction)
            {
                this.instruction = null;
                customInstruction = instruction;
                type = 2;
            }

            public void SetupCompletable(IInstructionCompletable completable)
            {
                this.completable = completable;
            }

            public void SetupCompletable(ICustomInstructionCompletable completable)
            {
                customCompletable = completable;
            }

            public void Clear()
            {
                type = 0;
                instruction = null;
                customInstruction = null;
                completable = null;
                customCompletable = null;
            }
        }
    }
}