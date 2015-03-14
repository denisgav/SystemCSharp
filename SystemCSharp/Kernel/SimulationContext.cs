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

    public enum SimulationStopMode
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
        private SimulationProcess processHandle;
        public virtual SimulationProcess ProcessHandle
        {
            get { return processHandle; }
            set { processHandle = value; }
        }

        private SimulationCurrentProcessKind kind;
        public SimulationCurrentProcessKind Kind
        {
            get { return kind; }
            set { kind = value; }
        }

        public SimulaionCurrentProcessInfo()
        {
            processHandle = null;
            kind = SimulationCurrentProcessKind.NO_PROC;
        }

        public SimulaionCurrentProcessInfo(SimulationProcess process_handle, SimulationCurrentProcessKind kind)
        {
            this.processHandle = process_handle;
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


        public SimulationContext()
        {
            moduleRegistry = new SimulationModuleRegistry(this);
            timeParameters = new SimulationTimeParameters();
            timestamp = SimulationTime.ZeroTime;
            currentTime = SimulationTime.ZeroTime;

            simObjectManager = new SimulationObjectManager();
            childObjects = new List<SimulationObject>();
            childEvents = new List<SimulationEvent>();
            isRunning = false;
            timeParameters = new SimulationTimeParameters();
            currentProcInfo = new SimulaionCurrentProcessInfo();
            runnable = new SimulationRunnable();

            deltaEvents = new List<SimulationEvent>();
            timedEvents = new List<SimulationTimedEvent>();
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

        private SimulationModuleRegistry moduleRegistry;
        public SimulationModuleRegistry ModuleRegistry
        {
            get { return moduleRegistry; }
            set { moduleRegistry = value; }
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

        public virtual int AddDeltaEvent(SimulationEvent e)
        {
            throw new NotImplementedException();
        }

        public virtual void RemoveDeltaEvent(SimulationEvent e)
        {
            throw new NotImplementedException();
        }

        private UInt64 changeStamp = 0;
        public virtual UInt64 ChangeStamp
        {
            get { return changeStamp; }
            set { changeStamp = value; }
        }


        private UInt64 deltaCount = 0;
        public virtual UInt64 DeltaCount
        {
            get
            {
                return deltaCount;
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

        private List<SimulationEvent> deltaEvents;
        public List<SimulationEvent> DeltaEvents
        {
            get { return deltaEvents; }
            set { deltaEvents = value; }
        }

        private List<SimulationTimedEvent> timedEvents;
        public List<SimulationTimedEvent> TimedEvents
        {
            get { return timedEvents; }
            set { timedEvents = value; }
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

        private bool forcedStop = false;
        public virtual bool ForcedStop
        {
            get { return forcedStop; }
            set { forcedStop = value; }
        }

        private SimulationStopMode stopMode;
        public SimulationStopMode StopMode
        {
            get { return stopMode; }
            set { stopMode = value; }
        }


        private SimulaionCurrentProcessInfo currentProcInfo;

        public SimulaionCurrentProcessInfo CurrentProcInfo
        {
            get { return currentProcInfo; }
            set { currentProcInfo = value; }
        }

        private bool writeCheckEnabled;

        public virtual bool WriteCheckEnabled
        {
            get { return writeCheckEnabled; }
            set { writeCheckEnabled = value; }
        }


        private SimulationObject currentWriter;

        public SimulationObject CurrentWritter
        {
            get { return currentWriter; }
            set { currentWriter = value; }
        }



        private SimulationExecutionPhase executionPhase;
        public virtual SimulationExecutionPhase ExecutionPhase
        {
            get { return executionPhase; }
            set { executionPhase = value; }
        }

        private SimulationRunnable runnable;

        public virtual SimulationRunnable Runnable
        {
            get { return runnable; }
            set { runnable = value; }
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

        private bool isPaused = false;
        public virtual bool IsPaused
        {
            get { return isPaused; }
            set { isPaused = value; }
        }
        public virtual void PushRunnableMethod(SimulationMethodProcess method_h)
        {
            runnable.PushBackMethod(method_h);
        }

        public virtual void PushRunnableMethodFront(SimulationMethodProcess method_h)
        {
            runnable.PushFrontMethod(method_h);
        }

        public virtual void PushRunnableThread(SimulationThreadProcess thread_h)
        {
            runnable.PushBackThread(thread_h);
        }

        public virtual void PushRunnableThreadFront(SimulationThreadProcess thread_h)
        {
            runnable.PushFrontThread(thread_h);
        }


        public virtual SimulationMethodProcess PopRunnableMethod()
        {
            SimulationMethodProcess method_h = runnable.PopMethod();
            if (method_h == null)
            {
                ResetCurrProc();
                return null;
            }
            SetCurrProc(method_h);
            return method_h;
        }

        public virtual SimulationThreadProcess PopRunnableThread()
        {
            SimulationThreadProcess thread_h = runnable.PopThread();
            if (thread_h == null)
            {
                ResetCurrProc();
                return null;
            }
            SetCurrProc(thread_h);
            return thread_h;
        }

        public virtual void SetCurrProc(SimulationProcess process_h)
        {
            currentProcInfo.ProcessHandle = process_h;
            currentProcInfo.Kind = process_h.ProcessKind;
            currentWriter = writeCheckEnabled ? process_h : null;
        }

        public virtual void ResetCurrProc()
        {
            currentProcInfo.ProcessHandle = null;
            currentProcInfo.Kind = SimulationCurrentProcessKind.NO_PROC;
            currentWriter = null;
        }

        public virtual void ExecuteMethodNext(SimulationMethodProcess method_h)
        {
            runnable.ExecuteMethodNext(method_h);
        }

        public virtual void ExecuteThreadNext(SimulationThreadProcess thread_h)
        {
            runnable.ExecuteThreadNext(thread_h);
        }

        public virtual void RemoveRunnableMethod(SimulationMethodProcess method_h)
        {
            runnable.RemoveMethod(method_h);
        }

        public virtual void RemoveRunnableThread(SimulationThreadProcess thread_h)
        {
            runnable.RemoveThread(thread_h);
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
            foreach (SimulationModule m in moduleRegistry)
                m.ElaborationDone();
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

        public virtual void Crunch(bool once = false)
        {
            while (true)
            {

                // EVALUATE PHASE

                executionPhase = SimulationExecutionPhase.PhaseEvaluate;
                bool empty_eval_phase = true;
                while (true)
                {

                    // execute method processes

                    runnable.ToggleMethods();
                    SimulationMethodProcess method_h = PopRunnableMethod();
                    while (method_h != null)
                    {
                        empty_eval_phase = false;
                        if (!method_h.RunProcess()) { ResetCurrProc(); return; }
                        method_h = PopRunnableMethod();
                    }

                    // execute (c)thread processes

                    runnable.ToggleMethods();
                    SimulationThreadProcess thread_h = PopRunnableThread();
                    while (thread_h != null)
                    {
                        thread_h = PopRunnableThread();
                    }

                    /*
                    if( thread_h != null ) {
                        empty_eval_phase = false;
                    m_cor_pkg->yield( thread_h->m_cor_p );
                    }
                    if( m_error ) {
                    goto out;
                    }
                    */
                    // check for call(s) to sc_stop
                    if (forcedStop)
                    {
                        if (stopMode == SimulationStopMode.STOP_IMMEDIATE) { ResetCurrProc(); return; }
                    }

                    // no more runnable processes

                    if (runnable.IsEmpty)
                    {
                        break;
                    }
                }

                // will work.

                executionPhase = SimulationExecutionPhase.PhaseUpdate;
                if (!empty_eval_phase)
                {
                    //	    SC_DO_PHASE_CALLBACK_(evaluation_done);
                    changeStamp++;
                    deltaCount++;
                }
                //m_prim_channel_registry->perform_update();
                //SC_DO_PHASE_CALLBACK_(update_done);
                executionPhase = SimulationExecutionPhase.PhaseNotify;


                // check for call(s) to sc_stop
                if (forcedStop)
                {
                    break;
                }


                // NOTIFICATION PHASE:
                //
                // Process delta notifications which will queue processes for 
                // subsequent execution.

                int size = deltaEvents.Count;
                if (size != 0)
                {
                    foreach (SimulationEvent e in deltaEvents)
                    {
                        e.Trigger();
                    }
                    deltaEvents.Clear();
                }

                if (runnable.IsEmpty)
                {
                    // no more runnable processes
                    break;
                }

                // if sc_pause() was called we are done.

                if (isPaused) break;

                // IF ONLY DOING ONE CYCLE, WE ARE DONE. OTHERWISE EXECUTE NEW CALLBACKS

                if (once) break;
            }
        }

    }
}
