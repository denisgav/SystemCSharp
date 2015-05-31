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
using System.Collections.Generic;
using System;

namespace sc_core
{
    // Standard process types:

    public enum sc_curr_proc_kind
    {
        SC_NO_PROC_,
        SC_METHOD_PROC_,
        SC_THREAD_PROC_,
        SC_CTHREAD_PROC_
    }

    // Descendant information for process hierarchy operations:

    public enum sc_descendant_inclusion_info
    {
        SC_NO_DESCENDANTS = 0,
        SC_INCLUDE_DESCENDANTS,
        SC_INVALID_DESCENDANTS
    }

    //==============================================================================
    // CLASS sc_process_host
    //
    // This is the base class for objects which may have processes defined for
    // their methods (e.g., sc_module)
    //==============================================================================

    public interface sc_process_host : IDisposable
    {
        void defunct();
    }


    //==============================================================================
    // CLASS sc_process_monitor
    //
    // This class provides a way of monitoring a process' status (e.g., waiting 
    // for a thread to complete its execution.) This class is intended to be a base
    // class for classes which need to monitor a process or processes (e.g.,
    // sc_join.) Its methods should be overloaded where notifications are desired.
    //==============================================================================

    public class sc_process_monitor
    {
        //C++ TO C# CONVERTER NOTE: Enums must be named in C#, so the following enum has been named AnonymousEnum:
        public enum AnonymousEnum
        {
            spm_exit = 0
        }
        public virtual void Dispose()
        {
        }
        public virtual void signal(sc_thread_process thread_p, int T)
        {
        }
    }

    public class sc_process_call_base
    {
        public sc_process_call_base()
        {
        }

        public virtual void Dispose()
        {
        }

        public virtual void invoke(sc_process_host host_p)
        {
        }
    }

    public class sc_process_call<T> : sc_process_call_base
    {
        public delegate void method_pDelegate();
        public sc_process_call(method_pDelegate method_p)
            : base()
        {
            this.m_method_p = method_p;
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public override void invoke(sc_process_host host_p)
        {
            m_method_p();
        }

        protected method_pDelegate m_method_p;
    }
    
    public enum process_throw_type
    {
        THROW_NONE = 0,
        THROW_KILL,
        THROW_USER,
        THROW_ASYNC_RESET,
        THROW_SYNC_RESET
    }

    public enum process_state
    {
        ps_bit_disabled = 1, // process is disabled.
        ps_bit_ready_to_run = 2, // process is ready to run.
        ps_bit_suspended = 4, // process is suspended.
        ps_bit_zombie = 8, // process is a zombie.
        ps_normal = 0 // must be zero.
    }

    public enum reset_type // types for sc_process_b::reset_process()
    {
        reset_asynchronous = 0, // asynchronous reset.
        reset_synchronous_off, // turn off synchronous reset sticky bit.
        reset_synchronous_on // turn on synchronous reset sticky bit.
    }

    public enum trigger_t
    {
        STATIC,
        EVENT,
        OR_LIST,
        AND_LIST,
        TIMEOUT,
        EVENT_TIMEOUT,
        OR_LIST_TIMEOUT,
        AND_LIST_TIMEOUT
    }

    public class scoped_flag
    {
        public scoped_flag(bool b)
        {
            __ref = b;
        }
        public void Dispose()
        {
            __ref = false;
        }
        public bool __ref;
    }

    public abstract class sc_process_b : sc_object
    {

        public static List<sc_event> empty_event_vector = new List<sc_event>();
        public static List<sc_object> empty_object_vector = new List<sc_object>();
        public static sc_process_b m_last_created_process_p = null;

        private static sc_process_b sc_process_b_m_last_created_process_p;


        public sc_process_b(string name_p, bool is_thread, bool free_host, sc_process_call_base method_p, sc_process_host host_p, sc_spawn_options opt_p)
            : base(name_p)
        {
            file = string.Empty;
            lineno = 0;
            proc_id = simcontext().next_proc_id();
            m_active_areset_n = 0;
            m_active_reset_n = 0;
            m_dont_init = false;
            m_dynamic_proc = simcontext().elaboration_done();
            m_event_p = null;
            m_event_count = 0;
            m_event_list_p = null;
            m_exist_p = null;
            m_free_host = free_host;
            m_has_reset_signal = false;
            m_has_stack = false;
            m_is_thread = is_thread;
            m_last_report_p = null;
            m_name_gen_p = null;
            m_process_kind = sc_curr_proc_kind.SC_NO_PROC_;
            m_references_n = 1;
            //\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\
            //m_resets = new List();
            //\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\
            m_reset_event_p = null;
            m_resume_event_p = null;
            m_runnable_p = null;
            m_semantics_host_p = host_p;
            m_semantics_method_p = method_p;
            m_state = (int)process_state.ps_normal;
            m_static_events = new List<sc_event>();
            m_sticky_reset = false;
            m_term_event_p = null;
            //\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\
            //m_throw_helper_p = 0;
            //\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\
            m_throw_status = process_throw_type.THROW_NONE;
            m_timed_out = false;
            m_timeout_event_p = null;
            m_trigger_type = trigger_t.STATIC;
            m_unwinding = false;
            m_last_created_process_p = this;
            m_timeout_event_p = new sc_event((sc_constants.SC_KERNEL_EVENT_PREFIX + "_free_event"));
        }


