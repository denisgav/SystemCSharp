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
    public enum sc_event_notify_t
    {
        NONE,
        DELTA,
        TIMED
    }

    // ----------------------------------------------------------------------------
    //  CLASS : sc_event_timed
    //
    //  Class for storing the time to notify a timed event.
    // ----------------------------------------------------------------------------
    public class sc_event_timed : IDisposable
    {
        public sc_event m_event;
        public sc_time m_notify_time = new sc_time();

        public sc_event_timed(sc_event e, sc_time t)
        {
            m_event = e;
            m_notify_time = new sc_time(t);
        }

        public virtual sc_time NotifyTime
        {
            get { return m_notify_time; }
        }

        public virtual sc_event Event
        {
            get { return m_event; }
        }

        // Track whether Dispose has been called.
        private bool disposed = false;

        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        // +----------------------------------------------------------------------------
        // |"sc_event::~sc_event"
        // | 
        // | This is the object instance destructor for this class. It cancels any
        // | outstanding waits and removes the event from the object manager's 
        // | instance table if it has a name.
        // +----------------------------------------------------------------------------
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    m_event.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                // If disposing is false,
                // only the following code is executed.

                // Note disposing has been done.
                disposed = true;

            }
        }

        // Use C# destructor syntax for finalization code.
        // This destructor will run only if the Dispose method
        // does not get called.
        // It gives your base class the opportunity to finalize.
        // Do not provide destructors in types derived from this class.
        ~sc_event_timed()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
    }

    public class sc_event : IDisposable
    {
        private string m_name; // name of object.
        private sc_object m_parent_p; // parent sc_object for this event.
        private sc_simcontext m_simc;
        private sc_event_notify_t m_notify_type;
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
            m_notify_type = sc_event_notify_t.NONE;
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
            m_notify_type = sc_event_notify_t.NONE;
            m_delta_event_index = -1;
            m_timed = null;
            m_methods_static = new List<sc_method_process>();
            m_methods_dynamic = new List<sc_method_process>();
            m_threads_static = new List<sc_thread_process>();
            m_threads_dynamic = new List<sc_thread_process>();
            // Skip simulator's internally defined events.
            register_event(name);
        }

        public virtual void cancel()
        {
            // cancel a delta or timed notification
            switch (m_notify_type)
            {
                case sc_event_notify_t.DELTA:
                    {
                        // remove this event from the delta events set
                        m_simc.remove_delta_event(this);
                        m_notify_type = sc_event_notify_t.NONE;
                        break;
                    }
                case sc_event_notify_t.TIMED:
                    {
                        // remove this event from the timed events set
                        Debug.Assert(m_timed != null);
                        m_timed.m_event = null;
                        m_timed = null;
                        m_notify_type = sc_event_notify_t.NONE;
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
            if (m_notify_type == sc_event_notify_t.DELTA)
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
                if (m_notify_type == sc_event_notify_t.TIMED)
                {
                    // remove this event from the timed events set
                    Debug.Assert(m_timed != null);
                    m_timed.m_event = null;
                    m_timed = null;
                }
                // add this event to the delta events set
                m_delta_event_index = m_simc.add_delta_event(this);
                m_notify_type = sc_event_notify_t.DELTA;
                return;
            }
            if ((m_simc.get_status() & (sc_status.SC_END_OF_UPDATE | sc_status.SC_BEFORE_TIMESTEP)) == 0)
            {
                string msg = string.Format("{0} :\n\t delta notification of '{1}' ignored", m_simc.get_status(), name());
                sc_report_handler.report(sc_core.sc_severity.SC_WARNING, "forbidden action in simulation phase callback", msg);
                return;
            }
            if (m_notify_type == sc_event_notify_t.TIMED)
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
            m_notify_type = sc_event_notify_t.TIMED;
        }

        // IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIII

        public virtual void notify(double v, sc_time_unit tu)
        {
            notify(new sc_time(v, tu, m_simc));
        }

        public virtual void notify_delayed()
        {
            if (m_notify_type != sc_event_notify_t.NONE)
            {
                sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "notify_delayed() cannot be called on events ", "");
            }
            // add this event to the delta events set
            m_delta_event_index = m_simc.add_delta_event(this);
            m_notify_type = sc_event_notify_t.DELTA;
        }

        public virtual void notify_delayed(sc_time t)
        {
            if (m_notify_type != sc_event_notify_t.NONE)
            {
                sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "notify_delayed() cannot be called on events ", "");
            }
            if (t == sc_time.SC_ZERO_TIME)
            {
                // add this event to the delta events set
                m_delta_event_index = m_simc.add_delta_event(this);
                m_notify_type = sc_event_notify_t.DELTA;
            }
            else
            {
                // add this event to the timed events set
                sc_event_timed et = new sc_event_timed(this, m_simc.time_stamp() + t);
                m_simc.add_timed_event(et);
                m_timed = et;
                m_notify_type = sc_event_notify_t.TIMED;
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
                m_notify_type = sc_event_notify_t.DELTA;
            }
            else
            {
                sc_event_timed et = new sc_event_timed(this, m_simc.time_stamp() + t);
                m_simc.add_timed_event(et);
                m_timed = et;
                m_notify_type = sc_event_notify_t.TIMED;
            }
        }

        public virtual void notify_next_delta()
        {
            if (m_notify_type != sc_event_notify_t.NONE)
            {
                sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "notify_delayed() cannot be called on events that have pending notifications", "");
            }
            // add this event to the delta events set
            m_delta_event_index = m_simc.add_delta_event(this);
            m_notify_type = sc_event_notify_t.DELTA;
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
            m_notify_type = sc_event_notify_t.NONE;
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


            for (int i = 0; i < m_threads_dynamic.Count; )
            {
                sc_thread_process p = m_threads_dynamic[i];
                if (p.trigger_dynamic(this))
                {
                    if (m_threads_dynamic.Count != 0)
                    {
                        m_threads_dynamic.RemoveAt(i);
                    }
                }
                else
                    i++;
            }


            m_notify_type = sc_event_notify_t.NONE;
            m_delta_event_index = -1;
            m_timed = null;
        }

        // Track whether Dispose has been called.
        private bool disposed = false;

        // +----------------------------------------------------------------------------
        // |"sc_object_manager::~sc_object_manager"
        // | 
        // | This is the object instance destructor for this class. It goes through
        // | each sc_object instance in the instance table and sets its m_simc field
        // | to NULL.
        // +----------------------------------------------------------------------------

        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        // +----------------------------------------------------------------------------
        // |"sc_event::~sc_event"
        // | 
        // | This is the object instance destructor for this class. It cancels any
        // | outstanding waits and removes the event from the object manager's 
        // | instance table if it has a name.
        // +----------------------------------------------------------------------------
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    cancel();
                    if (m_name.Length != 0)
                    {
                        sc_object_manager object_manager_p = m_simc.get_object_manager();
                        object_manager_p.remove_event(m_name);
                    }
                    m_timed.Dispose();

                    m_methods_static.Clear();
                    m_methods_dynamic.Clear();
                    m_threads_static.Clear();
                    m_threads_dynamic.Clear();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                // If disposing is false,
                // only the following code is executed.

                // Note disposing has been done.
                disposed = true;

            }
        }

        // Use C# destructor syntax for finalization code.
        // This destructor will run only if the Dispose method
        // does not get called.
        // It gives your base class the opportunity to finalize.
        // Do not provide destructors in types derived from this class.
        ~sc_event()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
    }    
}