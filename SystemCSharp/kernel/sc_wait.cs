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
    public static class sc_wait
    {

        public static void wait()
        {
            wait(sc_simcontext.sc_get_curr_simcontext());
        }

        // static sensitivity for SC_THREADs and SC_CTHREADs

        public static void wait(sc_simcontext simc)
        {
            sc_curr_proc_info cpi = simc.get_curr_proc_info();
            switch (cpi.kind)
            {
                case sc_curr_proc_kind.SC_THREAD_PROC_:
                case sc_curr_proc_kind.SC_CTHREAD_PROC_:
                    {
                        (cpi.process_handle as sc_cthread_process).wait_cycles();
                        break;
                    }
                default:
                    {
                        sc_report_handler.report(sc_severity.SC_ERROR, "wait() is only allowed in SC_THREADs and SC_CTHREADs\n        in SC_METHODs use next_trigger() instead", "");
                        break;
                    }
            }
        }

        // dynamic sensitivity for SC_THREADs and SC_CTHREADs

        public static void wait(sc_event e)
        {
            wait(e, sc_simcontext.sc_get_curr_simcontext());
        }


        // dynamic sensitivity for SC_THREADs and SC_CTHREADs

        //C++ TO C# CONVERTER NOTE: Overloaded method(s) are created above to convert the following method having default parameters:
        //ORIGINAL LINE: void wait(const sc_event& e, sc_simcontext* simc = sc_get_curr_simcontext())
        public static void wait(sc_event e, sc_simcontext simc)
        {
            sc_curr_proc_info cpi = simc.get_curr_proc_info();
            switch (cpi.kind)
            {
                case sc_curr_proc_kind.SC_THREAD_PROC_:
                    {
                        (cpi.process_handle as sc_thread_process).wait(e);
                        break;
                    }
                case sc_curr_proc_kind.SC_CTHREAD_PROC_:
                    {
                        warn_cthread_wait();
                        sc_cthread_process cthread_h = cpi.process_handle as sc_cthread_process;
                        cthread_h.wait(e);
                        cthread_h.wait_cycles();
                        break;
                    }
                default:
                    global::sc_core.sc_report_handler.report(sc_severity.SC_ERROR, "wait() is only allowed in SC_THREADs and SC_CTHREADs\n        in SC_METHODs use next_trigger() instead", "");
                    break;
            }
        }
        public static void wait(sc_event_or_list el)
        {
            wait(el, sc_simcontext.sc_get_curr_simcontext());
        }

        public static void wait(sc_event_or_list el, sc_simcontext simc)
        {
            sc_curr_proc_info cpi = simc.get_curr_proc_info();
            switch (cpi.kind)
            {
                case sc_curr_proc_kind.SC_THREAD_PROC_:
                    {
                        (cpi.process_handle as sc_thread_process).wait(el);
                        break;
                    }
                case sc_curr_proc_kind.SC_CTHREAD_PROC_:
                    {
                        warn_cthread_wait();
                        sc_report_handler.report(sc_severity.SC_INFO, "/IEEE_Std_1666/deprecated", "wait(event_list) is deprecated for SC_CTHREAD, use SC_THREAD");
                        sc_cthread_process cthread_h = (cpi.process_handle) as sc_cthread_process;
                        cthread_h.wait(el);
                        cthread_h.wait_cycles();
                        break;
                    }
                default:
                    sc_report_handler.report(sc_severity.SC_ERROR, "/IEEE_Std_1666/deprecated", "\n        " + "in SC_METHODs use next_trigger() instead");
                    break;
            }
        }

        public static void wait(sc_event_and_list el)
        {
            wait(el, sc_simcontext.sc_get_curr_simcontext());
        }

        public static void wait(sc_event_and_list el, sc_simcontext simc)
        {
            sc_curr_proc_info cpi = simc.get_curr_proc_info();
            switch (cpi.kind)
            {
                case sc_curr_proc_kind.SC_THREAD_PROC_:
                    {
                        (cpi.process_handle as sc_thread_process).wait(el);
                        break;
                    }
                case sc_curr_proc_kind.SC_CTHREAD_PROC_:
                    {
                        warn_cthread_wait();
                        sc_report_handler.report(sc_severity.SC_INFO, "/IEEE_Std_1666/deprecated", "wait(event_list) is deprecated for SC_CTHREAD, use SC_THREAD");
                        sc_cthread_process cthread_h = (cpi.process_handle) as sc_cthread_process;
                        cthread_h.wait(el);
                        cthread_h.wait_cycles();
                        break;
                    }
                default:
                    sc_report_handler.report(sc_severity.SC_ERROR, "/IEEE_Std_1666/deprecated", "\n        " + "in SC_METHODs use next_trigger() instead");
                    break;
            }
        }

        public static void wait(double v, sc_time_unit tu)
        {
            wait(v, tu, sc_simcontext.sc_get_curr_simcontext());
        }

        public static void wait(double v, sc_time_unit tu, sc_simcontext simc)
        {
            wait(new sc_time(v, tu, simc), simc);
        }

        public static void wait(sc_time t)
        {
            wait(t, sc_simcontext.sc_get_curr_simcontext());
        }

        public static void wait(sc_time t, sc_simcontext simc)
        {
            sc_curr_proc_info cpi = simc.get_curr_proc_info();
            switch (cpi.kind)
            {
                case sc_curr_proc_kind.SC_THREAD_PROC_:
                    {
                        (cpi.process_handle as sc_thread_process).wait(t);
                        break;
                    }
                case sc_curr_proc_kind.SC_CTHREAD_PROC_:
                    {
                        warn_cthread_wait();
                        sc_cthread_process cthread_h =
                                (cpi.process_handle as sc_cthread_process);
                        cthread_h.wait(t);
                        cthread_h.wait_cycles();
                        break;
                    }
                default:
                    sc_report_handler.report(sc_severity.SC_ERROR, "wait() is only allowed in SC_THREADs and SC_CTHREADs", "\n        in SC_METHODs use next_trigger() instead");
                    break;
            }
        }

        public static void wait(sc_time t, sc_event e)
        {
            wait(t, e, sc_simcontext.sc_get_curr_simcontext());
        }

        public static void wait(sc_time t, sc_event e, sc_simcontext simc)
        {
            sc_curr_proc_info cpi = simc.get_curr_proc_info();
            switch (cpi.kind)
            {
                case sc_curr_proc_kind.SC_THREAD_PROC_:
                    {
                        (cpi.process_handle as sc_thread_process).wait(new sc_time(t), e);
                        break;
                    }
                case sc_curr_proc_kind.SC_CTHREAD_PROC_:
                    {
                        sc_wait.warn_cthread_wait();
                        sc_cthread_process cthread_h = (cpi.process_handle) as sc_cthread_process;
                        cthread_h.wait(new sc_time(t), e);
                        cthread_h.wait_cycles();
                        break;
                    }
                default:
                    {
                        sc_report_handler.report(sc_severity.SC_ERROR, "wait() is only allowed in SC_THREADs and SC_CTHREADs", "\n        in SC_METHODs use next_trigger() instead");
                        break;
                    }
            }
        }
        public static void wait(double v, sc_time_unit tu, sc_event e)
        {
            wait(v, tu, e, sc_simcontext.sc_get_curr_simcontext());
        }

        public static void wait(double v, sc_time_unit tu, sc_event e, sc_simcontext simc)
        {
            wait(new sc_time(v, tu, simc), e, simc);
        }
        public static void wait(sc_time t, sc_event_or_list el)
        {
            wait(t, el, sc_simcontext.sc_get_curr_simcontext());
        }

        public static void wait(sc_time t, sc_event_or_list el, sc_simcontext simc)
        {
            sc_curr_proc_info cpi = simc.get_curr_proc_info();
            switch (cpi.kind)
            {
                case sc_curr_proc_kind.SC_THREAD_PROC_:
                    {
                        (cpi.process_handle as sc_thread_process).wait(new sc_time(t), el);
                        break;
                    }
                case sc_curr_proc_kind.SC_CTHREAD_PROC_:
                    {
                        sc_wait.warn_cthread_wait();
                        sc_cthread_process cthread_h = (cpi.process_handle) as sc_cthread_process;
                        cthread_h.wait(new sc_time(t), el);
                        cthread_h.wait_cycles();
                        break;
                    }
                default:
                    sc_report_handler.report(sc_severity.SC_ERROR, "wait() is only allowed in SC_THREADs and SC_CTHREADs", "\n        " + "in SC_METHODs use next_trigger() instead");
                    break;
            }
        }
        public static void wait(double v, sc_time_unit tu, sc_event_or_list el)
        {
            wait(v, tu, el, sc_simcontext.sc_get_curr_simcontext());
        }

        public static void wait(double v, sc_time_unit tu, sc_event_or_list el, sc_simcontext simc)
        {
            wait(new sc_time(v, tu, simc), el, simc);
        }
        public static void wait(sc_time t, sc_event_and_list el)
        {
            wait(t, el, sc_simcontext.sc_get_curr_simcontext());
        }

        public static void wait(sc_time t, sc_event_and_list el, sc_simcontext simc)
        {
            sc_curr_proc_info cpi = simc.get_curr_proc_info();
            switch (cpi.kind)
            {
                case sc_curr_proc_kind.SC_THREAD_PROC_:
                    {
                        (cpi.process_handle as sc_thread_process).wait(new sc_time(t), el);
                        break;
                    }
                case sc_curr_proc_kind.SC_CTHREAD_PROC_:
                    {
                        sc_wait.warn_cthread_wait();
                        sc_cthread_process cthread_h = (cpi.process_handle) as sc_cthread_process;
                        cthread_h.wait(new sc_time(t), el);
                        cthread_h.wait_cycles();
                        break;
                    }
                default:
                    sc_report_handler.report(sc_severity.SC_ERROR, "wait() is only allowed in SC_THREADs and SC_CTHREADs", "\n        in SC_METHODs use next_trigger() instead");
                    break;
            }
        }
        public static void wait(double v, sc_time_unit tu, sc_event_and_list el)
        {
            wait(v, tu, el, sc_simcontext.sc_get_curr_simcontext());
        }

        public static void wait(double v, sc_time_unit tu, sc_event_and_list el, sc_simcontext simc)
        {
            wait(new sc_time(v, tu, simc), el, simc);
        }

        // static sensitivity for SC_METHODs

        public static void next_trigger()
        {
            next_trigger(sc_simcontext.sc_get_curr_simcontext());
        }


        // static sensitivity for SC_METHODs

        public static void next_trigger(sc_simcontext simc)
        {
            sc_curr_proc_info cpi = simc.get_curr_proc_info();
            if (cpi.kind == sc_curr_proc_kind.SC_METHOD_PROC_)
            {
                (cpi.process_handle as sc_method_process).clear_trigger();
            }
            else
            {
                sc_report_handler.report(sc_severity.SC_ERROR, "next_trigger() is only allowed in SC_METHODs", "\n        in SC_THREADs and SC_CTHREADs use wait() instead");
            }
        }

        // dynamic sensitivity for SC_METHODs

        public static void next_trigger(sc_event e)
        {
            next_trigger(e, sc_simcontext.sc_get_curr_simcontext());
        }


        // dynamic sensitivity for SC_METHODs

        //C++ TO C# CONVERTER NOTE: Overloaded method(s) are created above to convert the following method having default parameters:
        //ORIGINAL LINE: void next_trigger(const sc_event& e, sc_simcontext* simc = sc_get_curr_simcontext())
        public static void next_trigger(sc_event e, sc_simcontext simc)
        {
            sc_curr_proc_info cpi = simc.get_curr_proc_info();
            if (cpi.kind == sc_curr_proc_kind.SC_METHOD_PROC_)
            {
                (cpi.process_handle as sc_method_process).next_trigger(e);
            }
            else
            {
                sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "next_trigger() is only allowed in SC_METHODs", "\n        in SC_THREADs and SC_CTHREADs use wait() instead");
            }
        }
        public static void next_trigger(sc_event_or_list el)
        {
            next_trigger(el, sc_simcontext.sc_get_curr_simcontext());
        }

        public static void next_trigger(sc_event_or_list el, sc_simcontext simc)
        {
            sc_curr_proc_info cpi = simc.get_curr_proc_info();
            if (cpi.kind == sc_curr_proc_kind.SC_METHOD_PROC_)
            {
                (cpi.process_handle as sc_method_process).next_trigger(el);
            }
            else
            {
                sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "next_trigger() is only allowed in SC_METHODs", "\n        in SC_THREADs and SC_CTHREADs use wait() instead");
            }
        }
        public static void next_trigger(sc_event_and_list el)
        {
            next_trigger(el, sc_simcontext.sc_get_curr_simcontext());
        }

        public static void next_trigger(sc_event_and_list el, sc_simcontext simc)
        {
            sc_curr_proc_info cpi = simc.get_curr_proc_info();
            if (cpi.kind == sc_curr_proc_kind.SC_METHOD_PROC_)
            {
                (cpi.process_handle as sc_method_process).next_trigger(el);
            }
            else
            {
                sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "next_trigger() is only allowed in SC_METHODs", "\n        in SC_THREADs and SC_CTHREADs use wait() instead");
            }
        }
        public static void next_trigger(sc_time t)
        {
            next_trigger(t, sc_simcontext.sc_get_curr_simcontext());
        }

        public static void next_trigger(sc_time t, sc_simcontext simc)
        {
            sc_curr_proc_info cpi = simc.get_curr_proc_info();
            if (cpi.kind == sc_curr_proc_kind.SC_METHOD_PROC_)
            {
                (cpi.process_handle as sc_method_process).next_trigger(t);
            }
            else
            {
                sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "next_trigger() is only allowed in SC_METHODs", "\n        in SC_THREADs and SC_CTHREADs use wait() instead");
            }
        }
        public static void next_trigger(double v, sc_time_unit tu)
        {
            next_trigger(v, tu, sc_simcontext.sc_get_curr_simcontext());
        }

        public static void next_trigger(double v, sc_time_unit tu, sc_simcontext simc)
        {
            next_trigger(new sc_time(v, tu, simc), simc);
        }
        public static void next_trigger(sc_time t, sc_event e)
        {
            next_trigger(t, e, sc_simcontext.sc_get_curr_simcontext());
        }

        public static void next_trigger(sc_time t, sc_event e, sc_simcontext simc)
        {
            sc_curr_proc_info cpi = simc.get_curr_proc_info();
            if (cpi.kind == sc_curr_proc_kind.SC_METHOD_PROC_)
            {
                (cpi.process_handle as sc_method_process).next_trigger(t, e);
            }
            else
            {
                sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "next_trigger() is only allowed in SC_METHODs", "\n        in SC_THREADs and SC_CTHREADs use wait() instead");
            }
        }
        public static void next_trigger(double v, sc_time_unit tu, sc_event e)
        {
            next_trigger(v, tu, e, sc_simcontext.sc_get_curr_simcontext());
        }

        public static void next_trigger(double v, sc_time_unit tu, sc_event e, sc_simcontext simc)
        {
            next_trigger(new sc_time(v, tu, simc), e, simc);
        }
        public static void next_trigger(sc_time t, sc_event_or_list el)
        {
            next_trigger(t, el, sc_simcontext.sc_get_curr_simcontext());
        }

        //C++ TO C# CONVERTER NOTE: Overloaded method(s) are created above to convert the following method having default parameters:
        //ORIGINAL LINE: void next_trigger(const sc_time& t, const sc_event_or_list& el, sc_simcontext* simc = sc_get_curr_simcontext())
        public static void next_trigger(sc_time t, sc_event_or_list el, sc_simcontext simc)
        {
            sc_curr_proc_info cpi = simc.get_curr_proc_info();
            if (cpi.kind == sc_curr_proc_kind.SC_METHOD_PROC_)
            {
                (cpi.process_handle as sc_method_process).next_trigger(t, el);
            }
            else
            {
                sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "next_trigger() is only allowed in SC_METHODs", "\n        in SC_THREADs and SC_CTHREADs use wait() instead");
            }
        }
        public static void next_trigger(double v, sc_time_unit tu, sc_event_or_list el)
        {
            next_trigger(v, tu, el, sc_simcontext.sc_get_curr_simcontext());
        }

        public static void next_trigger(double v, sc_time_unit tu, sc_event_or_list el, sc_simcontext simc)
        {
            next_trigger(new sc_time(v, tu, simc), el, simc);
        }
        public static void next_trigger(sc_time t, sc_event_and_list el)
        {
            next_trigger(t, el, sc_simcontext.sc_get_curr_simcontext());
        }

        public static void next_trigger(sc_time t, sc_event_and_list el, sc_simcontext simc)
        {
            sc_curr_proc_info cpi = simc.get_curr_proc_info();
            if (cpi.kind == sc_curr_proc_kind.SC_METHOD_PROC_)
            {
                (cpi.process_handle as sc_method_process).next_trigger(t, el);
            }
            else
            {
                sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "next_trigger() is only allowed in SC_METHODs", "\n        in SC_THREADs and SC_CTHREADs use wait() instead");
            }
        }
        public static void next_trigger(double v, sc_time_unit tu, sc_event_and_list el)
        {
            next_trigger(v, tu, el, sc_simcontext.sc_get_curr_simcontext());
        }

        public static void next_trigger(double v, sc_time_unit tu, sc_event_and_list el, sc_simcontext simc)
        {
            next_trigger(new sc_time(v, tu, simc), el, simc);
        }

        // for SC_METHODs and SC_THREADs and SC_CTHREADs

        public static bool timed_out()
        {
            return timed_out(sc_simcontext.sc_get_curr_simcontext());
        }


        // for SC_METHODs and SC_THREADs and SC_CTHREADs

        public static bool timed_out(sc_simcontext simc)
        {
            sc_report_handler.report(sc_severity.SC_INFO, "/IEEE_Std_1666/deprecated", "timed_out() function is deprecated");

            sc_curr_proc_info cpi = simc.get_curr_proc_info();

            return cpi.process_handle.timed_out();
        }

        // misc.

        public static void sc_set_location(string file, int lineno)
        {
            sc_set_location(file, lineno, sc_simcontext.sc_get_curr_simcontext());
        }

        // misc.

        //C++ TO C# CONVERTER NOTE: Overloaded method(s) are created above to convert the following method having default parameters:
        //ORIGINAL LINE: void sc_set_location(string file, int lineno, sc_simcontext* simc = sc_get_curr_simcontext())
        public static void sc_set_location(string file, int lineno, sc_simcontext simc)
        {
            sc_curr_proc_info cpi = simc.get_curr_proc_info();
            sc_process_b handle = cpi.process_handle;
            handle.file = file;
            handle.lineno = lineno;
        }

        // static sensitivity for SC_THREADs and SC_CTHREADs

        public static void warn_cthread_wait()
        {
            sc_report_handler.report(sc_core.sc_severity.SC_INFO, "/IEEE_Std_1666/deprecated", "all waits except wait() and wait(N)\n             are deprecated for SC_CTHREAD, " + "use an SC_THREAD instead");
        }
    }

}