        public int current_state()
        {
            return m_state;
        }

        public bool dont_initialize()
        {
            return m_dont_init;
        }
        public virtual void dont_initialize(bool dont)
        {
            m_dont_init = dont;
        }

        public string dump_state()
        {
            string result;
            result = "[";
            if (m_state == (int)process_state.ps_normal)
            {
                result += " normal";
            }
            else
            {
                if ((m_state & (int)process_state.ps_bit_disabled) != 0)
                    result += "disabled ";
                if ((m_state & (int)process_state.ps_bit_suspended) != 0)
                    result += "suspended ";
                if ((m_state & (int)process_state.ps_bit_ready_to_run) != 0)
                    result += "ready_to_run ";
                if ((m_state & (int)process_state.ps_bit_zombie) != 0)
                    result += "zombie ";
            }
            result += "]";
            return result;
        }

        public override List<sc_object> get_child_objects()
        {
            return m_child_objects;
        }

        public sc_curr_proc_kind proc_kind()
        {
            return m_process_kind;
        }

        public sc_event reset_event()
        {
            if (m_reset_event_p == null)
            {
                m_reset_event_p = new sc_event(((string)sc_constants.SC_KERNEL_EVENT_PREFIX + "_reset_event"));
            }
            return m_reset_event_p;
        }
        public sc_event terminated_event()
        {
            if (m_term_event_p == null)
            {
                m_term_event_p = new sc_event(((string)sc_constants.SC_KERNEL_EVENT_PREFIX + "_term_event"));
            }
            return m_term_event_p;
        }

        public override void add_child_object(sc_object object_p)
        {
            base.add_child_object(object_p);
            reference_increment();
        }
        public virtual void add_static_event(sc_event e)
        {
            sc_method_process method_h;
            sc_thread_process thread_h;


            // CHECK TO SEE IF WE ARE ALREADY REGISTERED WITH THE EVENT:

            for (int i = m_static_events.Count - 1; i >= 0; --i)
            {
                if (e == m_static_events[i])
                {
                    return;
                }
            }

            // REMEMBER THE EVENT AND THEN REGISTER OUR OBJECT INSTANCE WITH IT:

            m_static_events.Add(e);

            switch (m_process_kind)
            {
                case sc_curr_proc_kind.SC_THREAD_PROC_:
                case sc_curr_proc_kind.SC_CTHREAD_PROC_:
                    thread_h = (sc_thread_process)(this);
                    e.add_static(thread_h);
                    break;
                case sc_curr_proc_kind.SC_METHOD_PROC_:
                    method_h = (sc_method_process)(this);
                    e.add_static(method_h);
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }
        }

        public virtual bool dynamic()
        {
            return m_dynamic_proc;
        }

        public string gen_unique_name(string basename_, bool preserve_first)
        {
            if (m_name_gen_p == null)
                m_name_gen_p = new sc_name_gen();
            return m_name_gen_p.gen_unique_name(basename_, preserve_first);
        }

        public virtual sc_report get_last_report()
        {
            return m_last_report_p;
        }


        protected bool is_disabled()
        {
            return (m_state & (int)process_state.ps_bit_disabled) != 0;
        }

        protected bool is_runnable()
        {
            return m_runnable_p != null;
        }

        protected static sc_process_b last_created_process_base()
        {
            return m_last_created_process_p;
        }
        protected override bool remove_child_object(sc_object object_p)
        {
            if (base.remove_child_object(object_p))
            {
                reference_decrement();
                return true;
            }
            else
            {
                return false;
            }
        }
        public virtual void remove_dynamic_events()
        {
            remove_dynamic_events(false);
        }

