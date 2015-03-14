using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemCSharp.Kernel
{
    public class SimulationThreadProcess : SimulationProcess
    {
        public SimulationThreadProcess(string name, Func<object, int> func, SimulationSpawnOptions spawnOptions)
            : base(name, func, spawnOptions)
        { }

        public override string Kind
        {
            get { return "SimulationMethodProcess"; }
        }

        public virtual SimulationThreadProcess NextRunnable()
        {
            throw new NotImplementedException();
        }

        public virtual void SetNextRunnable(SimulationThreadProcess thread)
        {
            throw new NotImplementedException();
        }
    }
}
