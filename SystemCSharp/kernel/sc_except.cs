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


using System;
using System.Diagnostics;
namespace sc_core
{

    public class sc_user : Exception
    {
        //EMPTY
        public sc_user()
        {
        }
        public sc_user(sc_user UnnamedParameter1)
        {
        }
    }

    public class sc_halt : Exception
    {
        public sc_halt()
        {
        }
        public sc_halt(sc_halt UnnamedParameter1)
        {
        }
    }

    public class sc_kill : Exception
    {
        public sc_kill()
        {
        }
        public sc_kill(sc_kill UnnamedParameter1)
        {
        }
    }

    public class sc_unwind_exception : Exception
    {
        public virtual bool is_reset()
        {
            return m_is_reset;
        }
        

        public virtual string what()
        {
            return (m_is_reset) ? "RESET" : "KILL";
        }


        // enable catch by value
        public sc_unwind_exception(sc_unwind_exception that)
        {
            m_proc_p = that.m_proc_p;
            m_is_reset = that.m_is_reset;
            that.m_proc_p = null; // move to new instance
        }

        public sc_unwind_exception(sc_process_b proc_p)
            : this(proc_p, false)
        {
        }
        public sc_unwind_exception(sc_process_b proc_p, bool is_reset)
        {
            m_proc_p = proc_p;
            m_is_reset = is_reset;
            Debug.Assert(m_proc_p != null);
            m_proc_p.start_unwinding();
        }


        public virtual bool active()
        {
            return (m_proc_p != null) && m_proc_p.is_unwinding();
        }

        public virtual void clear()
        {
            Debug.Assert(m_proc_p != null);
            m_proc_p.clear_unwinding();
        }

        private sc_process_b m_proc_p; // used to check, if caught by the kernel
        private readonly bool m_is_reset; // true if this is an unwind of a reset

    }
}
