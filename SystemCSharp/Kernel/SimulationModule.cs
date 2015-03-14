using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemCSharp.Kernel
{
    public class SimulationModule
    {
        string instance_name;
        public string Name
        {
            get { return instance_name; }
        }

        public SimulationModule(string iname)
        {
            this.instance_name = iname;
        }

        // called when elaboration is done
        public virtual void ElaborationDone()
        {
            throw new NotImplementedException();
        }

        // called before simulation begins
        public virtual void StartSimulation()
        {
            throw new NotImplementedException();
        }

        // called after simulation ends
        public virtual void SimulationDone()
        {
            throw new NotImplementedException();
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

        private void ModuleInit()
        {
            simContext.ModuleRegistry.Add(this);
        }
    }
}
