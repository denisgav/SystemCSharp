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
using System;
using System.Text;
namespace sc_core
{

    //=============================================================================
    //  CLASS : sc_runnable
    //
    //  Class that manages the ready-to-run queues.
    //=============================================================================

    public partial class sc_runnable
    {
        public virtual void Init()
        {
            m_methods_pop = null;
            if (m_methods_push_head == null)
            {
                m_methods_push_head = new sc_method_process("methods_push_head", true,
                                                       null, null, null);
                m_methods_push_head.dont_initialize(true);
                m_methods_push_head.detach();
            }
            m_methods_push_tail = m_methods_push_head;
            m_methods_push_head.set_next_runnable(null);

            m_threads_pop = null;
            if (m_threads_push_head == null)
            {
                m_threads_push_head = new sc_thread_process("threads_push_head", true,
                                                        null, null, null);
                m_threads_push_head.dont_initialize(true);
                m_threads_push_head.detach();
            }
            m_threads_push_head.set_next_runnable(null);
            m_threads_push_tail = m_threads_push_head;
        }

        public virtual void toggle_methods()
        {
            if (m_methods_pop == null)
            {
                m_methods_pop = m_methods_push_head.next_runnable();
                m_methods_push_head.set_next_runnable(null);
                m_methods_push_tail = m_methods_push_head;
            }
        }

        public virtual void toggle_threads()
        {
            if (m_threads_pop == null)
            {
                m_threads_pop = m_threads_push_head.next_runnable();
                m_threads_push_head.set_next_runnable(null);
                m_threads_push_tail = m_threads_push_head;
            }
        }

        public virtual void remove_method(sc_method_process remove_p)
        {
            sc_method_process now_p;     // Method now checking.
            sc_method_process prior_p;   // Method prior to now_p.

            // Don't try to remove things if we have not been initialized.

            if (!IsInitialized) return;

            // Search the push queue:

            prior_p = m_methods_push_head;
            for (now_p = m_methods_push_head; now_p != null;
                now_p = now_p.next_runnable())
            {
                if (remove_p == now_p)
                {
                    prior_p.set_next_runnable(now_p.next_runnable());
                    if (now_p == m_methods_push_tail)
                    {
                        m_methods_push_tail = prior_p;
                    }
                    now_p.set_next_runnable(null);
                    Debug.WriteLine("removing method from push queue");
                    return;
                }
                prior_p = now_p;
            }

            // Search the pop queue:

            prior_p = null;
            for (now_p = m_methods_pop; now_p != null;
              now_p = now_p.next_runnable())
            {
                if (remove_p == now_p)
                {
                    if (prior_p != null)
                        prior_p.set_next_runnable(now_p.next_runnable());
                    else
                        m_methods_pop = now_p.next_runnable();
                    now_p.set_next_runnable(null);
                    Debug.WriteLine("removing method from pop queue");
                    return;
                }
                prior_p = now_p;
            }
        }

        public virtual void remove_thread(sc_thread_process remove_p)
        {
            sc_thread_process now_p;     // Thread now checking.
            sc_thread_process prior_p;   // Thread prior to now_p.

            // Don't try to remove things if we have not been initialized.

            if (!IsInitialized) return;

            // Search the push queue:

            prior_p = m_threads_push_head;
            for (now_p = m_threads_push_head; now_p != null;
              now_p = now_p.next_runnable())
            {
                if (remove_p == now_p)
                {
                    prior_p.set_next_runnable(now_p.next_runnable());
                    if (now_p == m_threads_push_tail)
                    {
                        m_threads_push_tail = prior_p;
                    }
                    now_p.set_next_runnable(null);
                    Debug.WriteLine("removing thread from push queue");
                    return;
                }
                prior_p = now_p;
            }

            // Search the pop queue:

            prior_p = null;
            for (now_p = m_threads_pop; now_p != null;
              now_p = now_p.next_runnable())
            {
                if (remove_p == now_p)
                {
                    if (prior_p != null)
                        prior_p.set_next_runnable(now_p.next_runnable());
                    else
                        m_threads_pop = now_p.next_runnable();
                    now_p.set_next_runnable(null);
                    Debug.WriteLine("removing thread from pop queue");
                    return;
                }
                prior_p = now_p;
            }
        }

