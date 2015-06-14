using System.Diagnostics;
using System.Collections.Generic;

//****************************************************************************
//
//  The following code is derived, directly or indirectly, from the SystemC
//  source code Copyright (c) 1996-2014 by all Contributors.
//  All Rights reserved.
//
//  The contents of this file are subject to the restrictions and limitations
//  set forth in the SystemC Open Source License (the "License");
//  You may not use this file except in compliance with such restrictions and
//  limitations. You may obtain instructions on how to receive a copy of the
//  License at http://www.accellera.org/. Software distributed by Contributors
//  under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF
//  ANY KIND, either express or implied. See the License for the specific
//  language governing rights and limitations under the License.
//
// ****************************************************************************
namespace sc_core
{

    //==============================================================================
    // CLASS sc_reset_target - RESET ENTRY FOR AN sc_process_b TARGET
    //
    // This class describes a reset condition associated with an sc_process_b
    // instance. 
    //==============================================================================
    public class sc_reset_target
    {
        public bool m_async; // true asynchronous reset, false synchronous.
        public bool m_level; // level for reset.
        public sc_process_b m_process_p;
    }

    //==============================================================================
    // CLASS sc_reset - RESET INFORMATION FOR A RESET SIGNAL
    //
    // See the top of sc_reset.cpp for an explaination of how the reset mechanism
    // is implemented.
    //==============================================================================
    public class sc_reset
    {
        internal static sc_reset_finder reset_finder_q = null; // Q of reset finders to reconcile.


