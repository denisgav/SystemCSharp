using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SystemCSharp.Fibers
{
    /// <summary>
    /// Represents a fiber thread (a thread that runs multiple fibers).
    /// </summary>
    internal class FiberSchedulingThread
    {
        private readonly Thread fiberWorker;
        private readonly List<Fiber> fibers;
        private readonly object locker = new object();

        private readonly ManualResetEvent wait = new ManualResetEvent(false);

        /// <summary>
        /// Gets the Fiber Count.
        /// </summary>
        public int FiberCount
        {
            get { return fibers.Count; }
        }

        /// <summary>
        /// Initializes a fiber thread and starts the thread loop.
        /// </summary>
        public FiberSchedulingThread()
        {
            fibers = new List<Fiber>(FiberScheduler.GetFibersPerThreadThreshold());
            fiberWorker = new Thread(ProcessFibers);
            fiberWorker.IsBackground = true;
            fiberWorker.Start();
        }

        public void Schedule(Fiber fiber)
        {
            lock (locker)
            {
                fibers.Add(fiber);
                wait.Set();
            }
        }

        /// <summary>
        /// Processes the list of fibers that are assigned to this thread.
        /// </summary>
        private void ProcessFibers()
        {
            int fiberIndex = 0;

            while (true)
            {
                Monitor.Enter(locker);
                {
                    if (fibers.Count > 0)
                    {
                        if (fiberIndex >= Int16.MaxValue)
                            fiberIndex = 0;

                        //get next fiber.
                        Fiber fiber = fibers[fiberIndex++ % fibers.Count];

                        IEnumerator<FiberStatus> fiberContext = fiber.FiberContext;

                        // do we need to wait a cycle ?
                        if (fiberContext.Current != null && fiberContext.Current.Status == ContextStatus.Wait)
                        {
                            fiberContext.Current.Status = ContextStatus.Switch;
                        }
                        else if (fiberContext.MoveNext() == false)
                        {
                            fibers.Remove(fiber);
                        }

                        // no need to hold the lock at this point.
                        Monitor.Exit(locker);
                    }
                    else
                    {
                        // no need to hold the lock at this point.
                        Monitor.Exit(locker);

                        wait.WaitOne();
                        wait.Reset();
                    }
                }
            }
        }
    }
}
