using System;
using System.Reflection;

namespace EasyCoroutine
{
    public class WorkerDecorator : IAwaitable, IPoolable, IThenable
    {
        protected Worker worker;
        protected Type type;
        protected bool isPoolObj;

        public Worker Worker => worker;

        public WorkerDecorator()
        {
            worker = new Worker();
            type = GetType();
        }

        public Worker.WorkerAwaiter GetAwaiter()
        {
            return worker.GetAwaiter();
        }

        ICustomAwaiter IAwaitable.GetAwaiter() => GetAwaiter();

        public IThenable Then(Action onFullfilled)
        {
            WaitPromise promise = FactoryMgr.PoolCreate<WaitPromise>();
            worker.AddNextJob(new WorkerNext(promise, onFullfilled));
            return promise;
        }

        public IThenable<Output> Then<Output>(Func<Output> onFullfilled)
        {
            var promise = FactoryMgr.PoolCreate<WaitPromise<Output>>();
            worker.AddNextJob(new WorkerNextWithOutput<Output>(promise, onFullfilled));
            return promise;
        }

        public IThenable Catch(Action<Exception> onReject)
        {
            var promise = FactoryMgr.PoolCreate<WaitPromise>();
            worker.AddRejectJob(new WorkerRejecter(promise, onReject));
            return promise;
        }

        public IThenable<NextResult> Catch<NextResult>(Func<Exception, NextResult> onReject)
        {
            var promise = FactoryMgr.PoolCreate<WaitPromise<NextResult>>();
            worker.AddRejectJob(new WorkerRejecter<NextResult>(promise, onReject));
            return promise;
        }

        protected void ResolveMe<Instance>(Instance ins)
            where Instance : WorkerDecorator, new()
        {
            ins.worker.Resolve();
            if (ins.isPoolObj)
                FactoryMgr.Restore(ins);
        }

        protected void RejectMe<Instance>(Instance ins, Exception e)
            where Instance : WorkerDecorator, new()
        {
            ins.worker.Reject(e);
            if (ins.isPoolObj)
                FactoryMgr.Restore(ins);
        }

        protected void DisposeMe<T>(T ins)
            where T : WorkerDecorator, new()
        {
            if (ins.isPoolObj)
                FactoryMgr.Restore(ins);
        }

        void IPoolable.OnCreate()
        {
            isPoolObj = true;
        }

        void IPoolable.OnReuse() { }

        void IPoolable.OnRestore()
        {
            if (worker.Status == WorkerStatus.Running)
                worker.Resolve();
            worker.Reset();
        }
    }

    public class WorkerDecorator<Result> : IAwaitable<Result>, IPoolable, IThenable<Result>
    {
        protected Worker<Result> worker;
        protected Type type;
        protected bool isPoolObj;
        protected Action<Result> workerResolver;
        protected Action<Exception> workerRejecter;

        public Worker<Result> Worker => worker;

        public WorkerDecorator()
        {
            worker = new Worker<Result>((resolver, rejecter) => {
                workerResolver = resolver;
                workerRejecter = rejecter;
            });
            type = GetType();
        }

        public Worker<Result>.WorkerAwaiter GetAwaiter()
        {
            return worker.GetAwaiter();
        }

        public IThenable Then(Action<Result> onFullfilled)
        {
            var promise = FactoryMgr.PoolCreate<WaitPromise>();
            worker.AddNextJob(new WorkerNextWithInput<Result>(promise, onFullfilled));
            return promise;
        }

        public IThenable<NextResult> Then<NextResult>(Func<Result, NextResult> onFullfilled)
        {
            var promise = FactoryMgr.PoolCreate<WaitPromise<NextResult>>();
            worker.AddNextJob(new WorkerNextWithInputOutput<Result, NextResult>(promise, onFullfilled));
            return promise;
        }

        public IThenable Then(Action onFullfilled)
        {
            var promise = FactoryMgr.PoolCreate<WaitPromise>();
            worker.AddNextJob(new WorkerNextWithInput<Result>(promise, _ => onFullfilled()));
            return promise;
        }

        public IThenable<NextResult> Then<NextResult>(Func<NextResult> onFullfilled)
        {
            var promise = FactoryMgr.PoolCreate<WaitPromise<NextResult>>();
            worker.AddNextJob(new WorkerNextWithInputOutput<Result, NextResult>(promise, _ => onFullfilled()));
            return promise;
        }

        public IThenable Catch(Action<Exception> onReject)
        {
            var promise = FactoryMgr.PoolCreate<WaitPromise>();
            worker.AddRejectJob(new WorkerRejecter(promise, onReject));
            return promise;
        }

        public IThenable<NextResult> Catch<NextResult>(Func<Exception, NextResult> onReject)
        {
            var promise = FactoryMgr.PoolCreate<WaitPromise<NextResult>>();
            worker.AddRejectJob(new WorkerRejecter<NextResult>(promise, onReject));
            return promise;
        }

        ICustomAwaiter<Result> IAwaitable<Result>.GetAwaiter() => GetAwaiter();

        protected void ResolveMe<Ins>(Ins ins, Result result)
            where Ins : WorkerDecorator<Result>, new()
        {
            ins.worker.Resolve(result);
            if (ins.isPoolObj)
                FactoryMgr.Restore(ins);
        }

        protected void RejectMe<Ins>(Ins ins, Exception e)
            where Ins : WorkerDecorator<Result>, new()
        {
            try { ins.worker.Reject(e); }
            catch { throw; }
            finally
            {
                if (ins.isPoolObj)
                    FactoryMgr.Restore(ins);
            }
        }

        protected void DisposeMe<T>(T ins)
            where T: WorkerDecorator<Result>, new()
        {
            if (ins.isPoolObj)
                FactoryMgr.Restore(ins);
        }

        void IPoolable.OnCreate()
        {
            isPoolObj = true;
        }

        void IPoolable.OnReuse()
        {
        }

        void IPoolable.OnRestore()
        {
            if (worker.Status == WorkerStatus.Running)
                worker.Resolve(default);
            worker.Reset();
        }
    }
}