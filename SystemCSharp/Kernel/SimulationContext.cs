using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemCSharp.Kernel
{
    public class SimulationContext
    {
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
        
        
        
    }
}