        //------------------------------------------------------------------------------
        //"sc_reset::reconcile_resets"
        //
        // This static method processes the sc_reset_finders to establish the actual
        // reset connections.
        //
        // Notes:
        //   (1) If reset is asserted we tell the process that it is in reset.
        //------------------------------------------------------------------------------
        /*
        protected static void reconcile_resets()
        {
            sc_signal_in_if_param<bool> iface_p; // Interface to reset signal.
            sc_reset_finder next_p; // Next finder to process.
            sc_reset_finder now_p; // Finder currently processing.
            sc_reset_target reset_target = new sc_reset_target(); // Target's reset entry.
            sc_reset reset_p; // Reset object to use.

            for (now_p = reset_finder_q; now_p!=null; now_p = next_p)
            {
                next_p = now_p.m_next_p;
                if (now_p.m_in_p != null)
                {
                    iface_p = now_p.m_in_p.get_interface() as sc_signal_in_if_param<bool>;
                }
                else if (now_p.m_inout_p != null)
                {
                    iface_p = now_p.m_inout_p.get_interface() as sc_signal_in_if_param<bool>;
                }
                else
                {
                    iface_p = now_p.m_out_p.get_interface() as sc_signal_in_if_param<bool>;
                }
                Debug.Assert(iface_p != null);
                reset_p = iface_p.is_reset();
                now_p.m_target_p.m_resets.Add(reset_p);
                reset_target.m_async = now_p.m_async;
                reset_target.m_level = now_p.m_level;
                reset_target.m_process_p = now_p.m_target_p;
                reset_p.m_targets.Add(reset_target);
                if (iface_p.read() == now_p.m_level) // see note 1 above
                    now_p.m_target_p.initially_in_reset(now_p.m_async);
                if (now_p != null)
                    now_p.Dispose();
            }
        }
        */
		/*
        //------------------------------------------------------------------------------
        //"sc_reset::reset_signal_is - ports"
        //
        // These overloads of the reset_signal_is() method will register the active
        // process with the sc_reset object instance associated with the supplied port.
        // If the port does not yet have a pointer to its sc_signal<bool> instance it
        // will create an sc_reset_finder class object instance that will be used
        // to set the process' reset information when the port has been bound.
        //
        // Arguments:
        //     async = true if the reset signal is asynchronous, false if not.
        //     port  = port for sc_signal<bool> that will provide the reset signal.
        //     level = level at which reset is active, either true or false.
        //------------------------------------------------------------------------------
        protected static void reset_signal_is(bool async, sc_in<bool> port, bool level)
        {
            sc_signal_in_if_param<bool> iface_p;
            sc_process_handle process_p = (sc_process_b)sc_simcontext.sc_get_current_process_handle();

            Debug.Assert(process_p != null);
            process_p.m_has_reset_signal = true;
            switch (process_p.proc_kind())
            {
                case sc_curr_proc_kind.SC_THREAD_PROC_:
                case sc_curr_proc_kind.SC_METHOD_PROC_:
                case sc_curr_proc_kind.SC_CTHREAD_PROC_:
                    iface_p = port.get_interface() as sc_signal_in_if_param<bool>;
                    if (iface_p != null)
                        reset_signal_is(async, iface_p, level);
                    else
                        new sc_reset_finder(async, port, level, process_p);
                    break;
                default:
                    sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "Unknown process type", process_p.name());
                    break;
            }
        }
        protected static void reset_signal_is(bool async, sc_inout<bool> port, bool level)
        {
            sc_signal_in_if_param<bool> iface_p;
            sc_process_handle process_p = (sc_process_b)GlobalMembersSc_simcontext.sc_get_current_process_handle();

            Debug.Assert(process_p != null);
            process_p.m_has_reset_signal = true;
            switch (process_p.proc_kind())
            {
                case sc_curr_proc_kind.SC_THREAD_PROC_:
                case sc_curr_proc_kind.SC_METHOD_PROC_:
                case sc_curr_proc_kind.SC_CTHREAD_PROC_:
                    iface_p = port.get_interface() as sc_signal_in_if_param<bool>;
                    if (iface_p != null)
                        reset_signal_is(async, iface_p, level);
                    else
                        new sc_reset_finder(async, port, level, process_p);
                    break;
                default:
                    sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "Unknown process type", process_p.name());
                    break;
            }
        }
        protected static void reset_signal_is(bool async, sc_out<bool> port, bool level)
        {
            sc_signal_in_if_param<bool> iface_p;
            sc_process_handle process_p = (sc_process_b)sc_simcontext.sc_get_current_process_handle();

            Debug.Assert(process_p != null);
            process_p.m_has_reset_signal = true;
            switch (process_p.proc_kind())
            {
                case sc_curr_proc_kind.SC_THREAD_PROC_:
                case sc_curr_proc_kind.SC_METHOD_PROC_:
                case sc_curr_proc_kind.SC_CTHREAD_PROC_:
                    iface_p = port.get_interface() as sc_signal_in_if_param<bool>;
                    if (iface_p != null)
                        reset_signal_is(async, iface_p, level);
                    else
                        new sc_reset_finder(async, port, level, process_p);
                    break;
                default:
                    sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "Unknown process type", process_p.name());
                    break;
            }
        }
		*/
        //------------------------------------------------------------------------------
        //"sc_reset::reset_signal_is"
        //
        // This static method will register the active process instance as being
        // reset by the sc_signal<bool> whose interface has been supplied. If no
        // sc_reset object instance has been attached to the sc_signal<bool> yet, it
        // will be created and attached. The active process instance is pushed into
        // the list of processes that the sc_reset object instance should notify if
        // the value of the reset signal changes.
        //
        // Arguments:
        //     async = true if the reset signal is asynchronous, false if not.
        //     iface = interface for the reset signal.
        //     level = is the level at which reset is active, either true or false.
        // Notes:
        //   (1) If reset is asserted we tell the process that it is in reset
        //       initially.
        //------------------------------------------------------------------------------
        protected static void reset_signal_is(bool async, sc_signal_in_if_param<bool> iface, bool level)
        {
            sc_process_b process_p;

            sc_reset_target reset_target = new sc_reset_target(); // entry to build for the process.
            ///*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/
            //sc_reset reset_p; // reset object.
            ///*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/

            process_p = sc_process_b.last_created_process_base();
            Debug.Assert(process_p != null);
            process_p.m_has_reset_signal = true;
            switch (process_p.proc_kind())
            {
                case sc_curr_proc_kind.SC_METHOD_PROC_:
                case sc_curr_proc_kind.SC_CTHREAD_PROC_:
                case sc_curr_proc_kind.SC_THREAD_PROC_:
                    ///*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/
                    //reset_p = iface.is_reset();
                    //process_p.m_resets.Add(reset_p);
                    ///*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/
                    reset_target.m_async = async;
                    reset_target.m_level = level;
                    reset_target.m_process_p = process_p;
                    ///*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/
                    //reset_p.m_targets.Add(reset_target);
                    ///*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/
                    if (iface.read() == level)
                        process_p.initially_in_reset(async);
                    break;
                default:
                    sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "Unknown process type", process_p.name());
                    break;
            }
        }

