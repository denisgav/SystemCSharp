using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemCSharp.Kernel
{
    public class SimulationObjectManager
    {
        public class ObjectManagerTableEntry
        {
            private SimulationObject simObject;
            public SimulationObject SimObject
            {
                get { return simObject; }
                set { simObject = value; }
            }

            private SimulationEvent simEvent;
            public SimulationEvent SimEvent
            {
                get { return simEvent; }
                set { simEvent = value; }
            }            
        }

        private Dictionary<string, ObjectManagerTableEntry> instanceTable;
        public virtual Dictionary<string, ObjectManagerTableEntry> InstanceTable
        {
            get { return instanceTable; }
            set { instanceTable = value; }
        }

        public virtual SimulationObject GetObjectByName(string name)
        {
            if (instanceTable.ContainsKey(name))
                return instanceTable[name].SimObject;

            return null;
        }

        public virtual SimulationEvent GetEventByName(string name)
        {
            if (instanceTable.ContainsKey(name))
                return instanceTable[name].SimEvent;

            return null;
        }

        public virtual void InsertEvent(string name, SimulationEvent e)
        {
            if (instanceTable.ContainsKey(name) == false)
            {
                instanceTable.Add(name, new ObjectManagerTableEntry(){SimEvent = e});
            }
            else
            {
                instanceTable[name].SimEvent = e;
            }
        }

        public virtual void InsertObject(string name, SimulationObject o)
        {
            if (instanceTable.ContainsKey(name) == false)
            {
                instanceTable.Add(name, new ObjectManagerTableEntry() { SimObject = o });
            }
            else
            {
                instanceTable[name].SimObject = o;
            }
        }

        public virtual void RemoveEvent(string name)
        {
            if (instanceTable.ContainsKey(name))
                instanceTable[name].SimEvent = null;
        }

        public virtual void RemoveObject(string name)
        {
            if (instanceTable.ContainsKey(name))
                instanceTable[name].SimObject = null;
        }

        private Stack<SimulationObject> hierarchyStack;
        public virtual Stack<SimulationObject> HierarchyStack
        {
            get { return hierarchyStack; }
            set { hierarchyStack = value; }
        }

        public virtual void HierarchyPush(SimulationObject o)
        {
            hierarchyStack.Push(o);
        }

        public virtual SimulationObject HierarchyPop()
        {
            return hierarchyStack.Pop();
        }

        public virtual SimulationObject HierarchyCurr()
        {
            return hierarchyStack.Peek();
        }

        public virtual int HierarchySize()
        {
            return hierarchyStack.Count;
        }

        public SimulationObjectManager()
        {
            hierarchyStack = new Stack<SimulationObject>();
            instanceTable = new Dictionary<string, ObjectManagerTableEntry>();
        }
        
    }
}