        public virtual void execute_method_next(sc_method_process method_h)
        {
            Debug.WriteLine("pushing this method to execute next");
            method_h.set_next_runnable(m_methods_pop);
            m_methods_pop = method_h;
        }

        public virtual void execute_thread_next(sc_thread_process thread_h)
        {
            Debug.WriteLine("pushing this thread to execute next");
            thread_h.set_next_runnable(m_threads_pop);
            m_threads_pop = thread_h;
        }

        public virtual void push_back_method(sc_method_process method_h)
        {
            Debug.WriteLine("pushing back method");
            method_h.set_next_runnable(null);
            m_methods_push_tail.set_next_runnable(method_h);
            m_methods_push_tail = method_h;
        }

        public virtual void push_back_thread(sc_thread_process thread_h)
        {
            Debug.WriteLine("pushing back thread");
            thread_h.set_next_runnable(null);
            m_threads_push_tail.set_next_runnable(thread_h);
            m_threads_push_tail = thread_h;
        }

        public virtual void push_front_method(sc_method_process method_h)
        {
            // assert( method_h->next_runnable() == 0 ); // Can't queue twice.
            Debug.WriteLine("pushing front method");
            method_h.set_next_runnable(m_methods_push_head.next_runnable());
            if (m_methods_push_tail == m_methods_push_head) // Empty queue.
            {
                m_methods_push_tail.set_next_runnable(method_h);
                m_methods_push_tail = method_h;
            }
            else                                               // Non-empty queue.
            {
                m_methods_push_head.set_next_runnable(method_h);
            }
        }

        public virtual void push_front_thread(sc_thread_process thread_h)
        {
            Debug.WriteLine("pushing front thread");
            thread_h.set_next_runnable(m_threads_push_head.next_runnable());
            if (m_threads_push_tail == m_threads_push_head) // Empty queue.
            {
                m_threads_push_tail.set_next_runnable(thread_h);
                m_threads_push_tail = thread_h;
            }
            else                                               // Non-empty queue.
            {
                m_threads_push_head.set_next_runnable(thread_h);
            }
        }

        public virtual bool IsInitialized
        {
            get { return m_methods_push_head != null && m_threads_push_head != null; }
        }

        public virtual bool IsEmpty
        {
            get
            {
                return IsInitialized && m_methods_push_head.next_runnable() == null &&
                    m_methods_pop == null &&
                    m_threads_push_head.next_runnable() == null &&
                    m_threads_pop == null;
            }
        }

        public virtual sc_method_process pop_method()
        {
            sc_method_process result_p;

            result_p = m_methods_pop;
            if (result_p != null)
            {
                m_methods_pop = result_p.next_runnable();
                result_p.set_next_runnable(null);
            }
            else
            {
                result_p = null;
            }
            Debug.WriteLine("popping method");
            return result_p;
        }

        public virtual sc_thread_process pop_thread()
        {
            sc_thread_process result_p;

            result_p = m_threads_pop;
            if (result_p != null)
            {
                m_threads_pop = result_p.next_runnable();
                result_p.set_next_runnable(null);
            }
            else
            {
                result_p = null;
            }
            Debug.WriteLine("popping thread for execution");
            return result_p;
        }

        public virtual string dump_to_string()
        {
            StringBuilder res = new StringBuilder();
            // Dump the thread queues:

            res.AppendLine("thread pop queue: ");
            for (sc_thread_process p = m_threads_pop; p != null;
                  p = p.next_runnable())
            {
                res.AppendLine(p.ToString());
            }

            res.AppendLine("thread push queue: ");
            for (sc_thread_process p = m_threads_push_head.next_runnable();
                  p != null; p = p.next_runnable())
            {
                res.AppendLine(p.ToString());
            }

            return res.ToString();
        }

        private sc_method_process m_methods_push_head;
        private sc_method_process m_methods_push_tail;
        private sc_method_process m_methods_pop;
        private sc_thread_process m_threads_push_head;
        private sc_thread_process m_threads_push_tail;
        private sc_thread_process m_threads_pop;

    }

} // namespace sc_core
