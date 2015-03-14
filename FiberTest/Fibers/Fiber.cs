using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemCSharp.Fibers
{
    /// <summary>
    /// Represents a Fiber that's a lightweigh isolated unit of concurent
    /// execution.
    /// </summary>
    public class Fiber
    {
        internal IEnumerator<FiberStatus> FiberContext;

        private static int internalId = 0;

        /// <summary>
        /// Gets the unique fiber Id.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Initializes a new instance of a Fiber.
        /// </summary>
        /// <param name="fiber">fiber funcion.</param>
        public Fiber(Func<IEnumerable<FiberStatus>> fiber)
        {
            Id = ++internalId;
            this.FiberContext = fiber().GetEnumerator();
        }

        /// <summary>
        /// Schedules fiber for execution.
        /// </summary>
        public void Run()
        {
            FiberScheduler.Schedule(this);
        }
    }
}
