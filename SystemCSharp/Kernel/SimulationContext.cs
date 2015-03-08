using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemCSharp.Kernel
{
    public class SimulationContext
    {
        private SimulationTimeParameters timeParameters;
        public SimulationTimeParameters TimeParameters
        {
            get { return timeParameters; }
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
        
        
    }
}
