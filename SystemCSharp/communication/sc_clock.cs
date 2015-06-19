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
    //  CLASS : sc_clock
    //
    //  The clock channel.
    // ----------------------------------------------------------------------------

    public class sc_clock : sc_signal<bool>
    {


        // ----------------------------------------------------------------------------
        //  CLASS : sc_clock
        //
        //  The clock channel.
        // ----------------------------------------------------------------------------

        // constructors

        public sc_clock()
            : base(sc_writer_policy.SC_ONE_WRITER)
        {
            base_type = sc_simcontext.sc_gen_unique_name("clock");
            m_period = new sc_time();
            m_duty_cycle = new double();
            m_start_time = new sc_time();
            m_posedge_first = new bool();
            m_posedge_time = new sc_time();
            m_negedge_time = new sc_time();
            m_next_posedge_event = new sc_event((sc_constants.SC_KERNEL_EVENT_PREFIX + "_next_posedge_event"));
            m_next_negedge_event = new sc_event((sc_constants.SC_KERNEL_EVENT_PREFIX + "_next_negedge_event"));
            init(sc_time.from_value(simcontext().m_time_params.default_time_unit), 0.5, sc_time.SC_ZERO_TIME, true);
            m_next_posedge_event.notify_internal(new sc_time(m_start_time));
        }

        public sc_clock(string name_)
            : base(name_, sc_writer_policy.SC_ONE_WRITER)
        {
            base_type = name_;
            m_period = new sc_time();
            m_duty_cycle = new double();
            m_start_time = new sc_time();
            m_posedge_first = new bool();
            m_posedge_time = new sc_time();
            m_negedge_time = new sc_time();
            m_next_posedge_event = new sc_event((sc_constants.SC_KERNEL_EVENT_PREFIX + name_ + "_next_posedge_event"));
            m_next_negedge_event = new sc_event((sc_constants.SC_KERNEL_EVENT_PREFIX + name_ + "_next_negedge_event"));
            init(sc_time.from_value(simcontext().m_time_params.default_time_unit), 0.5, sc_time.SC_ZERO_TIME, true);
            m_next_posedge_event.notify_internal(new sc_time(m_start_time));
        }

        public sc_clock(string name_, sc_time period_, double duty_cycle_, sc_time start_time_)
            : this(name_, period_, duty_cycle_, start_time_, true)
        {
        }
        public sc_clock(string name_, sc_time period_, double duty_cycle_)
            : this(name_, period_, duty_cycle_, sc_time.SC_ZERO_TIME, true)
        {
        }
        public sc_clock(string name_, sc_time period_)
            : this(name_, period_, 0.5, sc_time.SC_ZERO_TIME, true)
        {
        }
        public sc_clock(string name_, sc_time period_, double duty_cycle_, sc_time start_time_, bool posedge_first_)
            : base(sc_writer_policy.SC_ONE_WRITER)
        {
            base_type = name_;
            m_period = new sc_time();
            m_duty_cycle = new double();
            m_start_time = new sc_time();
            m_posedge_first = new bool();
            m_posedge_time = new sc_time();
            m_negedge_time = new sc_time();
            m_next_posedge_event = new sc_event((sc_constants.SC_KERNEL_EVENT_PREFIX + name_ + "_next_posedge_event"));
            m_next_negedge_event = new sc_event((sc_constants.SC_KERNEL_EVENT_PREFIX + name_ + "_next_negedge_event"));
            init(new sc_time(period_), duty_cycle_, new sc_time(start_time_), posedge_first_);

            if (posedge_first_)
            {
                // posedge first
                m_next_posedge_event.notify_internal(new sc_time(m_start_time));
            }
            else
            {
                // negedge first
                m_next_negedge_event.notify_internal(new sc_time(m_start_time));
            }
        }

        public sc_clock(string name_, double period_v_, sc_time_unit period_tu_)
            : this(name_, period_v_, period_tu_, 0.5)
        {
        }

        public sc_clock(string name_, double period_v_, sc_time_unit period_tu_, double duty_cycle_)
            : base(sc_writer_policy.SC_ONE_WRITER)
        {
            base_type = name_;
            m_period = new sc_time();
            m_duty_cycle = new double();
            m_start_time = new sc_time();
            m_posedge_first = new bool();
            m_posedge_time = new sc_time();
            m_negedge_time = new sc_time();
            m_next_posedge_event = new sc_event((sc_constants.SC_KERNEL_EVENT_PREFIX + (string)name_ + "_next_posedge_event"));
            m_next_negedge_event = new sc_event((sc_constants.SC_KERNEL_EVENT_PREFIX + (string)name_ + "_next_negedge_event"));
            init(new sc_time(period_v_, period_tu_, simcontext()), duty_cycle_, sc_time.SC_ZERO_TIME, true);

            // posedge first
            m_next_posedge_event.notify_internal(new sc_time(m_start_time));
        }

        public sc_clock(string name_, double period_v_, sc_time_unit period_tu_, double duty_cycle_, double start_time_v_, sc_time_unit start_time_tu_)
            : this(name_, period_v_, period_tu_, duty_cycle_, start_time_v_, start_time_tu_, true)
        {
        }

        public sc_clock(string name_, double period_v_, sc_time_unit period_tu_, double duty_cycle_, double start_time_v_, sc_time_unit start_time_tu_, bool posedge_first_)
            : base(sc_writer_policy.SC_ONE_WRITER)
        {
            base_type = name_;
            m_period = new sc_time();
            m_duty_cycle = new double();
            m_start_time = new sc_time();
            m_posedge_first = new bool();
            m_posedge_time = new sc_time();
            m_negedge_time = new sc_time();
            m_next_posedge_event = new sc_event((sc_constants.SC_KERNEL_EVENT_PREFIX + name_ + "_next_posedge_event"));
            m_next_negedge_event = new sc_event((sc_constants.SC_KERNEL_EVENT_PREFIX + name_ + "_next_negedge_event"));
            init(new sc_time(period_v_, period_tu_, simcontext()), duty_cycle_, new sc_time(start_time_v_, start_time_tu_, simcontext()), posedge_first_);

            if (posedge_first_)
            {
                // posedge first
                m_next_posedge_event.notify_internal(new sc_time(m_start_time));
            }
            else
            {
                // negedge first
                m_next_negedge_event.notify_internal(new sc_time(m_start_time));
            }
        }

        // for backward compatibility with 1.0

        // for backward compatibility with 1.0
        private bool sc_clock_warn_sc_clock = true;
        public sc_clock(string name_, double period_, double duty_cycle_, double start_time_)
            : this(name_, period_, duty_cycle_, start_time_, true)
        {
        }
        public sc_clock(string name_, double period_, double duty_cycle_)
            : this(name_, period_, duty_cycle_, 0.0, true)
        {
        }
        public sc_clock(string name_, double period_)
            : this(name_, period_, 0.5, 0.0, true)
        {
        }
        public sc_clock(string name_, double period_, double duty_cycle_, double start_time_, bool posedge_first_)
            : base(sc_writer_policy.SC_ONE_WRITER)
        {
            base_type = name_;
            m_period = new sc_time();
            m_duty_cycle = new double();
            m_start_time = new sc_time();
            m_posedge_first = new bool();
            m_posedge_time = new sc_time();
            m_negedge_time = new sc_time();
            m_next_posedge_event = new sc_event((sc_constants.SC_KERNEL_EVENT_PREFIX + name_ + "_next_posedge_event"));
            m_next_negedge_event = new sc_event((sc_constants.SC_KERNEL_EVENT_PREFIX + name_ + "_next_negedge_event"));
            sc_report_handler.report(sc_core.sc_severity.SC_INFO, "/IEEE_Std_1666/deprecated", "\n    sc_clock(const char*, double, double, double, bool)\n" + "    is deprecated use a form that includes sc_time or\n" + "    sc_time_unit");

            sc_time default_time = sc_time.from_value(simcontext().m_time_params.default_time_unit);

            init(sc_time.from_value(period_ * default_time.to_double()), duty_cycle_, sc_time.from_value(start_time_ * default_time.to_double()), posedge_first_);

            if (posedge_first_)
            {
                // posedge first
                m_next_posedge_event.notify_internal(new sc_time(m_start_time));
            }
            else
            {
                // negedge first
                m_next_negedge_event.notify_internal(new sc_time(m_start_time));
            }
        }

        // destructor (does nothing)

        // destructor (does nothing)

        public override void Dispose()
        {
        }

        public virtual void register_port(ref sc_port_base UnnamedParameter1, string if_typename_)
        {
            string nm = if_typename_;
            if (nm == typeof(sc_signal_inout_if<bool>).Name)
            {
                sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "attempted to bind sc_clock instance to sc_inout or sc_out", "");
            }
        }
        public override void write(bool value)
        {
            sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "attempt to write the value of an sc_clock instance", "");
        }

        // get the period
        public sc_time period()
        {
            return m_period;
        }

        // get the duty cycle
        public double duty_cycle()
        {
            return m_duty_cycle;
        }


        // get the current time / clock characteristics

        public bool posedge_first()
        {
            return m_posedge_first;
        }

        public sc_time start_time()
        {
            return m_start_time;
        }


        // interface methods

        // get the current time

        public static sc_time time_stamp()
        {
            return sc_simcontext.sc_time_stamp();
        }

        public override string kind()
        {
            return "sc_clock";
        }

        public override void before_end_of_elaboration()
        {
            string gen_base;
            sc_spawn_options posedge_options = new sc_spawn_options(); // Options for posedge process.
            sc_spawn_options negedge_options = new sc_spawn_options(); // Options for negedge process.

            posedge_options.spawn_method();
            posedge_options.dont_initialize();
            posedge_options.set_sensitivity(m_next_posedge_event);
            gen_base = basename();
            gen_base += "_posedge_action";
            {
                sc_process_handle result = new sc_process_handle(new sc_spawn_object<sc_clock_posedge_callback>(new sc_clock_posedge_callback(this), sc_simcontext.sc_gen_unique_name(gen_base), posedge_options));
            };

            negedge_options.spawn_method();
            negedge_options.dont_initialize();
            negedge_options.set_sensitivity(m_next_negedge_event);
            gen_base = basename();
            gen_base += "_negedge_action";
            {
                sc_process_handle result = new sc_process_handle(new sc_spawn_object<sc_clock_negedge_callback>(new sc_clock_negedge_callback(this), sc_simcontext.sc_gen_unique_name(gen_base), negedge_options));
            };
        }

        // processes

        // IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIII

        // processes

        protected void posedge_action()
        {
            m_next_negedge_event.notify_internal(new sc_time(m_negedge_time));
            m_new_val = true;
            request_update();
        }
        protected void negedge_action()
        {
            m_next_posedge_event.notify_internal(new sc_time(m_posedge_time));
            m_new_val = false;
            request_update();
        }


        // error reporting

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
                msg = string.Format("{0}: clock '{1}'", add_msg, name());
            }
            else
            {
                msg = string.Format("clock '{1}'", add_msg);
            }
            sc_report_handler.report(sc_core.sc_severity.SC_ERROR, id, msg);
        }


        protected void init(sc_time period_, double duty_cycle_, sc_time start_time_, bool posedge_first_)
        {
            if (period_ == sc_time.SC_ZERO_TIME)
            {
                report_error("sc_clock period is zero", "increase the period");
            }
            m_period = new sc_time(period_);
            m_posedge_first = posedge_first_;

            if (duty_cycle_ <= 0.0 || duty_cycle_ >= 1.0)
            {
                m_duty_cycle = 0.5;
            }
            else
            {
                m_duty_cycle = duty_cycle_;
            }

            m_negedge_time = m_period * m_duty_cycle;
            m_posedge_time = m_period - m_negedge_time;

            if (m_negedge_time == sc_time.SC_ZERO_TIME)
            {
                report_error("sc_clock high time is zero", "increase the period or increase the duty cycle");
            }
            if (m_posedge_time == sc_time.SC_ZERO_TIME)
            {
                report_error("sc_clock low time is zero", "increase the period or decrease the duty cycle");
            }

            if (posedge_first_)
            {
                this.m_cur_val = false;
                this.m_new_val = false;
            }
            else
            {
                this.m_cur_val = true;
                this.m_new_val = true;
            }

            m_start_time = new sc_time(start_time_);

        }


        public override bool is_clock()
        {
            return true;
        }

        protected string base_type = string.Empty;

        protected sc_time m_period = new sc_time(); // the period of this clock
        protected double m_duty_cycle; // the duty cycle (fraction of period)
        protected sc_time m_start_time = new sc_time(); // the start time of the first edge
        protected bool m_posedge_first; // true if first edge is positive
        protected sc_time m_posedge_time = new sc_time(); // time till next positive edge
        protected sc_time m_negedge_time = new sc_time(); // time till next negative edge

        protected sc_event m_next_posedge_event = new sc_event();
        protected sc_event m_next_negedge_event = new sc_event();


    }


    // ----------------------------------------------------------------------------

    public class sc_clock_posedge_callback
    {
        public sc_clock_posedge_callback(sc_clock target_p)
        {
            m_target_p = target_p;
        }

        protected sc_clock m_target_p;
    }

    public class sc_clock_negedge_callback
    {
        public sc_clock_negedge_callback(sc_clock target_p)
        {
            m_target_p = target_p;
        }
        protected sc_clock m_target_p;
    }


} // namespace sc_core

