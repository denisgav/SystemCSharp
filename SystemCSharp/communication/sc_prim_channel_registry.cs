using System.Collections.Generic;

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

//****************************************************************************
//
//  sc_prim_channel.cpp -- Abstract base class of all primitive channel
//                         classes.
//
//  Original Author: Martin Janssen, Synopsys, Inc., 2001-05-21
//
//  CHANGE LOG IS AT THE END OF THE FILE
// ****************************************************************************

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

//****************************************************************************
//
//  sc_prim_channel.h -- Abstract base class of all primitive channel classes.
//
//  Original Author: Martin Janssen, Synopsys, Inc., 2001-05-21
//
// CHANGE LOG AT THE END OF THE FILE
// ****************************************************************************



namespace sc_core
{
    // ----------------------------------------------------------------------------
    //  CLASS : sc_prim_channel_registry
    //
    //  Registry for all primitive channels.
    //  FOR INTERNAL USE ONLY!
    // ----------------------------------------------------------------------------

    public class sc_prim_channel_registry
    {
        // ----------------------------------------------------------------------------
        //  CLASS : sc_prim_channel_registry
        //
        //  Registry for all primitive channels.
        //  FOR INTERNAL USE ONLY!
        // ----------------------------------------------------------------------------

        public void insert(sc_prim_channel prim_channel_)
        {
            if (m_simc.is_running())
            {
                sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "insert primitive channel failed", "simulation running");
            }

            if (m_simc.elaboration_done())
            {

                sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "insert primitive channel failed", "elaboration done");
            }

            // insert
            m_prim_channel_vec.Add(prim_channel_);

        }
        public void remove(sc_prim_channel prim_channel_)
        {
            int i;
            for (i = 0; i < size(); ++i)
            {
                if (prim_channel_ == m_prim_channel_vec[i])
                {
                    break;
                }
            }
            if (i == size())
            {
                sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "insert primitive channel failed", string.Empty);
            }

            m_prim_channel_vec.RemoveAt(i);
        }


        public int size()
        {
            return m_prim_channel_vec.Count;
        }


        // IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIII

        // ----------------------------------------------------------------------------
        //  CLASS : sc_prim_channel_registry
        //
        //  Registry for all primitive channels.
        //  FOR INTERNAL USE ONLY!
        // ----------------------------------------------------------------------------

        public void request_update(sc_prim_channel prim_channel_)
        {
            prim_channel_.m_update_next_p = m_update_list_p;
            m_update_list_p = prim_channel_;
        }
        public void async_request_update(sc_prim_channel prim_channel_)
        {
            m_async_update_list_p.append(prim_channel_);
        }

        public bool pending_updates()
        {
            return m_update_list_p != (sc_prim_channel)sc_prim_channel.list_end || pending_async_updates();
        }

        public bool pending_async_updates()
        {
            return m_async_update_list_p.pending();
        }


        // constructor

        // constructor

        public sc_prim_channel_registry(sc_simcontext simc_)
        {
            m_async_update_list_p = null;
            m_construction_done = 0;
            m_prim_channel_vec = new List<sc_prim_channel>();
            m_simc = simc_;
            m_update_list_p = (sc_prim_channel)sc_prim_channel.list_end;
            m_async_update_list_p = new async_update_list();
        }

        // destructor

        // destructor

        public void Dispose()
        {
            //if (m_async_update_list_p != null)
            //    m_async_update_list_p.Dispose();
        }

        // called during the update phase of a delta cycle

        // +----------------------------------------------------------------------------
        // |"sc_prim_channel_registry::perform_update"
        // |
        // | This method updates the values of the primitive channels in its update
        // | lists.
        // +----------------------------------------------------------------------------
		public void perform_update()
        {
            // Update the values for the primitive channels set external to the
            // simulator.

            if (m_async_update_list_p.pending())
                m_async_update_list_p.accept_updates();

            sc_prim_channel next_p; // Next update to perform.
            sc_prim_channel now_p; // Update now performing.

            // Update the values for the primitive channels in the simulator's list.

            now_p = m_update_list_p;
            m_update_list_p = (sc_prim_channel)sc_prim_channel.list_end;
            for (; now_p != (sc_prim_channel)sc_prim_channel.list_end; now_p = next_p)
            {
                next_p = now_p.m_update_next_p;
                now_p.perform_update();
            }
        }

        // called when construction is done

        // called when construction is done

		public bool construction_done()
        {
            if (size() == m_construction_done)
                // nothing has been updated
                return true;

            for (; m_construction_done < size(); ++m_construction_done)
            {
                m_prim_channel_vec[m_construction_done].construction_done();
            }

            return false;
        }

        // called when elaboration is done

        // called when elaboration is done

		public void elaboration_done()
        {
            for (int i = 0; i < size(); ++i)
            {
                m_prim_channel_vec[i].elaboration_done();
            }
        }

        // called before simulation starts

        // called before simulation begins

		public void start_simulation()
        {
            for (int i = 0; i < size(); ++i)
            {
                m_prim_channel_vec[i].start_simulation();
            }
        }

        // called after simulation ends

        // called after simulation ends

		public void simulation_done()
        {
            for (int i = 0; i < size(); ++i)
            {
                m_prim_channel_vec[i].simulation_done();
            }
        }


        private class async_update_list
        {
            public bool pending()
            {
                return m_push_queue.Count != 0;
            }

            public void append(sc_prim_channel prim_channel_)
            {
                sc_scoped_lock @lock = new sc_scoped_lock(m_mutex);
                m_push_queue.Add(prim_channel_);
                // return releases the mutex
            }

            public void accept_updates()
            {
                System.Diagnostics.Debug.Assert(m_pop_queue.Count==0);
                {
                    sc_scoped_lock @lock = new sc_scoped_lock(m_mutex);
                    m_push_queue = m_pop_queue;
                    // leaving the block releases the mutex
                }

				foreach (sc_prim_channel ch in m_pop_queue) {
					ch.request_update ();
				}

                m_pop_queue.Clear();
            }

            private sc_host_mutex m_mutex = new sc_host_mutex();
            private List<sc_prim_channel> m_push_queue = new List<sc_prim_channel>();
            private List<sc_prim_channel> m_pop_queue = new List<sc_prim_channel>();

        }

        private async_update_list m_async_update_list_p; // external updates.
        private int m_construction_done; // # of constructs.
        private List<sc_prim_channel> m_prim_channel_vec = new List<sc_prim_channel>(); // existing channels.
        private sc_simcontext m_simc; // simulator context.
        private sc_prim_channel m_update_list_p; // internal updates.
    }
}