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

namespace sc_core
{

    // ----------------------------------------------------------------------------
    //  CLASS : sc_module_registry
    //
    //  Registry for all modules.
    //  FOR INTERNAL USE ONLY!
    // ----------------------------------------------------------------------------

    public class sc_module_registry
    {
        // ----------------------------------------------------------------------------
        //  CLASS : sc_module_registry
        //
        //  Registry for all modules.
        //  FOR INTERNAL USE ONLY!
        // ----------------------------------------------------------------------------

        public void insert(sc_module module_)
        {
            if (m_simc.is_running())
            {
                sc_report_handler.report(sc_severity.SC_ERROR, "insert module failed", "simulation running");
            }

            if (m_simc.elaboration_done())
            {
                global::sc_core.sc_report_handler.report(sc_severity.SC_ERROR, "insert module failed", "elaboration done");
            }


            // insert
            m_module_vec.Add(module_);
        }
        public void remove(sc_module module_)
        {
            if (m_module_vec.Contains(module_))
                m_module_vec.Remove(module_);
            else
                global::sc_core.sc_report_handler.report(sc_severity.SC_ERROR, "remove module failed", "");
        }

        public int size()
        {
            return m_module_vec.Count;
        }



        public sc_module_registry(sc_simcontext simc_)
        {
            m_construction_done = 0;
            m_module_vec = new List<sc_module>();
            m_simc = simc_;
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
                m_module_vec[m_construction_done].construction_done();
            }
            return false;
        }

        // called when elaboration is done

        // called when elaboration is done

        public void elaboration_done()
        {
            bool error = false;
            for (int i = 0; i < size(); ++i)
            {
                m_module_vec[i].elaboration_done(ref error);
            }
        }

        // called before simulation begins

        // called before simulation begins

        public void start_simulation()
        {
            for (int i = 0; i < size(); ++i)
            {
                m_module_vec[i].start_simulation();
            }
        }

        // called after simulation ends

        // called after simulation ends

        public void simulation_done()
        {
            for (int i = 0; i < size(); ++i)
            {
                m_module_vec[i].simulation_done();
            }
        }



        private int m_construction_done;
        private List<sc_module> m_module_vec = new List<sc_module>();
        private sc_simcontext m_simc;
    }

} // namespace sc_core