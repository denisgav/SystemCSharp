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
using System.Threading;
using System.Runtime.InteropServices;

namespace sc_core
{
    public class sc_cor : IDisposable
    {
        public sc_cor(sc_process_b parent)
        {
            this.parent = parent;
        }

        // destructor
        public virtual void Dispose()
        {
        }

        // switch stack protection on/off
        public virtual void stack_protect(bool arg) // enable
        {
        }

        private ParameterizedThreadStart threadFn;
        public ParameterizedThreadStart ThreadFn
        {
            get { return threadFn; }
            set { threadFn = value; }
        }

        private object functionCallArg;
        public object FunctionCallArg
        {
            get { return functionCallArg; }
            set { functionCallArg = value; }
        }

        private sc_cor_pkg corPkg;
        public sc_cor_pkg CorPkg
        {
            get { return corPkg; }
            set { corPkg = value; }
        }

        private AutoResetEvent autoEvent;
        public AutoResetEvent AutoEvent
        {
            get { return autoEvent; }
            set { autoEvent = value; }
        }


        private Thread thread;
        public Thread Thread
        {
            get { return thread; }
            set { thread = value; }
        }

        private sc_process_b parent;
        public sc_process_b Parent
        {
            get { return parent; }
            set { parent = value; }
        }

    }

    // ----------------------------------------------------------------------------
    //  CLASS : sc_cor_pkg
    //
    //  Coroutine package abstract base class.
    // ----------------------------------------------------------------------------

    public class sc_cor_pkg : IDisposable
    {
        private static sc_cor activeCoroutine;
        public static sc_cor ActiveCoroutine
        {
            get { return activeCoroutine; }
            set { activeCoroutine = value; }
        }

        private static Thread mainThread;
        public static Thread MainThread
        {
            get { return mainThread; }
            set { mainThread = value; }
        }

        private AutoResetEvent mainAutoEvent;
        public AutoResetEvent MainAutEvent
        {
            get { return mainAutoEvent; }
            set { mainAutoEvent = value; }
        }


        private static sc_cor mainCor;
        public static sc_cor MainCor
        {
            get { return mainCor; }
            set { mainCor = value; }
        }

        private static int instanceCount;
        public static int InstanceCount
        {
            get { return instanceCount; }
            set { instanceCount = value; }
        }

        // constructor
        public sc_cor_pkg(sc_simcontext simc)
        {
            m_simc = simc;
            Debug.Assert(simc != null);

            // initialize the current coroutine
            if (++instanceCount == 1)
            {
                Debug.Assert(activeCoroutine == null);
                mainCor = new sc_cor(null);
                mainCor.CorPkg = this;
                activeCoroutine = mainCor;

                
                mainAutoEvent = new AutoResetEvent(true);
                mainThread = System.Threading.Thread.CurrentThread;
                mainCor.Thread = mainThread;
                mainCor.AutoEvent = mainAutoEvent;

                m_simc.set_curr_proc(mainCor.Parent);
            }
        }

        // destructor
        public virtual void Dispose()
        {
        }

        public virtual void ThreadMethod(object o)
        {
            sc_cor cor = o as sc_cor;

            lock (mainCor)
            {
                cor.AutoEvent.Reset();
            }
            cor.AutoEvent.WaitOne();
            lock (mainCor)
            {
                cor.AutoEvent.Reset();
                m_simc.set_curr_proc(cor.Parent);
            }
            cor.ThreadFn(cor.FunctionCallArg);
        }

        // create a new coroutine
        public virtual sc_cor create(uint stack_size, ParameterizedThreadStart fn, object o, sc_process_b parent)
        {
            sc_cor cor_p = new sc_cor(parent);

            // INITIALIZE OBJECT'S FIELDS FROM ARGUMENT LIST:

            cor_p.CorPkg = this;
            cor_p.ThreadFn = fn;
            cor_p.FunctionCallArg = o;
            cor_p.AutoEvent = new AutoResetEvent(true);

            // SET UP THREAD CREATION ATTRIBUTES:
            //
            // Use default values except for stack size. If stack size is non-zero
            // set it.
            Thread thread = new Thread(ThreadMethod, (int)stack_size);

            // ALLOCATE THE POSIX THREAD TO USE AND FORCE SEQUENTIAL EXECUTION:
            //
            // Because pthread_create causes the created thread to be executed,
            // we need to let it run until we can block in the invoke_module_method.
            // So we:
            //   (1) Lock the creation mutex before creating the new thread.
            //   (2) Sleep on the creation condition, which will be signalled by
            //       the newly created thread just before it goes to sleep in
            //       invoke_module_method.
            // This scheme results in the newly created thread being dormant before
            // the main thread continues execution.

            cor_p.Thread = thread;
            thread.Start(cor_p);

            return cor_p;
        }

        // yield to the next coroutine
        public virtual void yield(sc_cor next_cor)
        {
            sc_cor from_p = activeCoroutine;
            sc_cor to_p = next_cor;

            //Console.WriteLine("Switch from {0} to {1}", (from_p.Parent != null) ? from_p.Parent.name() : "NULL", (to_p.Parent != null) ? to_p.Parent.name() : "NULL");

            if (to_p != from_p)
            {
                lock (mainCor)
                {
                    m_simc.set_curr_proc(to_p.Parent);
                    activeCoroutine = to_p; // When we come out of wait make ourselves active.
                    from_p.AutoEvent.Reset();
                    to_p.AutoEvent.Set();
                }
                //Console.WriteLine("From {0} wait one", (from_p.Parent != null) ? from_p.Parent.name() : "NULL");
                from_p.AutoEvent.WaitOne();
                //Console.WriteLine("From {0} wait done", (from_p.Parent != null) ? from_p.Parent.name() : "NULL");
            }

            lock (mainCor)
            {
                m_simc.set_curr_proc(from_p.Parent);
                activeCoroutine = from_p; // When we come out of wait make ourselves active.
            }

            //Console.WriteLine("Switch from {0} to {1} Done.", (from_p.Parent != null) ? from_p.Parent.name() : "NULL", (to_p.Parent != null) ? to_p.Parent.name() : "NULL");
        }

        // abort the current coroutine (and resume the next coroutine)
        public virtual void abort(sc_cor next_cor)
        {
            //next_cor.Mutex.WaitOne();
            //next_cor.Mutex.ReleaseMutex();
            //Console.WriteLine("Thread abort:", (next_cor.Parent != null) ? next_cor.Parent.name() : "NULL");
            next_cor.AutoEvent.Set();
            //next_cor.Thread.Abort();
            //next_cor.Dispose();
        }

        // get the main coroutine
        public virtual sc_cor get_main()
        {
            return mainCor;
        }

        // get the simulation context
        public sc_simcontext simcontext()
        {
            return m_simc;
        }

        private sc_simcontext m_simc;
    }

} // namespace sc_core