        public virtual void remove_dynamic_events(bool skip_timeout)
        {
            sc_method_process method_h;
            sc_thread_process thread_h;

            m_trigger_type = trigger_t.STATIC;
            switch (m_process_kind)
            {
                case sc_curr_proc_kind.SC_THREAD_PROC_:
                case sc_curr_proc_kind.SC_CTHREAD_PROC_:
                    thread_h = (sc_thread_process)(this);
                    if ((thread_h.m_timeout_event_p != null) && !skip_timeout)
                    {
                        thread_h.m_timeout_event_p.remove_dynamic(thread_h);
                        thread_h.m_timeout_event_p.cancel();
                    }
                    if (m_event_p != null)
                        m_event_p.remove_dynamic(thread_h);
                    if (m_event_list_p != null)
                    {
                        m_event_list_p.remove_dynamic(thread_h, null);
                        m_event_list_p.auto_delete();
                        m_event_list_p = null;
                    }
                    break;
                case sc_curr_proc_kind.SC_METHOD_PROC_:
                    method_h = (sc_method_process)(this);
                    if ((method_h.m_timeout_event_p != null) && !skip_timeout)
                    {
                        method_h.m_timeout_event_p.remove_dynamic(method_h);
                        method_h.m_timeout_event_p.cancel();
                    }
                    if (m_event_p != null)
                        m_event_p.remove_dynamic(method_h);
                    if (m_event_list_p != null)
                    {
                        m_event_list_p.remove_dynamic(method_h, null);
                        m_event_list_p.auto_delete();
                        m_event_list_p = null;
                    }
                    break;
                default: // Some other type, it needs to clean up itself.
                    break;
            }
        }
        public virtual void remove_static_events()
        {
            sc_method_process method_h;
            sc_thread_process thread_h;

            switch (m_process_kind)
            {
                case sc_curr_proc_kind.SC_THREAD_PROC_:
                case sc_curr_proc_kind.SC_CTHREAD_PROC_:
                    thread_h = (sc_thread_process)(this);
                    for (int i = m_static_events.Count - 1; i >= 0; --i)
                    {
                        m_static_events[i].remove_static(thread_h);
                    }
                    m_static_events.Clear();
                    break;
                case sc_curr_proc_kind.SC_METHOD_PROC_:
                    method_h = this as sc_method_process;
                    Debug.Assert(method_h != null);
                    for (int i = m_static_events.Count - 1; i >= 0; --i)
                    {
                        m_static_events[i].remove_static(method_h);
                    }
                    m_static_events.Clear();
                    break;
                default: // Some other type, it needs to clean up itself.
                    break;
            }
        }
        public virtual void set_last_report(sc_report last_p)
        {
            /*
            if (m_last_report_p != null)
                m_last_report_p.Dispose();
            */
            m_last_report_p = last_p;
        }
        public virtual bool timed_out()
        {
            return m_timed_out;
        }
        public virtual void report_error(string msgid)
        {
            report_error(msgid, "");
        }
        public virtual void report_error(string msgid, string msg)
        {
            string message = string.Format("{0}: {1}", msg, name());
            sc_report_handler.report(sc_core.sc_severity.SC_ERROR, msgid, message);
        }

        public virtual void disable_process()
        {
            disable_process(sc_descendant_inclusion_info.SC_NO_DESCENDANTS);
        }


        public virtual void disable_process(sc_descendant_inclusion_info descendants)
        {}

        protected void disconnect_process()
        {
            int mon_n; // monitor queue size.
            sc_thread_process thread_h;

            // IF THIS OBJECT IS PINING FOR THE FJORDS WE ARE DONE:

            if ((m_state & (int)process_state.ps_bit_zombie) != 0)
                return;

            // IF THIS IS A THREAD SIGNAL ANY MONITORS WAITING FOR IT TO EXIT:

            switch (m_process_kind)
            {
                case sc_curr_proc_kind.SC_THREAD_PROC_:
                case sc_curr_proc_kind.SC_CTHREAD_PROC_:
                    thread_h = (sc_thread_process)(this);
                    mon_n = thread_h.m_monitor_q.Count;
                    if (mon_n != 0)
                    {
                        for (int mon_i = 0; mon_i < mon_n; mon_i++)
                        {
                            thread_h.m_monitor_q[mon_i].signal(thread_h, (int)sc_process_monitor.AnonymousEnum.spm_exit);
                        }
                    }
                    break;
                default:
                    break;
            }

            // REMOVE EVENT WAITS, AND REMOVE THE PROCESS FROM ITS SC_RESET:

            remove_dynamic_events();
            remove_static_events();
            //---------------------------------------------------------------------------------
            //\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\
            /*
            for (List<sc_reset>.size_type rst_i = 0; rst_i < m_resets.Count; rst_i++)
            {
                m_resets[rst_i].remove_process(this);
            }
            m_resets.resize(0);
            */
            //\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\
            //---------------------------------------------------------------------------------

            // FIRE THE TERMINATION EVENT, MARK AS TERMINATED, AND DECREMENT THE COUNT:
            //
            // (1) We wait to set the process kind until after doing the removals
            //     above.
            // (2) Decrementing the reference count will result in actual object
            //     deletion if we hit zero.

            m_state = (int)process_state.ps_bit_zombie;
            if (m_term_event_p != null)
                m_term_event_p.notify();
            reference_decrement();
        }
        public virtual void enable_process()
        {
            enable_process(sc_descendant_inclusion_info.SC_NO_DESCENDANTS);
        }

