using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AsyncWork.Core
{
    public class WorkerRunner : IInstructionWaitable
    {
        private int mCoId = 0;
        private List<int> mFreeCoIds = new List<int>(8);
        private List<IEnumerator> mCorotuines = new List<IEnumerator>();

        public void WaitFor(YieldInstruction instruction)
        {
            if (mFreeCoIds.Count > 0)
            {

            }
        }

        public void WaitFor(CustomYieldInstruction instruction)
        {

        }
    }
}