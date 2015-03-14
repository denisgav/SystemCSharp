using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCS_Example
{
    class clkgen : SystemCSharp.Kernel.SimulationModule
    {
        Int64 period;
        //public SystemCsharp.sc_out<bool> clk;

        int step(object o)
        {
            Console.WriteLine("step");
            //clk.Write(1 - (clk.Read() & 1L), period / 2L);
            return 0;
        }

        public clkgen(SystemCSharp.Kernel.SimulationModule parent, String instance_name, Int64 period)
            : base(instance_name)
        {
            this.period = period;
            //SystemCsharp.SC.Register(parent, this, instance_name);
            //clk = new SystemCsharp.sc_out<bool>(this); // this, "clk");
            var s = new SystemCSharp.Kernel.SimulationMethodProcess("clkgen", step);
            //s.sensitive(clk);
        }

        public override void EndOfElaboration(int code)
        {
            //clk.Write(1 - (clk.Read() & 1L), period / 2L);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            clkgen clkgen1 = new clkgen(null, "clkgen1", 100);
            Console.WriteLine("Starting 1");
            SystemCSharp.Kernel.SimulationContext.GlobalSimContext.Elaborate();
            Console.WriteLine("Starting 2");


            SystemCSharp.Kernel.SimulationContext.Start(10000, SystemCSharp.Kernel.SimulationTimeUnit.FS);
            Console.WriteLine("Finished at {0}", SystemCSharp.Kernel.SimulationContext.GlobalSimContext.CurrentTime);

        }
    }
}
