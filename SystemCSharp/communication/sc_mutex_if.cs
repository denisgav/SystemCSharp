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
    //  CLASS : sc_mutex_if
    //
    //  The sc_mutex_if interface class.
    // ----------------------------------------------------------------------------

    public interface sc_mutex_if
    {

        // the classical operations: lock(), trylock(), and unlock()

        // blocks until mutex could be locked
        int @lock();

        // returns -1 if mutex could not be locked
        int trylock();

        // returns -1 if mutex was not locked by caller
        int unlock();
    }

    // ----------------------------------------------------------------------------
    //  CLASS : sc_scoped_lock
    //
    //  The sc_scoped_lock class to lock (and automatically release) a mutex.
    // ----------------------------------------------------------------------------

    public class sc_scoped_lock
    {
        //typedef Lockable lockable_type;
        public sc_scoped_lock(sc_mutex_if mtx)
        {
            m_ref = mtx;
            m_active = true;
            m_ref.@lock();
        }

        public bool release()
        {
            if (m_active)
            {
                m_ref.unlock();
                m_active = false;
                return true;
            }
            return false;
        }

        public void Dispose()
        {
            release();
        }

        private sc_mutex_if m_ref;
        private bool m_active;
    }

} // namespace sc_core

