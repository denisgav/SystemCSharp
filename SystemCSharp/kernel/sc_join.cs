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


using System.Diagnostics;


namespace sc_core
{

    public class sc_join : sc_process_monitor
    {
        //------------------------------------------------------------------------------
        //"sc_join::sc_join"
        //
        // This is the object instance constructor for this class.
        //------------------------------------------------------------------------------
        public sc_join()
        {
            m_join_event = new sc_event(((string)sc_constants.SC_KERNEL_EVENT_PREFIX + "_join_event"));
            m_threads_n = 0;
        }

        //------------------------------------------------------------------------------
        //"sc_join::add_process - sc_process_handle"
        //
        // This method adds a process to this join object instance. This consists of
        // incrementing the count of processes in the join process and adding this 
        // object instance to the supplied thread's monitoring queue.
        //     process_h = handle for process to be monitored.
        //------------------------------------------------------------------------------
        public void add_process(sc_process_handle process_h)
        {
            sc_thread_process thread_p; // Thread within process_h.

            thread_p = process_h.get_process_object() as sc_thread_process;
            if (thread_p != null)
            {
                m_threads_n++;
                thread_p.add_monitor(this);
            }
            else
            {
                sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "Attempt to register method process with sc_join object", "");
            }
        }
        public int process_count()
        {
            return m_threads_n;
        }

        public override void signal(sc_thread_process thread_p, int T)
        {
            if (T == (int)sc_process_monitor.AnonymousEnum.spm_exit)
            {
                thread_p.remove_monitor(this);
                if (--m_threads_n == 0)
                    m_join_event.notify();
            }
        }

        public void wait()
        {
            sc_wait.wait(m_join_event);
        }

        // suspend a thread that has a sensitivity list:

        public void wait_clocked()
        {
            do
            {
                sc_wait.wait();
            } while (m_threads_n != 0);
        }


        //------------------------------------------------------------------------------
        //"sc_join::add_process - sc_process_b*"
        //
        // This method adds a process to this join object instance. This consists of
        // incrementing the count of processes in the join process and adding this 
        // object instance to the supplied thread's monitoring queue.
        //     process_p -> thread to be monitored.
        //------------------------------------------------------------------------------
        protected void add_process(sc_process_b process_p)
        {
            sc_thread_process handle = process_p as sc_thread_process;
            Debug.Assert(handle != null);
            m_threads_n++;
            handle.add_monitor(this);
        }

        protected sc_event m_join_event = new sc_event(); // Event to notify when all threads have reported.
        protected int m_threads_n; // # of threads still need to wait for.
    }
}