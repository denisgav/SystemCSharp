using System.Diagnostics;

namespace sc_core
{
    // ----------------------------------------------------------------------------
    //  CLASS : sc_module_name
    //
    //  Module name class.
    // ----------------------------------------------------------------------------

    public class sc_module_name
    {

        public sc_module_name(string name_)
        {
            m_name = name_;
            m_module_p = null;
            m_simc = sc_simcontext.sc_get_curr_simcontext();
            m_pushed = true;
            m_simc.get_object_manager().push_module_name(this);
        }
        public sc_module_name(sc_module_name name_)
        {
            m_name = name_.m_name;
            m_module_p = null;
            m_simc = name_.m_simc;
            m_pushed = false;
        }

        public void Dispose()
        {
            if (m_pushed)
            {
                sc_module_name smn = m_simc.get_object_manager().pop_module_name();
                if (this != smn)
                {
                    sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "incorrect use of sc_module_name", "");
                }
                if (m_module_p != null)
                    m_module_p.end_module();
            }
        }

        public override string ToString()
        {
            return m_name;
        }

        public virtual void clear_module(sc_module module_p)
        {
            Debug.Assert(m_module_p == module_p);
            m_module_p = null;
        }
        public virtual void set_module(sc_module module_p)
        {
            m_module_p = module_p;
        }

        public virtual string name()
        {
            return m_name;
        }


        private string m_name;
        private sc_module m_module_p;
        private sc_simcontext m_simc;
        private bool m_pushed;

    }

} // namespace sc_core