        protected sc_reset(sc_signal_in_if_param<bool> iface_p)
        {
            m_iface_p = iface_p;
            m_targets = new List<sc_reset_target>();
        }

        //------------------------------------------------------------------------------
        //"sc_reset::notify_processes"
        //
        // Notify processes that there is a change in the reset signal value.
        //------------------------------------------------------------------------------
        protected void notify_processes()
        {
            bool active; // true if reset is active.
            sc_reset_target entry_p; // reset entry processing.
            bool value = m_iface_p.read(); // value of our signal.

            foreach (sc_reset_target process_i in m_targets)
            {
                entry_p = process_i;
                active = (entry_p.m_level == value);
                entry_p.m_process_p.reset_changed(entry_p.m_async, active);
            }
        }
        public void remove_process(sc_process_b process_p)
        {
            int process_i; // Index of process resetting.
            int process_n; // # of processes to reset.

            process_n = m_targets.Count;
            for (process_i = 0; process_i < process_n; )
            {
                if (m_targets[process_i].m_process_p == process_p)
                {
                    m_targets.RemoveAt(process_i);
                }
                else
                {
                    process_i++;
                }
            }
        }

        protected readonly sc_signal_in_if_param<bool> m_iface_p; // Interface to read.
        protected List<sc_reset_target> m_targets = new List<sc_reset_target>(); // List of processes to reset.

    }


    //==============================================================================
    // sc_reset_finder - place holder class for a port reset signal until it is
    //                   bound and an interface class is available. When the port
    //                   has been bound the information in this class will be used
    //                   to initialize its sc_reset object instance.
    //==============================================================================
    public class sc_reset_finder
    {
        //public sc_reset_finder(bool async, sc_in<bool> port_p, bool level, sc_process_b target_p)
        //{
        //    m_async = async;
        //    m_level = level;
        //    m_next_p = null;
        //    m_in_p = port_p;
        //    m_inout_p = 0;
        //    m_out_p = 0;
        //    m_target_p = target_p;
        //
        //    m_next_p = reset_finder_q;
        //    reset_finder_q = this;
        //}
        //public sc_reset_finder(bool async, sc_inout<bool> port_p, bool level, sc_process_b target_p)
        //{
        //    m_async = async;
        //    m_level = level;
        //    m_next_p = 0;
        //    m_in_p = 0;
        //    m_inout_p = port_p;
        //    m_out_p = 0;
        //    m_target_p = target_p;
        //
        //    m_next_p = sc_reset.reset_finder_q;
        //    sc_reset.reset_finder_q = this;
        //}

        public bool m_async; // True if asynchronous reset.
        public bool m_level; // Level for reset.
        public sc_reset_finder m_next_p; // Next reset finder in list.
		
        //sc_in<bool> m_in_p; // Port for which reset is needed.
        //sc_inout<bool> m_inout_p; // Port for which reset is needed.
        //sc_out<bool> m_out_p; // Port for which reset is needed.

        public sc_process_b m_target_p;
    }
}