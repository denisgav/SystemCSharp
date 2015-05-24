/*****************************************************************************

  The following code is derived, directly or indirectly, from the SystemC
  source code Copyright (c) 1996-2014 by all Contributors.
  All Rights reserved.

  The contents of this file are subject to the restrictions and limitations
  set forth in the SystemC Open Source License (the "License");
  You may not use this file except in compliance with such restrictions and
  limitations. You may obtain instructions on how to receive a copy of the
  License at http://www.accellera.org/. Software distributed by Contributors
  under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF
  ANY KIND, either express or implied. See the License for the specific
  language governing rights and limitations under the License.

 *****************************************************************************/

namespace sc_core
{


    //==============================================================================
    // sc_cthread_process -
    //
    //==============================================================================
    public class sc_cthread_process : sc_thread_process
    {
        //------------------------------------------------------------------------------
        //"sc_cthread_process::sc_cthread_process"
        //
        // This is the object instance constructor for this class.
        //------------------------------------------------------------------------------
        public sc_cthread_process(string name_p, bool free_host, sc_process_call_base method_p, sc_process_host host_p, sc_spawn_options opt_p)
            : base(name_p, free_host, method_p, host_p, opt_p)
        {
            m_dont_init = true;
            m_process_kind = sc_curr_proc_kind.SC_CTHREAD_PROC_;
        }


        //------------------------------------------------------------------------------
        //"sc_cthread_process::dont_initialize"
        //
        // This virtual method sets the initialization switch for this object instance.
        //------------------------------------------------------------------------------
        public override void dont_initialize(bool dont)
        {
            sc_report_handler.report(sc_core.sc_severity.SC_WARNING, "dont_initialize() has no effect for SC_CTHREADs", "");
        }

        public override string kind()
        {
            return "sc_cthread_process";
        }



        public override void Dispose()
        {
            base.Dispose();
        }

        //------------------------------------------------------------------------------
        //"sc_cthread_process::wait_halt"
        //
        //------------------------------------------------------------------------------
        public void wait_halt()
        {
            m_wait_cycle_n = 0;
            suspend_me();
            //throw new sc_halt();
        }



        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        // for SC_CTHREADs

        public static void halt()
        {
            halt(sc_simcontext.sc_get_curr_simcontext());
        }

        // for SC_CTHREADs

        //C++ TO C# CONVERTER NOTE: Overloaded method(s) are created above to convert the following method having default parameters:
        //ORIGINAL LINE: void halt(sc_simcontext* simc = sc_get_curr_simcontext())
        public static void halt(sc_simcontext simc)
        {
            sc_curr_proc_info cpi = simc.get_curr_proc_info();
            switch (cpi.kind)
            {
                case sc_curr_proc_kind.SC_CTHREAD_PROC_:
                    {
                        (cpi.process_handle as sc_cthread_process).wait_halt();

                        break;
                    }
                default:
                    sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "halt() is only allowed in SC_CTHREADs", "");
                    break;
            }
        }
        public static void wait(int n)
        {
            wait(n, sc_simcontext.sc_get_curr_simcontext());
        }



        public static void wait(int n, sc_simcontext simc)
        {
            sc_curr_proc_info cpi = simc.get_curr_proc_info();
            if (n <= 0)
            {
                string msg = string.Format("n = {0}", n);
                sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "wait(n) is only valid for n > 0", msg);
            }
            switch (cpi.kind)
            {
                case sc_curr_proc_kind.SC_THREAD_PROC_:
                case sc_curr_proc_kind.SC_CTHREAD_PROC_:
                    (cpi.process_handle as sc_cthread_process).wait_cycles(n);
                    break;
                default:
                    //C++ TO C# CONVERTER TODO TASK: There is no direct equivalent in C# to the C++ __LINE__ macro:
                    //C++ TO C# CONVERTER TODO TASK: There is no direct equivalent in C# to the C++ __FILE__ macro:
                    sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "wait() is only allowed in SC_THREADs and SC_CTHREADs", "\n        " + "in SC_METHODs use next_trigger() instead");
                    break;
            }
        }
        /*
        public static void at_posedge(sc_signal_in_if<bool> s)
        {
            at_posedge(s, sc_get_curr_simcontext());
        }


        public static void at_posedge(sc_signal_in_if<bool> s, sc_simcontext simc)
        {
            if (s.read() == true)
                do
                {
                    sc_event.wait(simc);
                } while (s.read() == true);
            do
            {
                sc_event.wait(simc);
            } while (s.read() == false);
        }
        public static void at_posedge(sc_signal_in_if<sc_dt.sc_logic> s)
        {
            at_posedge(s, sc_get_curr_simcontext());
        }

        public static void at_posedge(sc_signal_in_if<sc_dt.sc_logic> s, sc_simcontext simc)
        {
            if (s.read() == '1')
                do
                {
                    sc_event.wait(simc);
                } while (s.read() == '1');
            do
            {
                sc_event.wait(simc);
            } while (s.read() == '0');
        }
        public static void at_negedge(sc_signal_in_if<bool> s)
        {
            at_negedge(s, sc_get_curr_simcontext());
        }

        //C++ TO C# CONVERTER NOTE: Overloaded method(s) are created above to convert the following method having default parameters:
        //ORIGINAL LINE: void at_negedge(const sc_signal_in_if<bool>& s, sc_simcontext* simc = sc_get_curr_simcontext())
        public static void at_negedge(sc_signal_in_if<bool> s, sc_simcontext simc)
        {
            if (s.read() == false)
                do
                {
                    sc_event.wait(simc);
                } while (s.read() == false);
            do
            {
                Sc_event.wait(simc);
            } while (s.read() == true);
        }
        public static void at_negedge(sc_signal_in_if<sc_dt.sc_logic> s)
        {
            at_negedge(s, sc_get_curr_simcontext());
        }

        public static void at_negedge(sc_signal_in_if<sc_dt.sc_logic> s, sc_simcontext simc)
        {
            if (s.read() == '0')
                do
                {
                    sc_event.wait(simc);
                } while (s.read() == '0');
            do
            {
                sc_event.wait(simc);
            } while (s.read() == '1');
        }
        */
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------

    }

} // namespace sc_core
