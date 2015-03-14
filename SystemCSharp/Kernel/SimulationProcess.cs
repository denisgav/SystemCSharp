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

    public class SimulationProcess : SimulationObject
    {
        public static void Wait( SimulationContext context )
        { throw new NotImplementedException(); }

        public static void Wait( SimulationEvent e, SimulationContext context )
        { throw new NotImplementedException(); }

        public static void Wait( SimulationEventList el, SimulationContext context )
        { throw new NotImplementedException(); }

        public static void Wait( SimulationTime time, SimulationContext context )
        { throw new NotImplementedException(); }

        public static void Wait( SimulationTime time, SimulationEvent e, SimulationContext context )
        { throw new NotImplementedException(); }

        public static void Wait(SimulationTime time, SimulationEventList el, SimulationContext context)
        { throw new NotImplementedException(); }

        public virtual void TriggerStatic()
        { throw new NotImplementedException(); }

        public virtual bool TriggerDynamic(SimulationEvent e)
        { throw new NotImplementedException(); }

        public virtual string Kind
        {
            get { return "SimulationProcess"; }
        }

        private SimulationCurrentProcessKind processKind;
        public virtual SimulationCurrentProcessKind ProcessKind
        {
            get { return processKind; }
        }

        public virtual bool RunProcess()
        {
            throw new NotImplementedException();
        }
        

        public SimulationProcess(string name, Func<object, int> func, SimulationSpawnOptions spawnOptions)
        { }

        protected SimulationProcess m_runnable_p;      // sc_runnable link
    }
}
