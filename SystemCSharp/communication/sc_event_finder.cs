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
    //  CLASS : sc_event_finder
    //
    //  Event finder base class.
    // ----------------------------------------------------------------------------

    public abstract class sc_event_finder
    {
        public sc_port_base port()
        {
            return m_port;
        }

        // destructor (does nothing)
        public virtual void Dispose()
        {
        }

		public virtual sc_event find_event()
        {
            return find_event(null);
        }

        public abstract sc_event find_event(sc_interface if_p);


        // constructor

        // constructor

        protected sc_event_finder(sc_port_base port_)
        {
            m_port = port_;
        }

        // error reporting

        // ----------------------------------------------------------------------------
        //  CLASS : sc_event_finder
        //
        //  Event finder base class.
        // ----------------------------------------------------------------------------

        // error reporting

        protected void report_error(string id)
        {
            report_error(id, null);
        }

        protected void report_error(string id, string add_msg)
        {
            string msg = string.Empty;
            if (string.IsNullOrEmpty(add_msg) == false)
            {
                msg = string.Format("{0}: port '{1}' ({2})", add_msg, m_port.name(), m_port.kind());
            }
            else
            {
                msg = string.Format("port '{0}' ({1})", add_msg, m_port.name(), m_port.kind());
            }
            global::sc_core.sc_report_handler.report(sc_core.sc_severity.SC_ERROR, id, msg);
        }


        private readonly sc_port_base m_port; // port providing the event.

    }
    //C++ TO C# CONVERTER TODO TASK: The original C++ template specifier was replaced with a C# generic specifier, which may not produce the same behavior:
    //ORIGINAL LINE: template <class IF>


    // ----------------------------------------------------------------------------
    //  CLASS : sc_event_finder_t<IF>
    //
    //  Interface specific event finder class.
    // ----------------------------------------------------------------------------

    public class sc_event_finder_t<IF> : sc_event_finder where IF : class, sc_interface
    {

        // constructor

        public delegate sc_event event_method_Delegate();

        public sc_event_finder_t(sc_port_base port_, event_method_Delegate event_method_)
            : base(port_)
        {
            m_event_method = event_method_;
        }

        // destructor (does nothing)

        public override void Dispose()
        {
            base.Dispose();
        }

        public override sc_event find_event()
        {
            return find_event(null);
        }

        public override sc_event find_event(sc_interface if_p)
        {
            IF iface = (if_p != null) ? if_p as IF : port().get_interface() as IF;
            if (iface == null)
            {
                report_error("find event failed", "port is not bound");
            }
            return m_event_method();
        }
        private event_method_Delegate m_event_method;

    }

}