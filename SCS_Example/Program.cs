using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sc_core;
using System.Threading;

namespace SCS_Example
{
    
    class clkgen : sc_module
    {
        Int64 period;
        //public SystemCsharp.sc_out<bool> clk;

        public class my_process1 : sc_process_call_base
        {
            public override void invoke(sc_process_host host_p)
            {
                step();
                base.invoke(host_p);
            }
            void step()
            {
                Console.WriteLine("my_process1_step");
            }
        }

        public class my_thread1 : sc_process_call_base
        {
            public override void invoke(sc_process_host host_p)
            {
                step();
                base.invoke(host_p);
            }
            void step()
            {
                Console.WriteLine("my_thread1_step");
                Console.WriteLine("my_thread1_step wait 10 us");
                sc_wait.wait(10, sc_time_unit.SC_US);
                Console.WriteLine("my_thread1_step wait 10 us done");
                Console.WriteLine("my_thread1_step Current time {0}", sc_simcontext.sc_time_stamp());
            }
        }

        

        public clkgen(sc_module parent, string instance_name, Int64 period)
            : base(new sc_module_name(instance_name))
        {
            this.period = period;
            //SystemCsharp.SC.Register(parent, this, instance_name);
            //clk = new SystemCsharp.sc_out<bool>(this); // this, "clk");
            sc_process_handle s1 = sc_simcontext.sc_get_curr_simcontext().create_method_process("clkgen1", false, new my_process1(), null, null);
            sc_process_handle s2 = sc_simcontext.sc_get_curr_simcontext().create_thread_process("clkgen2", false, new my_thread1(), null, null);
            // = new sc_method_process("clkgen", false, new my_process1(), null, null);
            //s.sensitive(clk);
        }

        public override void end_of_elaboration()
        {
            base.end_of_elaboration();
        }

    }
    
    class Program
    {
        static void Main(string[] args)
        {

            Thread t = new Thread(Test);
            t.Start();
            t.Join();
            Console.ReadKey();
        }

        public static void Test(object o)
        {
            clkgen clkgen1 = new clkgen(null, "clkgen1", 100);
            Console.WriteLine("Starting 1");
            sc_simcontext.sc_start(100, sc_time_unit.SC_US);
            Console.WriteLine("Starting 2");


            //sc_simcontext.sc_start(100, sc_time_unit.SC_US);
            Console.WriteLine("Finished at {0}", sc_simcontext.sc_time_stamp());
        }
    }
}
