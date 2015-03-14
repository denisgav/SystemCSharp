using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemCSharp.Kernel
{
    public class SimulationMethodProcess : SimulationProcess
    {
        public SimulationMethodProcess(string name, Func<object, int> func, SimulationSpawnOptions spawnOptions)
            : base(name, func, spawnOptions)
        { }

        public override string Kind
        {
            get { return "SimulationMethodProcess"; }
        }

        public virtual SimulationMethodProcess NextRunnable()
        {
            throw new NotImplementedException();
        }

        public virtual void SetNextRunnable(SimulationMethodProcess method)
        {
            throw new NotImplementedException();
        }
        
    }
}
