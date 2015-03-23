using System.Collections.Generic;
using System;
namespace sc_core
{

    //C++ TO C# CONVERTER NOTE: C# has no need of forward class declarations:
    //class sc_simcontext;
    //C++ TO C# CONVERTER NOTE: C# has no need of forward class declarations:
    //class sc_object;

    public partial class sc_phase_callback_registry : IDisposable
    {
        internal const uint SC_PHASE_CALLBACK_MASK = (int)sc_status.SC_STATUS_ANY;

        public sc_phase_callback_registry(sc_simcontext m_simc)
        {
            this.m_simc = m_simc;
        }

        public static void erase_remove<T>(List<T> vec, T t)
        {
            vec.Clear();
        }

        internal static void warn_phase_callbacks(sc_core.sc_object obj)
        {
            string ss = obj.name() + "Please recompile SystemC with SC_ENABLE_SIMULATION_PHASE_CALLBACKS\" defined.";
            sc_report_handler.report(sc_core.sc_severity.SC_WARNING, "simulation phase callbacks not enabled", ss);
        }


        public class entry
        {

            public sc_object target;
            public uint mask;

        }

        public virtual void Dispose()
        {
        }

        // --- callback forwarders


        // -------------------- callback implementations --------------------
        //                   (empty, if feature is disabled)

        private bool construction_done()
        {
            do_callback(sc_status.SC_BEFORE_END_OF_ELABORATION);
            return false;
        }

        private void elaboration_done()
        {
            do_callback(sc_status.SC_END_OF_ELABORATION);
        }

        private void initialization_done()
        {
            do_callback(sc_status.SC_END_OF_INITIALIZATION);
        }

        private void start_simulation()
        {
            do_callback(sc_status.SC_START_OF_SIMULATION);
        }


        private void update_done()
        {
            throw new NotImplementedException();
        }

        private void before_timestep()
        {
            throw new NotImplementedException();
        }


        private void simulation_paused()
        {
            do_callback(sc_status.SC_PAUSED);
        }

        private void simulation_stopped()
        {
            do_callback(sc_status.SC_STOPPED);
        }
        private void simulation_done()
        {
            do_callback(sc_status.SC_END_OF_SIMULATION);
        }


        private void do_callback(sc_status s)
        {
            throw new NotImplementedException();
        }



        private sc_simcontext m_simc;
        private List<entry> m_cb_vec = new List<entry>();
        private List<sc_object> m_cb_update_vec = new List<sc_object>();
        private List<sc_object> m_cb_timestep_vec = new List<sc_object>();
    }
}