using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemCSharp.Fibers
{
    /// <summary>
    /// A fiber scheduler that schedules fibers and assigns then
    /// to fiber threads.
    /// </summary>
    public static class FiberScheduler
    {
        private static readonly object locker = new object();
        private static FiberSchedulingThread currentFiber;
        private static readonly FiberSchedulingThread[] fibers;
        private static int fiberThreadCnt = 5;

        /// <summary>
        /// Initializes the fiber scheduler, creating scheduling threads.
        /// </summary>
        static FiberScheduler()
        {
            fibers = new FiberSchedulingThread[Environment.ProcessorCount];
        }

        public static void Schedule(Fiber fiber)
        {
            lock (locker)
            {
                // we first check if the fiber is not null and if it's less then the threashold
                // and then we negate that to get the inverted logic, this is due that we need to
                // do null check first.
                if (!(currentFiber != null && currentFiber.FiberCount < fiberThreadCnt - 1))
                {
                    currentFiber = new FiberSchedulingThread();
                    fibers[fiberThreadCnt++ % Environment.ProcessorCount] = currentFiber;
                }

                currentFiber.Schedule(fiber);
            }
        }

        /// <summary>
        /// Sets the amount of fibers that can run on a single scheduling thread (5 by default).
        /// </summary>
        /// <param name="number">number of fibers.</param>
        public static void SetFibersPerThreadThreshold(int value)
        {
            lock (locker)
            {
                fiberThreadCnt = value;
            }
        }

        /// <summary>
        /// Gets the amount of fibers that can run on a single scheduling thread (5 by default).
        /// </summary>
        /// <returns></returns>
        public static int GetFibersPerThreadThreshold()
        {
            return fiberThreadCnt;
        }
    }
}
