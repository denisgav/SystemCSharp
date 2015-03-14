using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemCSharp.Kernel
{
    public class SimulationModuleRegistry : List<SimulationModule>
    {
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

        // called when elaboration is done
        public virtual void ElaborationDone()
        {
            foreach (SimulationModule m in this)
                m.ElaborationDone();
        }

        // called before simulation begins
        public virtual void StartSimulation()
        {
            foreach (SimulationModule m in this)
                m.StartSimulation();
        }

        // called after simulation ends
        public virtual void SimulationDone()
        {
            foreach (SimulationModule m in this)
                m.SimulationDone();
        }

        public SimulationModuleRegistry()
            : this(SimulationContext.GlobalSimContext)
        { }

        public SimulationModuleRegistry(SimulationContext simContext)
        {
            this.simContext = simContext;
        }
    }
}
