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
    //  CLASS : sc_mutex
    //
    //  The sc_mutex primitive channel class.
    // ----------------------------------------------------------------------------

    public class sc_mutex : sc_object, sc_mutex_if
    {

        // constructors and destructor


        // ----------------------------------------------------------------------------
        //  CLASS : sc_mutex
        //
        //  The sc_mutex primitive channel class.
        // ----------------------------------------------------------------------------

        // constructors

        public sc_mutex()
            : base(sc_simcontext.sc_gen_unique_name("mutex"))
        {
            m_owner = null;
            m_free = new sc_event((sc_constants.SC_KERNEL_EVENT_PREFIX + "_free_event"));
        }
        public sc_mutex(string name_)
            : base(name_)
        {
            m_owner = null;
            m_free = new sc_event((sc_constants.SC_KERNEL_EVENT_PREFIX + "_free_event"));
        }

        // destructor

        public override void Dispose()
        {
            base.Dispose();
        }


        // interface methods

        // blocks until mutex could be locked

        // interface methods

        // blocks until mutex could be locked

		public virtual int @lock()
        {
            if (m_owner == sc_simcontext.sc_get_current_process_b())
                return 0;
            while (in_use())
            {
                sc_wait.wait(m_free, sc_simcontext.sc_get_curr_simcontext());
            }
            m_owner = sc_simcontext.sc_get_current_process_b();
            return 0;
        }

        // returns -1 if mutex could not be locked

        // returns -1 if mutex could not be locked

		public virtual int trylock()
        {
            if (m_owner == sc_simcontext.sc_get_current_process_b())
                return 0;
            if (in_use())
            {
                return -1;
            }
            m_owner = sc_simcontext.sc_get_current_process_b();
            return 0;
        }

        // returns -1 if mutex was not locked by caller
		public virtual int unlock()
        {
            if (m_owner != sc_simcontext.sc_get_current_process_b())
            {
                return -1;
            }
            m_owner = null;
            m_free.notify();
            return 0;
        }

        public override string kind()
        {
            return "sc_mutex";
        }


        // support methods


        protected bool in_use()
        {
            return (m_owner != null);
        }


        protected sc_process_b m_owner;
        protected sc_event m_free = new sc_event();
    }

} // namespace sc_core
