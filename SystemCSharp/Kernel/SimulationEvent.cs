using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemCSharp.Kernel
{
    public enum SimulationEventNotifyType { NONE, DELTA, TIMED };

    public class SimulationTimedEvent
    {
        private SimulationEvent simEvent;
        public virtual SimulationEvent SimEvent
        {
            get { return simEvent; }
            set { simEvent = value; }
        }

        private SimulationTime notifyTime;
        public virtual SimulationTime NotifyTime
        {
            get { return notifyTime; }
            set { notifyTime = value; }
        }

        public SimulationTimedEvent()
        { }

        public SimulationTimedEvent(SimulationEvent simEvent, SimulationTime notifyTime)
        {
            this.simEvent = simEvent;
            this.notifyTime = notifyTime;
        }

        public SimulationTimedEvent(SimulationTimedEvent ste)
            : this(ste.simEvent, ste.notifyTime)
        { }
    }

    public class SimulationEvent
    {
        private List<SimulationMethodProcess> methodsStatic;
        public virtual List<SimulationMethodProcess> MethodsStatic
        {
            get { return methodsStatic; }
            set { methodsStatic = value; }
        }

        private List<SimulationMethodProcess> methodsDynamic;
        public virtual List<SimulationMethodProcess> MethodsDynamic
        {
            get { return methodsDynamic; }
            set { methodsDynamic = value; }
        }

        private List<SimulationThreadProcess> threadsStatic;
        public virtual List<SimulationThreadProcess> ThreadsStatic
        {
            get { return threadsStatic; }
            set { threadsStatic = value; }
        }

        private List<SimulationThreadProcess> threadsDynamic;
        public virtual List<SimulationThreadProcess> ThreadsDynamic
        {
            get { return threadsDynamic; }
            set { threadsDynamic = value; }
        }

        private string name;
        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }

        private SimulationObject parent;
        public virtual SimulationObject Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        private SimulationEventNotifyType notifyType;
        public virtual SimulationEventNotifyType NotifyType
        {
            get { return notifyType; }
            set { notifyType = value; }
        }

        private SimulationTimedEvent timedData;
        public virtual SimulationTimedEvent TimedData
        {
            get { return timedData; }
            set { timedData = value; }
        }

        public virtual string BaseName
        {
            get
            {
                string[] name_parts = name.Split(new char[] { SimulationContext.HIERARCHY_CHAR }, StringSplitOptions.RemoveEmptyEntries);
                if (name_parts.Length >= 2)
                    return name_parts[name_parts.Length - 2];
                else
                    return name_parts[name_parts.Length - 1];
            }
        }

        private SimulationContext simContext;
        public virtual SimulationContext SimulationContext
        {
            get
            {
                if (simContext == null)
                    simContext = SimulationContext.GlobalSimContext;
                return simContext;
            }
        }

        private int deltaEventIndex;
        public virtual int DeltaEventIndex
        {
            get { return deltaEventIndex; }
            set { deltaEventIndex = value; }
        }

        public virtual bool IsInHierarchy
        {
            get { return string.IsNullOrWhiteSpace(name); }
        }

        public virtual void Cancel()
        {
            switch (notifyType)
            {
                case SimulationEventNotifyType.DELTA:
                    {
                        simContext.RemoveDeltaEvent(this);
                    }
                    break;
                case SimulationEventNotifyType.TIMED:
                    {
                        notifyType = SimulationEventNotifyType.NONE;
                        timedData = null;
                    }
                    break;
                default:
                    { }
                    break;
            }
        }

        public virtual void Trigger()
        {
            foreach (SimulationMethodProcess smp in methodsStatic)
            {
                smp.TriggerStatic();
            }

            for (int i = 0; i < methodsDynamic.Count; )
            {

                bool res = methodsDynamic[i].TriggerDynamic(this);
                if (res)
                {
                    methodsDynamic.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }

            foreach (SimulationThreadProcess smp in threadsStatic)
            {
                smp.TriggerStatic();
            }

            for (int i = 0; i < threadsDynamic.Count; )
            {

                bool res = threadsDynamic[i].TriggerDynamic(this);
                if (res)
                {
                    threadsDynamic.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }

        public virtual void Notify()
        {
            Cancel();
            Trigger();
        }

        public virtual void Notify(double delay, SimulationTimeUnit tu)
        {
            Notify(new SimulationTime(delay, tu, SimulationContext));
        }

        public virtual void Notify(SimulationTime time)
        {
            if (notifyType == SimulationEventNotifyType.DELTA)
                return;

            if (time.Equals(SimulationTime.ZeroTime))
            {
                if (notifyType == SimulationEventNotifyType.TIMED)
                {
                    // remove this event from the timed events set
                    timedData.SimEvent = null;
                    timedData = null;
                }
                // add this event to the delta events set
                deltaEventIndex = SimulationContext.AddDeltaEvent(this);
                notifyType = SimulationEventNotifyType.DELTA;
                return;
            }
            if (notifyType == SimulationEventNotifyType.TIMED)
            {
                if (timedData.NotifyTime <= SimulationContext.Timestamp + time)
                {
                    return;
                }
                // remove this event from the timed events set
                timedData.SimEvent = null;
                timedData = null;
            }
            // add this event to the timed events set
            SimulationTimedEvent et = new SimulationTimedEvent(this, SimulationContext.Timestamp + time);
            SimulationContext.AddTimedEvent(et);
            timedData = et;
            notifyType = SimulationEventNotifyType.TIMED;
        }
    }

    public class SimulationEventList : List<SimulationEvent>
    {
        private int busy;
        public virtual bool IsBusy
        {
            get { return busy != 0; }
        }

        private bool autoDelete;
        public bool AutoDelete
        {
            get { return autoDelete; }
            set { autoDelete = value; }
        }

        public bool Temporary
        {
            get { return autoDelete && !(busy != 0); }
        }


        public virtual void Swap(SimulationEventList that)
        {
            if ((IsBusy == true) || (that.IsBusy == true))
                throw new Exception("Could not swap when some of event lists is busy");

            List<SimulationEvent> data = new List<SimulationEvent>();
            data.AddRange(this);
            this.Clear();
            this.AddRange(that);
            that.Clear();
            that.AddRange(data);
        }

        public virtual void DoAutoDelete()
        {
            if (busy != 0)
            {
                --busy;
            }
            if (busy == 0 && autoDelete)
            {
                Clear();
            }
        }

        public virtual void AddDynamic(SimulationMethodProcess method)
        {
            foreach (SimulationEvent e in this)
            {
                e.MethodsDynamic.Add(method);
            }
        }
        public virtual void AddDynamic(SimulationThreadProcess thread)
        {
            foreach (SimulationEvent e in this)
            {
                e.ThreadsDynamic.Add(thread);
            }
        }
        public virtual void RemoveDynamic(SimulationMethodProcess method, SimulationEvent except)
        {
            foreach (SimulationEvent e in this)
            {
                if(e != except)
                    e.MethodsDynamic.Remove(method);
            }
        }
        public virtual void RemoveDynamic(SimulationThreadProcess thread, SimulationEvent except)
        {
            foreach (SimulationEvent e in this)
            {
                if (e != except)
                    e.ThreadsDynamic.Remove(thread);
            }
        }

    }
}
