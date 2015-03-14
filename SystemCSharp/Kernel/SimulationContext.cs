using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemCSharp.Kernel
{
    public enum SimulationExecutionPhase
    {
        PhaseInitialize = 0,
        PhaseEvaluate,
        PhaseUpdate,
        PhaseNotify
    }

    public enum SimulationStpMode
    {          // sc_stop modes:
        STOP_FINISH_DELTA,
        STOP_IMMEDIATE
    };

    public enum SimulationStarvationPolicy
    {
        EXIT_ON_STARVATION,
        RUN_TO_TIME
    };

    public enum SimulationCurrentProcessKind
    {
        NO_PROC,
        METHOD_PROC,
        THREAD_PROC,
        CTHREAD_PROC
    }

    public class SimulaionCurrentProcessInfo
    {
        SimulationProcess process_handle;
        SimulationCurrentProcessKind kind;
        public SimulaionCurrentProcessInfo()
        {
            process_handle = null;
            kind = SimulationCurrentProcessKind.NO_PROC;
        }

        public SimulaionCurrentProcessInfo(SimulationProcess process_handle, SimulationCurrentProcessKind kind)
        {
            this.process_handle = process_handle;
            this.kind = kind;
        }
    };

    public class SimulationContext
    {
        public const int SIM_OK = 0;
        public const int SIM_ERROR = 1;
        public const int SIM_USER_STOP = 2;

        public static void Start()
        {
            Start(SimulationContext.GlobalSimContext.MaxSimulationTime - SimulationContext.GlobalSimContext.Timestamp, SimulationStarvationPolicy.EXIT_ON_STARVATION);
        }

        public static void Start(SimulationTime duration, SimulationStarvationPolicy p = SimulationStarvationPolicy.RUN_TO_TIME)
        {
            SimulationContext context_p;      // current simulation context.
            SimulationTime entry_time;     // simulation time upon entry.
            SimulationTime exit_time = SimulationTime.ZeroTime;      // simulation time to set upon exit.
            UInt64 starting_delta; // delta count upon entry.
            int status;         // current simulation status.

            // Set up based on the arguments passed to us:

            context_p = SimulationContext.GlobalSimContext;
            starting_delta = context_p.DeltaCount;
            entry_time = context_p.CurrentTime;
            if (p == SimulationStarvationPolicy.RUN_TO_TIME)
                exit_time = context_p.Timestamp + duration;

            // If the simulation status is bad issue the appropriate message:

            status = context_p.SimulationStatus;
            if (status != SIM_OK)
            {
                if (status == SIM_USER_STOP)
                    throw new Exception("Could not start simulation due to user stop notification");
                if (status == SIM_ERROR)
                    throw new Exception("Could not start simulation due to internal error");
                return;
            }

            // If the simulation status is good perform the simulation:

            context_p.Simulate(duration);

            // Re-check the status:

            status = context_p.SimulationStatus;

            // Update the current time to the exit time if that is the starvation
            // policy:

            if (p == SimulationStarvationPolicy.RUN_TO_TIME && !context_p.IsPaused && status == SIM_OK)
            {
                context_p.currentTime = exit_time;
            }

        }
        public static void Start(int duration, SimulationTimeUnit unit, SimulationStarvationPolicy p = SimulationStarvationPolicy.RUN_TO_TIME)
        {
            Start(new SimulationTime((double)duration, unit), p);
        }

        public static void Start(double duration, SimulationTimeUnit unit, SimulationStarvationPolicy p = SimulationStarvationPolicy.RUN_TO_TIME)
        {
            Start(new SimulationTime(duration, unit), p);
        }


        public const char HIERARCHY_CHAR = '.';

        private SimulationTimeParameters timeParameters;
        public SimulationTimeParameters TimeParameters
        {
            get { return timeParameters; }
            set { timeParameters = value; }
        }

        private static SimulationContext globalSimContext;
        public static SimulationContext GlobalSimContext
        {
            get
            {
                if (globalSimContext == null)
                    globalSimContext = new SimulationContext();
                return globalSimContext;
            }
        }

        private bool isRunning;
        public bool IsRunning
        {
            get { return isRunning; }
            set { isRunning = value; }
        }

        private List<SimulationObject> childObjects;
        public virtual List<SimulationObject> ChildObjects
        {
            get { return childObjects; }
            set { childObjects = value; }
        }

        private List<SimulationEvent> childEvents;
        public virtual List<SimulationEvent> ChildEvents
        {
            get { return childEvents; }
            set { childEvents = value; }
        }

        private SimulationObjectManager simObjectManager;
        public SimulationObjectManager SimObjectManager
        {
            get { return simObjectManager; }
            set { simObjectManager = value; }
        }

        public SimulationObject GetActiveObject()
        {
            return simObjectManager.HierarchyCurr();
        }

        public SimulationContext()
        {
            simObjectManager = new SimulationObjectManager();
            childObjects = new List<SimulationObject>();
            childEvents = new List<SimulationEvent>();
            isRunning = false;
            timeParameters = new SimulationTimeParameters();
        }

        public virtual int AddDeltaEvent(SimulationEvent e)
        {
            throw new NotImplementedException();
        }

        public virtual void RemoveDeltaEvent(SimulationEvent e)
        {
            throw new NotImplementedException();
        }

        public UInt64 DeltaCount
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual void AddTimedEvent(SimulationTimedEvent e)
        {
            throw new NotImplementedException();
        }

        private SimulationTime timestamp;
        public virtual SimulationTime Timestamp
        {
            get { return timestamp; }
        }

        private SimulationTime currentTime;
        public virtual SimulationTime CurrentTime
        {
            get { return currentTime; }
        }


        private SimulationTime maxSimulationTime;
        public virtual SimulationTime MaxSimulationTime
        {
            get
            {
                if (maxSimulationTime.Equals(SimulationTime.ZeroTime))
                {
                    maxSimulationTime = SimulationTime.MaxTime;
                }
                return maxSimulationTime;
            }
            set { maxSimulationTime = value; }
        }

        private Exception internalError;
        public virtual Exception InternalError
        {
            get { return internalError; }
            set { internalError = value; }
        }

        private bool forcedStop;
        public virtual bool ForcedStop
        {
            get { return forcedStop; }
            set { forcedStop = value; }
        }

        public virtual int SimulationStatus
        {
            get
            {
                if (internalError != null)
                {
                    return SIM_ERROR;
                }
                if (forcedStop)
                {
                    return SIM_USER_STOP;
                }
                return SIM_OK;
            }
        }

        private bool isPaused;
        public virtual bool IsPaused
        {
            get { return isPaused; }
            set { isPaused = value; }
        }



        public virtual void Initialize(bool noCrunch)
        {
            Elaborate();

            PrepareToSimulation();
            if (noCrunch == false)
                Crunch();
        }

        public virtual void Simulate(SimulationTime period)
        {
            throw new NotImplementedException();
        }

        public virtual void Elaborate()
        {
            throw new NotImplementedException();
        }

        public virtual void PrepareToSimulation()
        {
            throw new NotImplementedException();
        }

        public virtual void Stop()
        {
            throw new NotImplementedException();
        }

        public virtual void End()
        {
            throw new NotImplementedException();
        }

        public virtual void Reset()
        {
            throw new NotImplementedException();
        }

        public virtual void Crunch()
        {
            throw new NotImplementedException();
        }
    }
}
