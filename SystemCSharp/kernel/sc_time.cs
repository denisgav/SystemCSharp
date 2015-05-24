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


using System.Diagnostics;
using System;

namespace sc_core
{
    public enum sc_time_unit
    {
        SC_FS = 0,
        SC_PS,
        SC_NS,
        SC_US,
        SC_MS,
        SC_SEC
    }

    public class sc_time
    {

        public static readonly sc_time SC_ZERO_TIME = new sc_time();

        internal static double[] time_values = { 1, 1e3, 1e6, 1e9, 1e12, 1e15 };

        internal static string[] time_units = { "fs", "ps", "ns", "us", "ms", "s" };

        // functions for accessing the time resolution and default time unit

        public static bool isPow10(double number, double epsilon = 0.05)
        {
            if (number > 0)
            {
                for (int i = 1; i < 16; i++)
                {
                    if ((number >= (Math.Pow((double)10, i) - epsilon)) &&
                        (number <= (Math.Pow((double)10, i) + epsilon)))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static void sc_set_time_resolution(double v, sc_time_unit tu)
        {
            // first perform the necessary checks

            // must be positive
            if (v < 0.0)
            {
                sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "set time resolution failed", "value not positive");
            }

            // must be a power of ten
            if (isPow10(v) == false)
            {
                sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "set time resolution failed", "value not a power of ten");
            }

            sc_simcontext simc = sc_simcontext.sc_get_curr_simcontext();

            // can only be specified during elaboration
            if (simc.is_running())
            {
                sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "set time resolution failed", "simulation running");
            }

            sc_time_params time_params = simc.m_time_params;

            // can be specified only once
            if (time_params.time_resolution_specified)
            {
                sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "set time resolution failed", "already specified");
            }

            // can only be specified before any sc_time is constructed
            if (time_params.time_resolution_fixed)
            {
                sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "set time resolution failed", "sc_time object(s) constructed");
            }

