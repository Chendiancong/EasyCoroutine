using System;
using UnityEngine;
using AsyncWork.Core;
using DCFramework;

namespace AsyncWork
{
    public class WaitInstruction: WorkerDecorator, IInstructionCompletable, ICustomInstructionCompletable, IPoolable
    {
        public readonly static FactoryWithPool<WaitInstruction> factory =
            new FactoryWithPool<WaitInstruction>(() => new WaitInstruction());

        static WaitInstruction() { }

        private bool mIsPool;

        public void OnComplete(YieldInstruction instruction)
        {
            worker.Resolve();
            if (mIsPool)
                factory.Restore(this);
        }

        public void OnComplete(CustomYieldInstruction instruction)
        {
            worker.Resolve();
            if (mIsPool)
                factory.Restore(this);
        }

        public void OnCreate()
        {
            mIsPool = true;
        }

        public void OnRestore()
        {
            if (worker.Status == WorkerStatus.Running)
                worker.Resolve();
            worker.Reset();
        }

        public void OnReuse()
        {

        }

        public WaitInstruction Start(YieldInstruction instruction, IInstructionWaitable runner)
        {
            if (worker.Status == WorkerStatus.Waiting)
            {
                worker.Start();
                runner.WaitFor(instruction, this);
            }
            return this;
        }

        public WaitInstruction Start(CustomYieldInstruction instruction, IInstructionWaitable runner)
        {
            if (worker.Status == WorkerStatus.Waiting)
            {
                worker.Start();
                runner.WaitFor(instruction, this);
            }
            return this;
        }

        public WaitInstruction(YieldInstruction instruction, IInstructionWaitable runner) : base()
        {
            Start(instruction, runner);
        }

        public WaitInstruction(CustomYieldInstruction instruction, IInstructionWaitable runner) : base()
        {
            Start(instruction, runner);
        }

        public WaitInstruction() : base() { }
    }
}