        public abstract void enable_process(sc_descendant_inclusion_info descendants);
        protected void initially_in_reset(bool async)
        {
            if (async)
                m_active_areset_n++;
            else
                m_active_reset_n++;
        }

        public virtual bool is_unwinding()
        {
            return m_unwinding;
        }
        public virtual bool start_unwinding()
        {
            if (!m_unwinding)
            {
                switch (m_throw_status)
                {
                    case process_throw_type.THROW_KILL:
                    case process_throw_type.THROW_ASYNC_RESET:
                    case process_throw_type.THROW_SYNC_RESET:
                        m_unwinding = true;
                        return true;
                    case process_throw_type.THROW_USER:
                    default:
                        break;
                }
            }
            return false;
        }
        public virtual bool clear_unwinding()
        {
            m_unwinding = false;
            return true;
        }

        public virtual void kill_process()
        {
            kill_process(sc_descendant_inclusion_info.SC_NO_DESCENDANTS);
        }

        public abstract void kill_process(sc_descendant_inclusion_info descendants);

        public virtual void reset_changed(bool async, bool asserted)
        {

            // Error out on the corner case:

            if (!sc_simcontext.sc_allow_process_control_corners && !async && ((m_state & (int)process_state.ps_bit_suspended) != 0))
            {
                report_error("Undefined process control interaction", "synchronous reset changed on a suspended process");
            }

            // IF THIS OBJECT IS PUSHING UP DAISIES WE ARE DONE:

            if ((m_state & (int)process_state.ps_bit_zombie) != 0)
                return;

            // Reset is being asserted:

            if (asserted)
            {
                // if ( m_reset_event_p ) m_reset_event_p->notify();
                if (async)
                {
                    m_active_areset_n++;
                    if (sc_simcontext.sc_is_running())
                        throw_reset(true);
                }
                else
                {
                    m_active_reset_n++;
                    if (sc_simcontext.sc_is_running())
                        throw_reset(false);
                }
            }

            // Reset is being deasserted:

            else
            {
                if (async)
                {
                    m_active_areset_n--;
                }
                else
                {
                    m_active_reset_n--;
                }
            }

            // Clear the throw type if there are no active resets.

            if ((m_throw_status == process_throw_type.THROW_SYNC_RESET || m_throw_status == process_throw_type.THROW_ASYNC_RESET) && m_active_areset_n == 0 && m_active_reset_n == 0 && !m_sticky_reset)
            {
                m_throw_status = process_throw_type.THROW_NONE;
            }
        }

        public virtual void throw_reset(bool p)
        {
            throw new NotImplementedException();
        }
        public virtual void reset_process(reset_type rt)
        {
            reset_process(rt, sc_descendant_inclusion_info.SC_NO_DESCENDANTS);
        }

        public virtual void reset_process(reset_type rt, sc_descendant_inclusion_info descendants)
        {

            // PROCESS THIS OBJECT INSTANCE'S DESCENDANTS IF REQUESTED TO:

            if (descendants == sc_descendant_inclusion_info.SC_INCLUDE_DESCENDANTS)
            {
                List<sc_object> children = get_child_objects();
                int child_n = children.Count;

                for (int child_i = 0; child_i < child_n; child_i++)
                {
                    sc_process_b child_p = children[child_i] as sc_process_b;
                    if (child_p != null)
                        child_p.reset_process(rt, descendants);
                }
            }

            // PROCESS THIS OBJECT INSTANCE:

            switch (rt)
            {
                // One-shot asynchronous reset: remove dynamic sensitivity and throw:
                //
                // If this is an sc_method only throw if it is active.

                case reset_type.reset_asynchronous:
                    if (sc_simcontext.sc_get_status() != sc_status.SC_RUNNING)
                    {
                        report_error("a process may not be asynchronously reset while the simulation is not running");
                    }
                    else
                    {
                        remove_dynamic_events();
                        throw_reset(true);
                    }
                    break;

                // Turn on sticky synchronous reset: use standard reset mechanism.

                case reset_type.reset_synchronous_on:
                    if (m_sticky_reset == false)
                    {
                        m_sticky_reset = true;
                        reset_changed(false, true);
                    }
                    break;

                // Turn off sticky synchronous reset: use standard reset mechanism.

                default:
                    if (m_sticky_reset == true)
                    {
                        m_sticky_reset = false;
                        reset_changed(false, false);
                    }
                    break;
            }
        }
        public virtual void resume_process()
        {
            resume_process(sc_descendant_inclusion_info.SC_NO_DESCENDANTS);
        }

