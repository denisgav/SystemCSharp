using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemCSharp.Kernel
{
    public class SimulationMethodProcess : SimulationProcess
    {
        public SimulationMethodProcess(string name, Func<object, int> func)
            : base(name, false)
        { }

        public virtual string Kind
        {
            get { return "SimulationMethodProcess"; }
        }
        
    }
}
