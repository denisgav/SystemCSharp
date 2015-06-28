using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using sc_core;

namespace SCS_Example
{
	class SimpleTest : sc_module
	{
		Int64 period;
		//public SystemCsharp.sc_out<bool> clk;

		public class my_process1 : sc_process_call_base
		{
			public virtual void invoke(sc_process_host host_p)
			{
				step();
			}
			void step()
			{
				Console.WriteLine("my_process1_step");
			}
		}

		public class my_thread1 : sc_process_call_base
		{
			public virtual void invoke(sc_process_host host_p)
			{
				for (int i = 0; i < 3; i++)
				{
					step();
				}
			}
			void step()
			{
				Console.WriteLine("my_thread1_step");
				Console.WriteLine("my_thread1_step wait 10 us");
				sc_wait.wait(10, sc_time_unit.SC_US);
				Console.WriteLine("my_thread1_step wait 10 us done");
				Console.WriteLine("my_thread1_step Current time {0}", sc_simcontext.sc_time_stamp());
				Console.WriteLine("my_thread1_step done");
			}
		}

		public class my_thread2 : sc_process_call_base
		{
			public virtual void invoke(sc_process_host host_p)
			{
				for (int i = 0; i < 5; i++)
				{
					step();
				}
			}
			void step()
			{
				Console.WriteLine("my_thread2_step");
				Console.WriteLine("my_thread2_step wait 15 us");
				sc_wait.wait(15, sc_time_unit.SC_US);
				Console.WriteLine("my_thread2_step wait 15 us done");
				Console.WriteLine("my_thread2_step Current time {0}", sc_simcontext.sc_time_stamp());
				Console.WriteLine("my_thread2_step done");
			}
		}



		public SimpleTest(sc_module parent, string instance_name, Int64 period)
			: base(new sc_module_name(instance_name))
		{
			this.period = period;
			//SystemCsharp.SC.Register(parent, this, instance_name);
			//clk = new SystemCsharp.sc_out<bool>(this); // this, "clk");
			//sc_process_handle s1 = sc_simcontext.sc_get_curr_simcontext().create_method_process("clkgen1", false, new my_process1(), null, null);
			sc_process_handle s2 = sc_simcontext.sc_get_curr_simcontext().create_thread_process("clkgen2", false, new my_thread1(), null, null);
			sc_process_handle s3 = sc_simcontext.sc_get_curr_simcontext().create_thread_process("clkgen3", false, new my_thread2(), null, null);
			// = new sc_method_process("clkgen", false, new my_process1(), null, null);
			//s.sensitive(clk);
		}

		public override void end_of_elaboration()
		{
			base.end_of_elaboration();
		}

		public static void Test(object o)
		{
			SimpleTest simpleTest = new SimpleTest(null, "clkgen1", 100);
			Console.WriteLine("Starting 1");
			sc_simcontext.sc_start(10000, sc_time_unit.SC_US);
			//Console.WriteLine("Starting 2");
			//sc_simcontext.sc_start(10000, sc_time_unit.SC_US);
			//Console.WriteLine("Finished at {0}", sc_simcontext.sc_time_stamp());
		}

	}

}
