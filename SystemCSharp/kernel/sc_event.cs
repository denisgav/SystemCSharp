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


using System.Collections.Generic;
using System;
using System.Diagnostics;
namespace sc_core
{
    public class sc_event
    {
        private enum notify_t
        {
            NONE,
            DELTA,
            TIMED
        }

        private string m_name; // name of object.
        private sc_object m_parent_p; // parent sc_object for this event.
        private sc_simcontext m_simc;
        private notify_t m_notify_type;
        public int m_delta_event_index;
        private sc_event_timed m_timed;

        private List<sc_method_process> m_methods_static = new List<sc_method_process>();
        private List<sc_method_process> m_methods_dynamic = new List<sc_method_process>();
        private List<sc_thread_process> m_threads_static = new List<sc_thread_process>();
        private List<sc_thread_process> m_threads_dynamic = new List<sc_thread_process>();


        // +----------------------------------------------------------------------------
        // |"sc_event::sc_event(name)"
        // | 
        // | This is the object instance constructor for non-named sc_event instances.
        // | If this is during elaboration add create a name and add it to the object
        // | hierarchy.
        // +----------------------------------------------------------------------------
        public sc_event()
        {
            m_name = string.Empty;
            m_parent_p = null;
            m_simc = sc_simcontext.sc_get_curr_simcontext();
            m_notify_type = notify_t.NONE;
            m_delta_event_index = -1;
            m_timed = null;
            m_methods_static = new List<sc_method_process>();
            m_methods_dynamic = new List<sc_method_process>();
            m_threads_static = new List<sc_thread_process>();
            m_threads_dynamic = new List<sc_thread_process>();
            register_event("");
        }

        // +----------------------------------------------------------------------------
        // |"sc_event::sc_event(name)"
        // | 
        // | This is the object instance constructor for named sc_event instances.
        // | If the name is non-null or the this is during elaboration add the
        // | event to the object hierarchy.
        // |
        // | Arguments:
        // |     name = name of the event.
        // +----------------------------------------------------------------------------
        public sc_event(string name)
        {
            m_name = name;
            m_parent_p = null;
            m_simc = sc_simcontext.sc_get_curr_simcontext();
            m_notify_type = notify_t.NONE;
            m_delta_event_index = -1;
            m_timed = null;
            m_methods_static = new List<sc_method_process>();
            m_methods_dynamic = new List<sc_method_process>();
            m_threads_static = new List<sc_thread_process>();
            m_threads_dynamic = new List<sc_thread_process>();
            // Skip simulator's internally defined events.
            register_event(name);
        }

        // +----------------------------------------------------------------------------
        // |"sc_event::~sc_event"
        // | 
        // | This is the object instance destructor for this class. It cancels any
        // | outstanding waits and removes the event from the object manager's 
        // | instance table if it has a name.
        // +----------------------------------------------------------------------------
        public virtual void Dispose()
        {
            cancel();
            if (m_name.Length != 0)
            {
                sc_object_manager object_manager_p = m_simc.get_object_manager();
                object_manager_p.remove_event(m_name);
            }
        }

        public virtual void cancel()
        {
            // cancel a delta or timed notification
            switch (m_notify_type)
            {
                case notify_t.DELTA:
                    {
                        // remove this event from the delta events set
                        m_simc.remove_delta_event(this);
                        m_notify_type = notify_t.NONE;
                        break;
                    }
                case notify_t.TIMED:
                    {
                        // remove this event from the timed events set
                        Debug.Assert(m_timed != null);
                        m_timed.m_event = null;
                        m_timed = null;
                        m_notify_type = notify_t.NONE;
                        break;
                    }
                default:
                    ;
                    break;
            }
        }

        public virtual string name()
        {
            return m_name;
        }

        // ----------------------------------------------------------------------------
        //  CLASS : sc_event
        //
        //  The event class.
        // ----------------------------------------------------------------------------
        public virtual string basename()
        {
            string[] name_parts = m_name.Split(new char[] { sc_object.SC_HIERARCHY_CHAR }, StringSplitOptions.RemoveEmptyEntries);
            if (name_parts.Length >= 2)
                return name_parts[name_parts.Length - 2];
            else
                return name_parts[name_parts.Length - 1];
        }

