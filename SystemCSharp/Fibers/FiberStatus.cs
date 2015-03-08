using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemCSharp.Fibers
{
    /// <summary>
    /// Represents a context switch status of a fiber.
    /// </summary>
    public enum ContextStatus
    {
        Switch,
        Wait
    }

    /// <summary>
    /// Represents a Fiber Status upon making a context switch.
    /// </summary>
    public class FiberStatus
    {
        /// <summary>
        /// Gets the fiber context switch status.
        /// </summary>
        public ContextStatus Status { get; internal set; }

        private FiberStatus(ContextStatus status)
        {
            this.Status = status;
        }

        /// <summary>
        /// Invokes a context switch to another fiber.
        /// </summary>
        /// <returns>Fiber Status.</returns>
        public static FiberStatus Yield()
        {
            return new FiberStatus(ContextStatus.Switch);
        }

        /// <summary>
        /// Invokes a context switch with a full cycle wait.
        /// </summary>
        /// <remarks>
        /// This is usefull for controling the priority of certain
        /// fiber tasks.
        /// </remarks>
        /// <returns></returns>
        public static FiberStatus Wait()
        {
            return new FiberStatus(ContextStatus.Wait);
        }
    }
}
