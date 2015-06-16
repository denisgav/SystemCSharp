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


using System;
using System.Text;
namespace sc_core
{
    public class sc_signal_b : sc_prim_channel
    {
        public static void sc_signal_invalid_writer(sc_object target, sc_object first_writer, sc_object second_writer, bool check_delta)
        {
            if (second_writer != null)
            {

                string msg = string.Format("\n signal `{0}' ({1})\n first driver '{2}' ({3}) \n second driver '{4}' ({5})", target.name(), target.kind(), first_writer.name(), first_writer.kind(), second_writer.name(), second_writer.kind());

                if (check_delta)
                {
                    msg = string.Format("{0}\n first conflicting write in delta cycle {1}", msg, sc_simcontext.sc_delta_count());
                }
                sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "sc_signal<T> cannot have more than one driver", msg);
            }
        }

        // to avoid code bloat in sc_signal<T>

        public static void sc_deprecated_get_data_ref()
        {
            sc_report_handler.report(sc_core.sc_severity.SC_INFO, "/IEEE_Std_1666/deprecated", "get_data_ref() is deprecated, use read() instead");
        }

        public static void sc_deprecated_get_new_value()
        {
            sc_report_handler.report(sc_core.sc_severity.SC_INFO, "/IEEE_Std_1666/deprecated", "sc_signal<T>::get_new_value() is deprecated");
        }