        public virtual void set_parent_object(sc_object o)
        {
            m_parent_p = o;
        }

        public virtual sc_object get_parent_object()
        {
            return m_parent_p;
        }


        public virtual bool in_hierarchy()
        {
            return m_name.Length != 0;
        }

        public virtual void notify()
        {
            // immediate notification
            // coming from sc_prim_channel::update
            // coming from phase callbacks
            if (m_simc.update_phase())
            {
                sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "immediate notification is not allowed during the update phase", "");
                return;
            }
            cancel();
            trigger();
        }

        public virtual void notify(sc_time t)
        {
            if (m_notify_type == notify_t.DELTA)
            {
                return;
            }
            if (t == sc_time.SC_ZERO_TIME)
            {
                if ((m_simc.get_status() & (sc_status.SC_END_OF_UPDATE | sc_status.SC_BEFORE_TIMESTEP)) == 0)
                {
                    string msg = string.Format("{0} :\n\t delta notification of '{1}' ignored", m_simc.get_status(), name());
                    sc_report_handler.report(sc_core.sc_severity.SC_WARNING, "forbidden action in simulation phase callback", msg);
                    return;
                }
                if (m_notify_type == notify_t.TIMED)
                {
                    // remove this event from the timed events set
                    Debug.Assert(m_timed != null);
                    m_timed.m_event = null;
                    m_timed = null;
                }
                // add this event to the delta events set
                m_delta_event_index = m_simc.add_delta_event(this);
                m_notify_type = notify_t.DELTA;
                return;
            }
            if ((m_simc.get_status() & (sc_status.SC_END_OF_UPDATE | sc_status.SC_BEFORE_TIMESTEP)) == 0)
            {
                string msg = string.Format("{0} :\n\t delta notification of '{1}' ignored", m_simc.get_status(), name());
                sc_report_handler.report(sc_core.sc_severity.SC_WARNING, "forbidden action in simulation phase callback", msg);
                return;
            }
            if (m_notify_type == notify_t.TIMED)
            {
                Debug.Assert(m_timed != null);
                if (m_timed.m_notify_time <= m_simc.time_stamp() + t)
                {
                    return;
                }
                // remove this event from the timed events set
                m_timed.m_event = null;
                m_timed = null;
            }
            // add this event to the timed events set
            sc_event_timed et = new sc_event_timed(this, (m_simc.time_stamp() + t));
            m_simc.add_timed_event(et);
            m_timed = et;
            m_notify_type = notify_t.TIMED;
        }

        // IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIII

        public virtual void notify(double v, sc_time_unit tu)
        {
            notify(new sc_time(v, tu, m_simc));
        }

