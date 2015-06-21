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

    public class sc_spawn_options
    {
        protected List<sc_spawn_reset_base> resets_n = new List<sc_spawn_reset_base>(); // number of reset specifications to process.

        
        public sc_spawn_options()
        {
            m_dont_initialize = false;
            m_resets = new List<sc_spawn_reset_base>();
            m_sensitive_events = new List<sc_event>();
            m_sensitive_event_finders = new List<sc_event_finder>();
            m_sensitive_interfaces = new List<sc_interface>();
            m_sensitive_port_bases = new List<sc_port_base>();
            m_spawn_method = false;
            m_stack_size = 0;
            resets_n = new List<sc_spawn_reset_base>();
            foreach (sc_spawn_reset_base reset_i in m_resets)
                reset_i.specify_reset();
        }
        

        // +======================================================================
        // | CLASS sc_spawn_options (implementation)
        // |
        // +======================================================================
        
        public void Dispose()
        {
            foreach(sc_spawn_reset_base resets_i in m_resets)
                if (resets_i != null)
                    resets_i.Dispose();
        }
        /*
        public void async_reset_signal_is(sc_in<bool> port, bool level)
        {
            m_resets.Add(new sc_spawn_reset<sc_in<bool>>(true, port, level));
        }
        public void async_reset_signal_is(sc_inout<bool> port, bool level)
        {
            m_resets.Add(new sc_spawn_reset<sc_inout<bool>>(true, port, level));
        }
        public void async_reset_signal_is(sc_out<bool> port, bool level)
        {
            m_resets.Add(new sc_spawn_reset<sc_out<bool>>(true, port, level));
        }
        */
        public void async_reset_signal_is(sc_signal_in_if<bool> port, bool level)
        {
            m_resets.Add(new sc_spawn_reset<sc_signal_in_if<bool>>(true, port, level));
        }

        /*
        public void reset_signal_is(sc_in<bool> port, bool level)
        {
            m_resets.Add(new sc_spawn_reset<sc_in<bool>>(false, port, level));
        }
        public void reset_signal_is(sc_inout<bool> port, bool level)
        {
            m_resets.Add(new sc_spawn_reset<sc_inout<bool>>(false, port, level));
        }
        public void reset_signal_is(sc_out<bool> port, bool level)
        {
            m_resets.Add(new sc_spawn_reset<sc_out<bool>>(false, port, level));
        }
        */
        public void reset_signal_is(sc_signal_in_if<bool> port, bool level)
        {
            m_resets.Add(new sc_spawn_reset<sc_signal_in_if<bool>>(false, port, level));
        }
        
        public void dont_initialize()
        {
            m_dont_initialize = true;
        }

        public bool is_method()
        {
            return m_spawn_method;
        }

        public void set_stack_size(uint stack_size)
        {
            m_stack_size = stack_size;
        }

        public void set_sensitivity(sc_event e)
        {
            m_sensitive_events.Add(e);
        }

        
        public void set_sensitivity(sc_port_base port_base)
        {
            m_sensitive_port_bases.Add(port_base);
        }

        public void set_sensitivity(sc_interface interface_p)
        {
            m_sensitive_interfaces.Add(interface_p);
        }

        //\/\/\/\/\/\/\/\/\/\\/\/\/\/\/\/\/\/\/\/\/\//\/\/\/\/\/\/\/
        //public void set_sensitivity(sc_export_base export_base)
        //{
        //    m_sensitive_interfaces.Add(export_base.get_interface());
        //}
        //\/\/\/\/\/\/\/\/\/\\/\/\/\/\/\/\/\/\/\/\/\//\/\/\/\/\/\/\/

        public void set_sensitivity(sc_event_finder event_finder)
        {
            m_sensitive_event_finders.Add(event_finder);
        }
        
        public void spawn_method()
        {
            m_spawn_method = true;
        }




        public bool m_dont_initialize;
        public List<sc_spawn_reset_base> m_resets = new List<sc_spawn_reset_base>();
        public List<sc_event> m_sensitive_events = new List<sc_event>();
        protected List<sc_event_finder> m_sensitive_event_finders = new List<sc_event_finder>();
        protected List<sc_interface> m_sensitive_interfaces = new List<sc_interface>();
        protected List<sc_port_base> m_sensitive_port_bases = new List<sc_port_base>();
        public bool m_spawn_method; // Method not thread.
        public uint m_stack_size; // Thread stack size.
    }


    // +======================================================================
    // | CLASS sc_spawn_reset_base - Class to do a generic access to an 
    // |                             sc_spawn_rest object instance
    // +======================================================================
    public abstract class sc_spawn_reset_base
    {
        public sc_spawn_reset_base(bool async, bool level)
        {
            m_async = async;
            m_level = level;
        }
        public abstract void specify_reset();
        public virtual void Dispose() { }

        protected bool m_async; // = true if async reset.
        protected bool m_level; // level indicating reset.
    }

    // +======================================================================
    // | CLASS sc_spawn_reset<SOURCE>
    // |  - Reset specification for sc_spawn_options.
    // +======================================================================
    public class sc_spawn_reset<SOURCE> : sc_spawn_reset_base where SOURCE:sc_signal_in_if<bool>
    {
        public sc_spawn_reset(bool async, SOURCE source, bool level)
            : base(async, level)
        {
            m_source = source;
        }
        public override void specify_reset()
        {
            sc_reset.reset_signal_is(m_async, m_source, m_level);
        }

        protected readonly SOURCE m_source; // source of reset signal.
    }

} // namespace sc_core
