using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using sc_core;


namespace SCS_Example
{
	public class PortTest: sc_module
	{
		Int64 period;
		public sc_out<bool> clk;

		public class clock_process : sc_process_call_base
		{
			public virtual void invoke (sc_process_host host_p)
			{
				step ();
			}

			void step ()
			{
				//for(int i=0; i<10; i++){
				while(true){
					Console.WriteLine ("clock_process_step");
					//clk.write ();
					sc_wait.wait (100, sc_time_unit.SC_FS);
					Console.WriteLine("clock_process_step Current time {0}", sc_simcontext.sc_time_stamp());
				}
			}
		}

		public PortTest (sc_module parent, string instance_name, Int64 period)
				: base(new sc_module_name(instance_name))
		{
			this.period = period;
			sc_process_handle clk_p = sc_simcontext.sc_get_curr_simcontext().create_thread_process("clkgen", false, new clock_process(), null, null);
			//clk = new sc_out<bool> ("clk", 1);
			//s2.sensitive(clk);
		}

		public override void end_of_elaboration ()
		{
			base.end_of_elaboration ();
		}

		public static void Test (object o)
		{
			PortTest simpleTest = new PortTest (null, "clkgen", 100);
			Console.WriteLine ("Starting 1");
			sc_simcontext.sc_start (10000, sc_time_unit.SC_US);
			Console.WriteLine ("Starting 1 Done. End invoke.");
			//sc_simcontext.sc_curr_simcontext.stop ();
			sc_simcontext.sc_curr_simcontext.end ();

			//Console.WriteLine("Starting 2");
			//sc_simcontext.sc_start(10000, sc_time_unit.SC_US);
			//Console.WriteLine("Finished at {0}", sc_simcontext.sc_time_stamp());
		}
	}
}

