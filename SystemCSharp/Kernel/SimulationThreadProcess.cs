using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemCSharp.Kernel
{
    public class SimulationThreadProcess : SimulationProcess
    {
        public SimulationThreadProcess(string name)
            : base(name, true)
        { }
    }
}
