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

    // ----------------------------------------------------------------------------
    //  CLASS : sc_semaphore
    //
    //  The sc_semaphore primitive channel class.
    // ----------------------------------------------------------------------------

    public class sc_semaphore : sc_object, sc_semaphore_if
    {

        // constructors


        // constructors

        public sc_semaphore(int init_value_)
            : base(sc_simcontext.sc_gen_unique_name("semaphore"))
        {
            m_free = new sc_event(((string)sc_constants.SC_KERNEL_EVENT_PREFIX + "_free_event"));
            m_value = init_value_;
            if (m_value < 0)
            {
                report_error("sc_semaphore requires an initial value >= 0");
            }
        }
        public sc_semaphore(string name_, int init_value_)
            : base(name_)
        {
            m_free = new sc_event(((string)sc_constants.SC_KERNEL_EVENT_PREFIX + "_free_event"));
            m_value = init_value_;
            if (m_value < 0)
            {
                report_error("sc_semaphore requires an initial value >= 0");
            }
        }


        // interface methods

        // lock (take) the semaphore, block if not available

        // interface methods

        // lock (take) the semaphore, block if not available

        public virtual int wait()
        {
            while (in_use())
            {
                sc_wait.wait(m_free, sc_simcontext.sc_get_curr_simcontext());
            }
            --m_value;
            return 0;
        }

        // lock (take) the semaphore, return -1 if not available

        public virtual int trywait()
        {
            if (in_use())
            {
                return -1;
            }
            --m_value;
            return 0;
        }

        // unlock (give) the semaphore

        public virtual int post()
        {
            ++m_value;
            m_free.notify();
            return 0;
        }

        // get the value of the semaphore
        public virtual int get_value()
        {
            return m_value;
        }

        public override string kind()
        {
            return "sc_semaphore";
        }


        // support methods

        protected bool in_use()
        {
            return (m_value <= 0);
        }


        // error reporting

        // ----------------------------------------------------------------------------
        //  CLASS : sc_semaphore
        //
        //  The sc_semaphore primitive channel class.
        // ----------------------------------------------------------------------------

        // error reporting

        protected void report_error(string id)
        {
            report_error(id, string.Empty);
        }

        protected void report_error(string id, string add_msg)
        {
            string msg = string.Empty;
            if (string.IsNullOrEmpty(add_msg) == false)
            {
                msg = string.Format("{0}: semaphore '{1}'", add_msg, name());
            }
            else
            {
                msg = string.Format("semaphore '{0}'", name());
            }
            sc_report_handler.report(sc_core.sc_severity.SC_ERROR, id, msg);
        }


        protected sc_event m_free = new sc_event(); // event to block on when m_value is negative
        protected int m_value; // current value of the semaphore


        public void register_port(sc_port_base port_, string if_typename_)
        {
            throw new System.NotImplementedException();
        }

        public sc_event default_event()
        {
            throw new System.NotImplementedException();
        }
    }

} // namespace sc_core