        public static void sc_deprecated_trace()
        {
            sc_report_handler.report(sc_core.sc_severity.SC_INFO, "/IEEE_Std_1666/deprecated", "sc_signal<T>::trace() is deprecated");
        }
        public static sc_event sc_lazy_kernel_event(sc_event[] ev, string name)
        {
            if ((ev != null) && (ev.Length != 0))
            {
                string kernel_name = sc_constants.SC_KERNEL_EVENT_PREFIX + "_" + name;
                ev = new sc_event[] { new sc_event(kernel_name) };
            }
            return ev[0];

        }
    }

    public class sc_signal<T> : sc_prim_channel, sc_signal_inout_if<T>
    {
        public sc_signal(sc_writer_policy m_writer_policy)
            : base(sc_simcontext.sc_gen_unique_name("signal"))
        {
            m_change_event_p = null;
            m_cur_val = default(T);
            m_change_stamp = ulong.MaxValue;
            m_new_val = default(T);
            this.m_writer_policy = m_writer_policy;
            writer_policy_check = sc_writer_policy_check_creator.CreateWriterPolicyCheck(this.m_writer_policy, null, true);
        }

        public sc_signal(string name_, sc_writer_policy m_writer_policy)
            : base(name_)
        {
            m_change_event_p = null;
            m_cur_val = default(T);
            m_change_stamp = ulong.MaxValue;
            m_new_val = default(T);
            this.m_writer_policy = m_writer_policy;
            writer_policy_check = sc_writer_policy_check_creator.CreateWriterPolicyCheck(this.m_writer_policy, null, true);
        }

        public sc_signal(string name_, T initial_value_, sc_writer_policy m_writer_policy)
            : base(name_)
        {
            m_change_event_p = null;
            m_cur_val = initial_value_;
            m_change_stamp = ulong.MaxValue;
            m_new_val = initial_value_;
            this.m_writer_policy = m_writer_policy;
            writer_policy_check = sc_writer_policy_check_creator.CreateWriterPolicyCheck(this.m_writer_policy, null, true);
        }

        /*
	public override void Dispose()
	{
		if (m_change_event_p != null)
			m_change_event_p.Dispose();
		base.Dispose();
	}
        */

        public override void Dispose()
        {
            if (m_change_event_p != null)
                m_change_event_p.Dispose();
            if (m_negedge_event_p != null)
                m_negedge_event_p.Dispose();
            if (m_posedge_event_p != null)
                m_posedge_event_p.Dispose();
            base.Dispose();
        }
        public virtual sc_writer_policy get_writer_policy()
        {
            return m_writer_policy;
        }


        // get the value changed event
        public virtual sc_event value_changed_event()
        {
            return sc_signal_b.sc_lazy_kernel_event(new sc_event[] { m_change_event_p }, "value_changed_event");
        }


        // read the current value
        public virtual T read()
        {
            return m_cur_val;
        }

        // get a reference to the current value (for tracing)
        public virtual T get_data_ref()
        {
            sc_signal_b.sc_deprecated_get_data_ref();
            return m_cur_val;
        }


        // was there an event?
        public virtual bool Event()
        {
            return simcontext().event_occurred(m_change_stamp);
        }


        public sc_signal<T> CopyFrom(T a)
        {
            write(a);
            return this;
        }
        
        public sc_signal<T> CopyFrom(sc_signal_in_if_param<T> a)
        {
            write(a.read());
            return this;
        }
        
        public sc_signal<T> CopyFrom(sc_signal<T> a)
        {
            write(a.read());
            return this;
        }

        public T get_new_value()
        {
            sc_signal_b.sc_deprecated_get_new_value();
            return m_new_val;
        }


        //\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\//\/\/\/\/\/\//\
        /*
        	public new void trace(sc_trace_file tf)
	{
		GlobalMembersSc_signal.sc_deprecated_trace();
			GlobalMembersSc_clock.sc_trace(tf, read(), name());

				if (tf != null)
				{
				}

	}
         */
        //\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\//\/\/\/\/\/\//\

        public override string kind()
        {
            return "sc_signal";
        }



        // IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIII

        public void register_port(sc_port_base port_, string if_typename_)
        {

            bool is_output = if_typename_.Equals(typeof(sc_signal_inout_if<T>).Name);
            if (!writer_policy_check.check_port(this, port_, is_output))
                throw new NotImplementedException();
        }

        // write the new value

        public virtual void write(T value_)
        {
            bool value_changed = !(m_cur_val.Equals(value_));
            if (!writer_policy_check.check_write(this, value_changed))
                return;

            m_new_val = value_;
            if (value_changed)
            {
                request_update();
            }
        }

        public virtual string print()
        {
            return m_cur_val.ToString();
        }

        public virtual string dump()
        {
            StringBuilder res = new StringBuilder();
            res.AppendLine(string.Format("name = {0}", name()));
            res.AppendLine(string.Format("value = {0}", m_cur_val));
            res.AppendLine(string.Format("new value = ", m_new_val));
            return res.ToString();
        }

        //ORIGINAL LINE: void sc_signal<T,POL>::update()
        public virtual void update()
        {
            writer_policy_check.update();
            if (!(m_new_val.Equals(m_cur_val)))
            {
                do_update();
            }
        }

        public virtual void do_update()
        {
            m_cur_val = m_new_val;
            if (m_change_event_p != null)
                m_change_event_p.notify_next_delta();
            m_change_stamp = simcontext().change_stamp();
        }

        // get the default event
        public virtual sc_event default_event()
        {
            return value_changed_event();
        }


        // was there a positive edge event?
        public virtual bool posedge()
        {
            return (Event() && m_cur_val != null);
        }

        // was there a negative edge event?
        public virtual bool negedge()
        {
            return (Event() && m_cur_val == null);
        }


        /*
	public new void trace(sc_trace_file tf)
	{
		GlobalMembersSc_signal.sc_deprecated_trace();
#if DEBUG_SYSTEMC
			GlobalMembersSc_clock.sc_trace(tf, read(), name());
//#else
				if (tf != null)
				{
				}
//#endif
	}
        */

        protected virtual bool is_clock()
        {
            return false;
        }

        protected sc_event m_change_event_p; // value change event if present.
        protected sc_event m_negedge_event_p; // negative edge event if present.
        protected sc_event m_posedge_event_p; // positive edge event if present.
        protected sc_reset m_reset_p; // reset mechanism if present.

        private sc_writer_policy_check writer_policy_check;
        private sc_writer_policy m_writer_policy;
        protected T m_cur_val = default(T);
        protected ulong m_change_stamp; // delta of last event
        protected T m_new_val = default(T);
    }

}