        public virtual void notify_delayed()
        {
            if (m_notify_type != notify_t.NONE)
            {
                sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "notify_delayed() cannot be called on events ", "");
            }
            // add this event to the delta events set
            m_delta_event_index = m_simc.add_delta_event(this);
            m_notify_type = notify_t.DELTA;
        }

        public virtual void notify_delayed(sc_time t)
        {
            if (m_notify_type != notify_t.NONE)
            {
                sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "notify_delayed() cannot be called on events ", "");
            }
            if (t == sc_time.SC_ZERO_TIME)
            {
                // add this event to the delta events set
                m_delta_event_index = m_simc.add_delta_event(this);
                m_notify_type = notify_t.DELTA;
            }
            else
            {
                // add this event to the timed events set
                sc_event_timed et = new sc_event_timed(this, m_simc.time_stamp() + t);
                m_simc.add_timed_event(et);
                m_timed = et;
                m_notify_type = notify_t.TIMED;
            }
        }

        public virtual void notify_delayed(double v, sc_time_unit tu)
        {
            notify_delayed(new sc_time(v, tu, m_simc));
        }

        public static sc_event_or_list operator |(sc_event e1, sc_event e2)
        {
            sc_event_or_list expr = new sc_event_or_list();
            expr.push_back(e1);
            expr.push_back(e2);
            return expr;
        }

        public static sc_event_and_list operator &(sc_event e1, sc_event e2)
        {
            sc_event_and_list expr = new sc_event_and_list();
            expr.push_back(e1);
            expr.push_back(e2);
            return expr;
        }

        public virtual void add_static(sc_method_process method_h)
        {
            m_methods_static.Add(method_h);
        }

        public virtual void add_static(sc_thread_process thread_h)
        {
            m_threads_static.Add(thread_h);
        }

        public virtual void add_dynamic(sc_method_process method_h)
        {
            m_methods_dynamic.Add(method_h);
        }

        public virtual void add_dynamic(sc_thread_process thread_h)
        {
            m_threads_dynamic.Add(thread_h);
        }

        public virtual void notify_internal(sc_time t)
        {
            if (t == sc_time.SC_ZERO_TIME)
            {
                // add this event to the delta events set
                m_delta_event_index = m_simc.add_delta_event(this);
                m_notify_type = notify_t.DELTA;
            }
            else
            {
                sc_event_timed et = new sc_event_timed(this, m_simc.time_stamp() + t);
                m_simc.add_timed_event(et);
                m_timed = et;
                m_notify_type = notify_t.TIMED;
            }
        }

        public virtual void notify_next_delta()
        {
            if (m_notify_type != notify_t.NONE)
            {
                sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "notify_delayed() cannot be called on events that have pending notifications", "");
            }
            // add this event to the delta events set
            m_delta_event_index = m_simc.add_delta_event(this);
            m_notify_type = notify_t.DELTA;
        }


        public virtual bool remove_static(sc_method_process method_h_)
        {
            if (m_methods_static.Contains(method_h_))
            {
                m_methods_static.Remove(method_h_);
                return true;
            }
            else
                return false;
        }

        public bool remove_static(sc_thread_process thread_h_)
        {
            if (m_threads_static.Contains(thread_h_))
            {
                m_threads_static.Remove(thread_h_);
                return true;
            }
            else
                return false;
        }

        public virtual bool remove_dynamic(sc_method_process method_h_)
        {
            if (m_methods_dynamic.Contains(method_h_))
            {
                m_methods_dynamic.Remove(method_h_);
                return true;
            }
            else
                return false;
        }

        public virtual bool remove_dynamic(sc_thread_process thread_h_)
        {
            if (m_threads_dynamic.Contains(thread_h_))
            {
                m_threads_dynamic.Remove(thread_h_);
                return true;
            }
            else
                return false;
        }


        // +----------------------------------------------------------------------------
        // |"sc_event::register_event"
        // | 
        // | This method sets the name of this object instance and optionally adds 
        // | it to the object manager's hierarchy. The object instance will be
        // | inserted into the object manager's hierarchy if one of the following is
        // | true:
        // |   (a) the leaf name is non-null and does not start with  
        // |       SC_KERNEL_EVENT_PREFIX.
        // |   (b) the event is being created before the start of simulation.
        // |
        // | Arguments:
        // |     leaf_name = leaf name of the object or NULL.
        // +----------------------------------------------------------------------------
        public virtual void register_event(string leaf_name)
        {
            sc_object_manager object_manager = m_simc.get_object_manager();
            m_parent_p = m_simc.active_object();

            // No name provided, if we are not executing then create a name:

            if (string.IsNullOrEmpty(leaf_name))
            {
                if (sc_simcontext.sc_is_running(m_simc))
                    return;
                leaf_name = sc_simcontext.sc_gen_unique_name("event");
            }

            // Create a hierarchichal name and place it into the object manager if
            // its not a kernel event:

            m_name = object_manager.create_name(leaf_name);

            if (string.IsNullOrEmpty(leaf_name))
            {
                object_manager.insert_event(m_name, this);
                if (m_parent_p != null)
                    m_parent_p.add_child_event(this);
                else
                    m_simc.add_child_event(this);
            }
        }

        public virtual void reset()
        {
            m_notify_type = notify_t.NONE;
            m_delta_event_index = -1;
            m_timed = null;
            // clear the dynamic sensitive methods
            m_methods_dynamic.Clear();
            // clear the dynamic sensitive threads
            m_threads_dynamic.Clear();
        }


        // +----------------------------------------------------------------------------
        // |"sc_event::trigger"
        // | 
        // | This method "triggers" this object instance. This consists of scheduling
        // | for execution all the processes that are schedulable and waiting on this 
        // | event.
        // +----------------------------------------------------------------------------
        public virtual void trigger()
        {
            int last_i; // index of last element in vector now accessing.
            int size; // size of vector now accessing.


            // trigger the static sensitive methods

            if ((m_methods_static.Count) != 0)
            {
                foreach (sc_method_process p in m_methods_static)
                {
                    p.trigger_static();
                }
            }

            // trigger the dynamic sensitive methods


            if ((m_methods_dynamic.Count) != 0)
            {
                for (int i = 0; i < m_methods_dynamic.Count; )
                {
                    sc_method_process p = m_methods_dynamic[i];
                    if (p.trigger_dynamic(this))
                    {
                        m_methods_dynamic.RemoveAt(i);
                    }
                    else
                        i++;
                }
            }


            // trigger the static sensitive threads

            if ((m_threads_static.Count) != 0)
            {
                foreach (sc_thread_process p in m_threads_static)
                {
                    p.trigger_static();
                }
            }

            // trigger the dynamic sensitive threads

            if ((m_threads_dynamic.Count) != 0)
            {
                for (int i = 0; i < m_threads_dynamic.Count; )
                {
                    sc_thread_process p = m_threads_dynamic[i];
                    if (p.trigger_dynamic(this))
                    {
                        m_threads_dynamic.RemoveAt(i);
                    }
                    else
                        i++;
                }
            }

            m_notify_type = notify_t.NONE;
            m_delta_event_index = -1;
            m_timed = null;
        }
    }


    public class sc_event_expr<T>
        where T : sc_event
    {

        private sc_event_expr()
        {
            m_expr = new List<T>();
        }


        public sc_event_expr(sc_event_expr<T> e) // move semantics
        {
            m_expr = e.m_expr;
        }

        public virtual void release()
        {
            Debug.Assert(m_expr != null);
            m_expr.Clear();
        }


        public virtual void push_back(T el)
        {
            Debug.Assert(m_expr != null);
            m_expr.Add(el);
        }


        private List<T> m_expr = new List<T>();

    }

    // ----------------------------------------------------------------------------
    //  CLASS : sc_event_list
    //
    //  Base class for lists of events.
    // ----------------------------------------------------------------------------

    public partial class sc_event_list
    {

        public int size()
        {
            return m_events.Count;
        }



        // ----------------------------------------------------------------------------
        //  CLASS : sc_event_list
        //
        //  Base class for lists of events.
        // ----------------------------------------------------------------------------

        public virtual void push_back(sc_event e)
        {
            // make sure e is not already in the list
            if (m_events.Contains(e) == false)
                m_events.Push(e);
        }
        public virtual void push_back(sc_event_list el)
        {
            foreach (sc_event e in el.m_events)
            {
                push_back(e);
            }
            el.auto_delete();
        }


        // IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIII

        public sc_event_list(bool and_list_)
            : this(and_list_, false)
        {
        }

        public sc_event_list(bool and_list_, bool auto_delete_)
        {
            m_events = new Stack<sc_event>();
            m_and_list = and_list_;
            m_auto_delete = auto_delete_;
            m_busy = 0;
        }

        public sc_event_list(sc_event e, bool and_list_)
            : this(e, and_list_, false)
        {
        }

        public sc_event_list(sc_event e, bool and_list_, bool auto_delete_)
        {
            m_events = new Stack<sc_event>();
            m_and_list = and_list_;
            m_auto_delete = auto_delete_;
            m_busy = 0;
            m_events.Push(e);
        }

        ~sc_event_list()
        {
            if (m_busy != 0)
                report_premature_destruction();
        }



        public virtual bool and_list()
        {
            return m_and_list;
        }


        public virtual void add_dynamic(sc_method_process method_h)
        {
            m_busy++;
            if (m_events.Count != 0)
            {
                foreach (sc_event e in m_events)
                {
                    e.add_dynamic(method_h);
                }
            }
        }


        public virtual void add_dynamic(sc_thread_process thread_h)
        {
            m_busy++;
            if (m_events.Count != 0)
            {
                foreach (sc_event e in m_events)
                {
                    e.add_dynamic(thread_h);
                }
            }
        }

        public virtual void remove_dynamic(sc_method_process method_h, sc_event e_not)
        {
            if (m_events.Count != 0)
            {
                foreach (sc_event e in m_events)
                {
                    if (e != e_not)
                    {
                        e.remove_dynamic(method_h);
                    }
                }
            }
        }

        public virtual void remove_dynamic(sc_thread_process thread_h, sc_event e_not)
        {
            if (m_events.Count != 0)
            {
                foreach (sc_event e in m_events)
                {
                    if (e != e_not)
                    {
                        e.remove_dynamic(thread_h);
                    }
                }
            }
        }


        public virtual bool busy()
        {
            return m_busy != 0;
        }

        public virtual bool temporary()
        {
            return m_auto_delete && m_busy == 0;
        }

        public virtual void auto_delete()
        {
            if (m_busy != 0)
            {
                --m_busy;
            }
        }


        protected void report_premature_destruction()
        {
            // TDB: reliably detect premature destruction
            //
            // If an event list is used as a member of a module,
            // its lifetime may (correctly) end, although there
            // are processes currently waiting for it.
            //
            // Detecting (and ignoring) this corner-case is quite
            // difficult for similar reasons to the sc_is_running()
            // return value during the destruction of the module
            // hierarchy.
            //
            // Ignoring the lifetime checks for now, if no process
            // is currently running (which is only part of the story):

            if (sc_simcontext.sc_get_current_process_handle().valid())
            {
                // FIXME: improve error-handling
                Debug.Assert(false, "sc_event_list prematurely destroyed");
            }

        }

        protected void report_invalid_modification()
        {
            // FIXME: improve error-handling
            Debug.Assert(false, "sc_event_list modfied while being waited on");
        }


        private readonly Stack<sc_event> m_events = new Stack<sc_event>();
        private bool m_and_list;
        private bool m_auto_delete;
        private uint m_busy;
    }


    // ----------------------------------------------------------------------------
    //  CLASS : sc_event_and_list
    //
    //  AND list of events.
    // ----------------------------------------------------------------------------

    public class sc_event_and_list : sc_event_list
    {

        protected sc_event_and_list(bool auto_delete_)
            : base(true, auto_delete_)
        {
        }



        // IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIII

        public sc_event_and_list()
            : base(true)
        {
        }
        public sc_event_and_list(sc_event e)
            : base(true)
        {
            push_back(e);
        }


        public static sc_event_and_list operator &(sc_event_and_list e1, sc_event e2)
        {

            sc_event_and_list expr = new sc_event_and_list();
            expr.push_back(e1);
            expr.push_back(e2);
            return expr;
        }

    }

    // ----------------------------------------------------------------------------
    //  CLASS : sc_event_or_list
    //
    //  OR list of events.
    // ----------------------------------------------------------------------------

    public class sc_event_or_list : sc_event_list
    {
        protected sc_event_or_list(bool auto_delete_)
            : base(false, auto_delete_)
        {
        }


        // IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIII

        public sc_event_or_list()
            : base(false)
        {
        }
        public sc_event_or_list(sc_event e)
            : base(false)
        {
            push_back(e);
        }




        public static sc_event_or_list operator |(sc_event_or_list e1, sc_event e2)
        {
            sc_event_or_list expr = new sc_event_or_list();
            expr.push_back(e1);
            expr.push_back(e2);
            return expr;
        }

    }

    // ----------------------------------------------------------------------------
    //  CLASS : sc_event_timed
    //
    //  Class for storing the time to notify a timed event.
    // ----------------------------------------------------------------------------

    public class sc_event_timed
    {
        public sc_event_timed(sc_event e, sc_time t)
        {
            m_event = e;
            m_notify_time = new sc_time(t);
        }



        public sc_time notify_time()
        {
            return m_notify_time;
        }

        public sc_event m_event;
        public sc_time m_notify_time = new sc_time();

        public sc_event Event()
        {
            return m_event;
        }
    }
}