            // must be larger than or equal to 1 fs
            double resolution = v * time_values[(int)tu];
            if (resolution < 1.0)
            {
                sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "set time resolution failed", "value smaller than 1 fs");
            }

            // recalculate the default time unit
            double time_unit = (double)(time_params.default_time_unit) * (time_params.time_resolution / resolution);
            if (time_unit < 1.0)
            {
                sc_report_handler.report(sc_core.sc_severity.SC_WARNING, "set time resolution failed", "");
                time_params.default_time_unit = 1;
            }
            else
            {
                time_params.default_time_unit = (ulong)(time_unit);
            }

            time_params.time_resolution = resolution;
            time_params.time_resolution_specified = true;
        }

        public static sc_time sc_get_time_resolution()
        {
            return sc_time.from_value(1);
        }


        public static void sc_set_default_time_unit(double v, sc_time_unit tu)
        {
            sc_report_handler.report(sc_core.sc_severity.SC_INFO, "/IEEE_Std_1666/deprecated", "deprecated function: sc_set_default_time_unit");

            // first perform the necessary checks

            // must be positive
            if (v < 0.0)
            {
                sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "set default time unit failed", "value not positive");
            }

            // must be a power of ten
            if (isPow10(v) == false)
            {
                sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "set default time unit failed", "value not a power of ten");
            }

            sc_simcontext simc = sc_simcontext.sc_get_curr_simcontext();

            // can only be specified during elaboration
            if (simc.is_running())
            {
                sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "set default time unit failed", "simulation running");
            }

            sc_time_params time_params = simc.m_time_params;

            // can only be specified before any sc_time is constructed
            if (time_params.time_resolution_fixed)
            {
                sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "set default time unit failed", "sc_time object(s) constructed");
            }

            // can be specified only once
            if (time_params.default_time_unit_specified)
            {
                sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "set default time unit failed", "already specified");
            }

            // must be larger than or equal to the time resolution
            double time_unit = (v * time_values[(int)tu]) / time_params.time_resolution;
            if (time_unit < 1.0)
            {
                sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "set default time unit failed", "value smaller than time resolution");
            }

            time_params.default_time_unit = (ulong)(time_unit);
            time_params.default_time_unit_specified = true;
        }

        public static sc_time sc_get_default_time_unit()
        {
            sc_report_handler.report(sc_core.sc_severity.SC_INFO, "/IEEE_Std_1666/deprecated", "deprecated function: sc_get_default_time_unit");
            return sc_time.from_value(sc_simcontext.sc_get_curr_simcontext().m_time_params.default_time_unit);
        }

        // constructors


        // IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIII

        // constructors

        public sc_time()
        {
            m_value = 0;
        }

        // ----------------------------------------------------------------------------
        //  CLASS : sc_time
        //
        //  The time class.
        // ----------------------------------------------------------------------------

        // constructors

        public sc_time(double v, sc_time_unit tu)
        {
            m_value = 0;
            if (v != 0)
            {
                sc_time_params time_params = sc_simcontext.sc_get_curr_simcontext().m_time_params;
                double scale_fac = time_values[(int)tu] / time_params.time_resolution;
                m_value = (ulong)(v);
                time_params.time_resolution_fixed = true;
            }
        }

        public sc_time(double v, sc_time_unit tu, sc_simcontext simc)
        {
            m_value = 0;
            if (v != 0)
            {
                sc_time_params time_params = simc.m_time_params;
                double scale_fac = time_values[(int)tu] / time_params.time_resolution;
                m_value = (ulong)(v);
                time_params.time_resolution_fixed = true;
            }
        }
        public sc_time(sc_time t)
        {
            m_value = t.m_value;
        }

        public static sc_time from_value(uint v)
        {
            return from_value((ulong)v);
        }
        public static sc_time from_value(double v)
        {
            return from_value((ulong)v);
        }
        public static sc_time from_value(ulong v)
        {
            sc_time t = new sc_time();
            if (v != 0 && !(DefineConstants.SC_MAXTIME_ALLOWED_ != 0))
            {
                sc_time_params time_params = sc_simcontext.sc_get_curr_simcontext().m_time_params;
                time_params.time_resolution_fixed = true;
            }
            t.m_value = v;
            return t;
        }

        private bool sc_time_warn_constructor = true;
        public sc_time(double v, bool scale)
        {
            m_value = 0;
            sc_report_handler.report(sc_core.sc_severity.SC_INFO, "/IEEE_Std_1666/deprecated", "deprecated constructor: sc_time(double,bool)");

            if (v != 0)
            {
                sc_time_params time_params = sc_simcontext.sc_get_curr_simcontext().m_time_params;
                if (scale)
                {
                    double scale_fac = time_params.default_time_unit;
                    m_value = (ulong)(v * scale_fac);
                }
                else
                {
                    m_value = (ulong)(v);
                }
                time_params.time_resolution_fixed = true;
            }
        }
        
        public sc_time(ulong v, bool scale)
        {
            m_value = 0;
            sc_report_handler.report(sc_core.sc_severity.SC_INFO, "/IEEE_Std_1666/deprecated", "deprecated constructor: sc_time(uint64,bool)");

            if (v != 0)
            {
                sc_time_params time_params = sc_simcontext.sc_get_curr_simcontext().m_time_params;
                if (scale)
                {
                    double scale_fac = (double)(time_params.default_time_unit);
                    double tmp = (double)v * scale_fac + 0.5;
                    m_value = (ulong)(tmp);
                }
                else
                {
                    m_value = v;
                }
                time_params.time_resolution_fixed = true;
            }
        }


        public ulong value()
        {
            return m_value;
        }

        public double to_double()
        {
            return (double)(m_value);
        }


        public double to_default_time_units()
        {
            sc_time_params time_params = sc_simcontext.sc_get_curr_simcontext().m_time_params;
            return ((double)(m_value) / (double)(time_params.default_time_unit));
        }

        public double to_seconds()
        {
            sc_time_params time_params = sc_simcontext.sc_get_curr_simcontext().m_time_params;

            return ((double)(m_value) * time_params.time_resolution * 1e-15);
        }

        public string to_string()
        {
            ulong val = m_value;
            if (val == 0)
            {
                return (string)("0 s");
            }
            sc_time_params time_params = sc_simcontext.sc_get_curr_simcontext().m_time_params;
            ulong tr = (ulong)(time_params.time_resolution);
            int n = 0;
            while ((tr % 10) == 0)
            {
                tr /= 10;
                n++;
            }
            Debug.Assert(tr == 1);
            while ((val % 10) == 0)
            {
                val /= 10;
                n++;
            }
            string buf = val.ToString();
            string result = buf;
            if (n >= 15)
            {
                for (int i = n - 15; i > 0; --i)
                {
                    result += "0";
                }
                result += " s";
            }
            else
            {
                for (int i = n % 3; i > 0; --i)
                {
                    result += "0";
                }
                result += " ";
                result += time_units[n / 3];
            }
            return result;
        }

        public override string ToString()
        {
            return to_string();
        }

        public override bool Equals(object obj)
        {
            sc_time els = obj as sc_time;
            if (els == null)
                return false;
            else
                return m_value == els.m_value;
        }

        public static bool operator ==(sc_time t1, sc_time t2)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(t1, t2))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)t1 == null) || ((object)t2 == null))
            {
                return false;
            }

            return (t1.m_value == t2.m_value);
        }

        public static bool operator !=(sc_time t1, sc_time t2)
        {
            return !(t1 == t2);
        }

        public static bool operator <(sc_time t1, sc_time t2)
        {
            if (t1 == null)
                throw new ArgumentNullException("t1");
            if (t2 == null)
                throw new ArgumentNullException("t2");
            return (t1.m_value < t2.m_value);
        }

        public static bool operator <=(sc_time t1, sc_time t2)
        {
            if (t1 == null)
                throw new ArgumentNullException("t1");
            if (t2 == null)
                throw new ArgumentNullException("t2");
            return (t1.m_value <= t2.m_value);
        }

        public static bool operator >(sc_time t1, sc_time t2)
        {
            if (t1 == null)
                throw new ArgumentNullException("t1");
            if (t2 == null)
                throw new ArgumentNullException("t2");
            return (t1.m_value > t2.m_value);
        }

        public static bool operator >=(sc_time t1, sc_time t2)
        {
            if (t1 == null)
                throw new ArgumentNullException("t1");
            if (t2 == null)
                throw new ArgumentNullException("t2");
            return (t1.m_value >= t2.m_value);
        }


        public static sc_time operator %(sc_time t1, sc_time t2)
        {
            if (t1 == null)
                throw new ArgumentNullException("t1");
            if (t2 == null)
                throw new ArgumentNullException("t2");
            sc_time tmp = new sc_time(t1);
            return tmp %= t2;
        }

        public static sc_time operator +(sc_time t1, sc_time t2)
        {
            if (t1 == null)
                throw new ArgumentNullException("t1");
            if (t2 == null)
                throw new ArgumentNullException("t2");
            return from_value(t1.m_value + t2.m_value);
        }

        public static sc_time operator -(sc_time t1, sc_time t2)
        {
            if (t1 == null)
                throw new ArgumentNullException("t1");
            if (t2 == null)
                throw new ArgumentNullException("t2");
            return from_value(t1.m_value - t2.m_value);
        }

        public static sc_time operator /(sc_time t1, double d)
        {
            if (t1 == null)
                throw new ArgumentNullException("t1");
            return from_value((double)t1.m_value / d);
        }

        public static sc_time operator *(sc_time t1, double d)
        {
            if (t1 == null)
                throw new ArgumentNullException("t1");
            return from_value((double)t1.m_value / d);
        }


        private ulong m_value;
    }


    // ----------------------------------------------------------------------------
    //  STRUCT : sc_time_params
    //
    //  Struct that holds the time resolution and default time unit.
    // ----------------------------------------------------------------------------

    public class sc_time_params
    {
        public double time_resolution; // in femto seconds
        public bool time_resolution_specified;
        public bool time_resolution_fixed;

        public ulong default_time_unit; // in time resolution
        public bool default_time_unit_specified;


        // ----------------------------------------------------------------------------
        //  STRUCT : sc_time_params
        //
        //  Struct that holds the time resolution and default time unit.
        // ----------------------------------------------------------------------------

        public sc_time_params()
        {
            time_resolution = 1000;
            time_resolution_specified = false;
            time_resolution_fixed = false;
            default_time_unit = 1000;
            default_time_unit_specified = false;
        }
    }


} // namespace sc_core