        public abstract void resume_process(sc_descendant_inclusion_info descendants);
        public virtual void suspend_process()
        {
            suspend_process(sc_descendant_inclusion_info.SC_NO_DESCENDANTS);
        }

        public abstract void suspend_process(sc_descendant_inclusion_info descendants);

        public virtual bool terminated()
        {
            return (m_state & (int)process_state.ps_bit_zombie) != 0;
        }

        public virtual void trigger_reset_event()
        {
            if (m_reset_event_p != null)
                m_reset_event_p.notify();
        }

        public virtual void delete_process()
        {
            Debug.Assert(m_references_n == 0);

            // Immediate deletion:

            // Deferred deletion: note we set the reference count to one  for the call
            // to reference_decrement that occurs in sc_simcontext::crunch().


            m_references_n = 1;
            detach();
            simcontext().mark_to_collect_process(this);

        }

        public virtual void reference_decrement()
        {
            m_references_n--;
            if (m_references_n == 0)
                delete_process();
        }
        public virtual void reference_increment()
        {
            Debug.Assert(m_references_n != 0);
            m_references_n++;
        }

        protected void semantics()
        {

            // within this function, the process has a stack associated

            scoped_flag scoped_stack_flag = new scoped_flag(m_has_stack);

            Debug.Assert(m_process_kind != sc_curr_proc_kind.SC_NO_PROC_);

            // Determine the reset status of this object instance and potentially
            // trigger its notify event:

            // See if we need to trigger the notify event:

            if (m_reset_event_p != null && ((m_throw_status == process_throw_type.THROW_SYNC_RESET) || (m_throw_status == process_throw_type.THROW_ASYNC_RESET)))
            {
                trigger_reset_event();
            }

            // Set the new reset status of this object based on the reset counts:

            m_throw_status = m_active_areset_n != 0 ? process_throw_type.THROW_ASYNC_RESET : (m_active_reset_n != 0 ? process_throw_type.THROW_SYNC_RESET : process_throw_type.THROW_NONE);

            // Dispatch the actual semantics for the process:

            m_semantics_method_p.invoke(m_semantics_host_p);
        }

        // debugging stuff:

        public string file;
        public int lineno;
        public int proc_id;

        protected int m_active_areset_n; // number of aresets active.
        protected int m_active_reset_n; // number of resets active.
        protected bool m_dont_init; // true: no initialize call.
        protected bool m_dynamic_proc; // true: after elaboration.
        protected sc_event m_event_p; // Dynamic event waiting on.
        protected int m_event_count; // number of events.
        protected sc_event_list m_event_list_p; // event list waiting on.
        protected sc_process_b m_exist_p; // process existence link.
        protected bool m_free_host; // free sc_semantic_host_p.
        protected bool m_has_reset_signal; // has reset_signal_is.
        protected bool m_has_stack; // true is stack present.
        protected bool m_is_thread; // true if this is thread.
        protected sc_report m_last_report_p; // last report this process.
        protected sc_name_gen m_name_gen_p; // subprocess name generator
        protected sc_curr_proc_kind m_process_kind; // type of process.
        protected int m_references_n; // outstanding handles.
        //\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\
        //protected List<sc_reset> m_resets = new List<sc_reset>(); // resets for process.
        //\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\
        protected sc_event m_reset_event_p; // reset event.
        protected sc_event m_resume_event_p; // resume event.
        protected sc_process_b m_runnable_p; // sc_runnable link
        protected sc_process_host m_semantics_host_p; // host for semantics.
        protected sc_process_call_base m_semantics_method_p;
        public int m_state; // process state.
        public List<sc_event> m_static_events = new List<sc_event>(); // static events waiting on.
        protected bool m_sticky_reset; // see note 3 above.
        protected sc_event m_term_event_p; // terminated event.
        protected process_throw_type m_throw_status; // exception throwing status
        protected bool m_timed_out; // true if we timed out.
        protected sc_event m_timeout_event_p; // timeout event.
        protected trigger_t m_trigger_type; // type of trigger using.
        protected bool m_unwinding; // true if unwinding stack.

    }
}


