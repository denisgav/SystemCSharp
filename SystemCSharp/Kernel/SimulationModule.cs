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
            get{return instance_name;}
        }

        public SimulationModule(string iname)
        {
            this.instance_name = iname;
        }
        public virtual void EndOfElaboration(int code)
        {
        }
    }
}
