using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemCSharp.Kernel
{
    public enum SimulationProcessThrowType
    {
        THROW_NONE = 0,
        THROW_KILL,
        THROW_USER,
        THROW_ASYNC_RESET,
        THROW_SYNC_RESET
    }

    public enum SimulationProcessState
    {
        ps_bit_disabled = 1,      // process is disabled.
        ps_bit_ready_to_run = 2,  // process is ready to run.
        ps_bit_suspended = 4,     // process is suspended.
        ps_bit_zombie = 8,        // process is a zombie.
        ps_normal = 0             // must be zero.
    }

    public enum SimulationProcessResetType
    {             // types for sc_process_b::reset_process()
        reset_asynchronous = 0,   // asynchronous reset.
        reset_synchronous_off,    // turn off synchronous reset sticky bit.
        reset_synchronous_on      // turn on synchronous reset sticky bit.
    }

    public enum SimulationProcessTriggeringType
    {
        STATIC,
        EVENT,
        OR_LIST,
        AND_LIST,
        TIMEOUT,
        EVENT_TIMEOUT,
        OR_LIST_TIMEOUT,
        AND_LIST_TIMEOUT
    }

    public class SimulationProcess
    {
        public SimulationProcess(string name, bool is_thread)
        { }
    }
}
