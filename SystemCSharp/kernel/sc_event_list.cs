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
    public class sc_event_expr<T> : IEnumerable<T>
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

        public IEnumerator<T> GetEnumerator()
        {
            foreach (T item in m_expr)
            {
                yield return item;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            foreach (T item in m_expr)
            {
                yield return item;
            }
        }
    }

    // ----------------------------------------------------------------------------
    //  CLASS : sc_event_list
    //
    //  Base class for lists of events.
    // ----------------------------------------------------------------------------

    public class sc_event_list : IDisposable, IEnumerable<sc_event>
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
                    if (m_busy != 0)
                        report_premature_destruction();
                    m_events.Clear();
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
        ~sc_event_list()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }

        public IEnumerator<sc_event> GetEnumerator()
        {
            foreach (sc_event e in m_events)
                yield return e;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            foreach (sc_event e in m_events)
                yield return e;
        }
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
}
