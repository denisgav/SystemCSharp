using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SystemCSharp.Kernel
{
    public class SimulationRunnable
    {
        public SimulationRunnable()
        { }

        public virtual void Init()
        {
        }

        public virtual void ToggleMethods()
        {
            if (m_methods_pop == null)
            {
                m_methods_pop = m_methods_push_head.NextRunnable(); ;
                m_methods_push_head.SetNextRunnable(null);
                m_methods_push_tail = m_methods_push_head;
            }
        }

        public virtual void ToggleThreads()
        {
            if (m_threads_pop == null)
            {
                m_threads_pop = m_threads_push_head.NextRunnable();
                m_threads_push_head.SetNextRunnable(null);
                m_threads_push_tail = m_threads_push_head;
            }
        }

        public virtual void RemoveMethod(SimulationMethodProcess remove_p)
        {
            SimulationMethodProcess now_p;     // Method now checking.
            SimulationMethodProcess prior_p;   // Method prior to now_p.

            // Don't try to remove things if we have not been initialized.

            if (!IsInitialized) return;

            // Search the push queue:

            prior_p = m_methods_push_head;
            for (now_p = m_methods_push_head; now_p != null;
                now_p = now_p.NextRunnable())
            {
                if (remove_p == now_p)
                {
                    prior_p.SetNextRunnable(now_p.NextRunnable());
                    if (now_p == m_methods_push_tail)
                    {
                        m_methods_push_tail = prior_p;
                    }
                    now_p.SetNextRunnable(null);
                    Debug.WriteLine("removing method from push queue");
                    return;
                }
                prior_p = now_p;
            }

            // Search the pop queue:

            prior_p = null;
            for (now_p = m_methods_pop; now_p != null;
              now_p = now_p.NextRunnable())
            {
                if (remove_p == now_p)
                {
                    if (prior_p != null)
                        prior_p.SetNextRunnable(now_p.NextRunnable());
                    else
                        m_methods_pop = now_p.NextRunnable();
                    now_p.SetNextRunnable(null);
                    Debug.WriteLine("removing method from pop queue");
                    return;
                }
                prior_p = now_p;
            }
        }

        public virtual void RemoveThread(SimulationThreadProcess remove_p)
        {
            SimulationThreadProcess now_p;     // Thread now checking.
            SimulationThreadProcess prior_p;   // Thread prior to now_p.

            // Don't try to remove things if we have not been initialized.

            if (!IsInitialized) return;

            // Search the push queue:

            prior_p = m_threads_push_head;
            for (now_p = m_threads_push_head; now_p != null;
              now_p = now_p.NextRunnable())
            {
                if (remove_p == now_p)
                {
                    prior_p.SetNextRunnable(now_p.NextRunnable());
                    if (now_p == m_threads_push_tail)
                    {
                        m_threads_push_tail = prior_p;
                    }
                    now_p.SetNextRunnable(null);
                    Debug.WriteLine("removing thread from push queue");
                    return;
                }
                prior_p = now_p;
            }

            // Search the pop queue:

            prior_p = null;
            for (now_p = m_threads_pop; now_p != null;
              now_p = now_p.NextRunnable())
            {
                if (remove_p == now_p)
                {
                    if (prior_p != null)
                        prior_p.SetNextRunnable(now_p.NextRunnable());
                    else
                        m_threads_pop = now_p.NextRunnable();
                    now_p.SetNextRunnable(null);
                    Debug.WriteLine("removing thread from pop queue");
                    return;
                }
                prior_p = now_p;
            }
        }

        public virtual void ExecuteMethodNext(SimulationMethodProcess method_h)
        {
            Debug.WriteLine("pushing this method to execute next");
            method_h.SetNextRunnable(m_methods_pop);
            m_methods_pop = method_h;
        }

        public virtual void ExecuteThreadNext(SimulationThreadProcess thread_h)
        {
            Debug.WriteLine("pushing this thread to execute next");
            thread_h.SetNextRunnable(m_threads_pop);
            m_threads_pop = thread_h;
        }

        public virtual void PushBackMethod(SimulationMethodProcess method_h)
        {
            Debug.WriteLine("pushing back method");
            method_h.SetNextRunnable(null);
            m_methods_push_tail.SetNextRunnable(method_h);
            m_methods_push_tail = method_h;
        }

        public virtual void PushBackThread(SimulationThreadProcess thread_h)
        {
            Debug.WriteLine("pushing back thread");
            thread_h.SetNextRunnable(null);
            m_threads_push_tail.SetNextRunnable(thread_h);
            m_threads_push_tail = thread_h;
        }

        public virtual void PushFrontMethod(SimulationMethodProcess method_h)
        {
            // assert( method_h->next_runnable() == 0 ); // Can't queue twice.
            Debug.WriteLine("pushing front method");
            method_h.SetNextRunnable(m_methods_push_head.NextRunnable());
            if (m_methods_push_tail == m_methods_push_head) // Empty queue.
            {
                m_methods_push_tail.SetNextRunnable(method_h);
                m_methods_push_tail = method_h;
            }
            else                                               // Non-empty queue.
            {
                m_methods_push_head.SetNextRunnable(method_h);
            }
        }

        public virtual void PushFrontThread(SimulationThreadProcess thread_h)
        {
            Debug.WriteLine("pushing front thread");
            thread_h.SetNextRunnable(m_threads_push_head.NextRunnable());
            if (m_threads_push_tail == m_threads_push_head) // Empty queue.
            {
                m_threads_push_tail.SetNextRunnable(thread_h);
                m_threads_push_tail = thread_h;
            }
            else                                               // Non-empty queue.
            {
                m_threads_push_head.SetNextRunnable(thread_h);
            }
        }

        public virtual bool IsInitialized
        {
            get { throw new NotImplementedException(); }
        }

        public virtual bool IsEmpty
        {
            get { throw new NotImplementedException(); }
        }

        public virtual SimulationMethodProcess PopMethod()
        {
            SimulationMethodProcess result_p;

            result_p = m_methods_pop;
            if (result_p != null)
            {
                m_methods_pop = result_p.NextRunnable();
                result_p.SetNextRunnable(null);
            }
            else
            {
                result_p = null;
            }
            Debug.WriteLine("popping method");
            return result_p;
        }

        public virtual SimulationThreadProcess PopThread()
        {
            SimulationThreadProcess result_p;

            result_p = m_threads_pop;
            if (result_p != null)
            {
                m_threads_pop = result_p.NextRunnable();
                result_p.SetNextRunnable(null);
            }
            else
            {
                result_p = null;
            }
            Debug.WriteLine("popping thread for execution");
            return result_p;
        }

        public virtual string DumpToString()
        {
            StringBuilder res = new StringBuilder();
            // Dump the thread queues:

            res.AppendLine("thread pop queue: ");
            for (SimulationThreadProcess p = m_threads_pop; p != null;
                  p = p.NextRunnable())
            {
                res.AppendLine(p.ToString());
            }

            res.AppendLine("thread push queue: ");
            for (SimulationThreadProcess p = m_threads_push_head.NextRunnable();
                  p != null; p = p.NextRunnable())
            {
                res.AppendLine(p.ToString());
            }

            return res.ToString();
        }

        private SimulationMethodProcess m_methods_push_head;
        private SimulationMethodProcess m_methods_push_tail;
        private SimulationMethodProcess m_methods_pop;
        private SimulationThreadProcess m_threads_push_head;
        private SimulationThreadProcess m_threads_push_tail;
        private SimulationThreadProcess m_threads_pop;
    }
}
