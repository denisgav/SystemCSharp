using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemCSharp.Kernel
{
    public enum SimulationTimeUnit
    {
        FS = 0,
        PS,
        NS,
        US,
        MS,
        SEC
    };

    public class SimulationTime
    {
        private static double[] time_values = new double[] {
            1,       // fs
            1e3,     // ps
            1e6,     // ns
            1e9,     // us
            1e12,    // ms
            1e15     // s
        };

        private static string[] time_units = new string[] {
            "fs",
            "ps",
            "ns",
            "us",
            "ms",
            "s"
        };

        private static SimulationTime zeroTime;
        public static SimulationTime ZeroTime
        {
            get
            {
                if (zeroTime == null)
                    zeroTime = FromUInt64(0);
                return zeroTime;
            }
        }

        private static SimulationTime maxTime;
        public static SimulationTime MaxTime
        {
            get
            {
                if (maxTime == null)
                    maxTime = FromUInt64(UInt64.MaxValue);
                return maxTime;
            }
        }

        private UInt64 _value;

        private SimulationContext currentSimContext;
        public SimulationContext CurrentSimContext
        {
            get
            {
                if (currentSimContext == null)
                {
                    currentSimContext = SimulationContext.GlobalSimContext;
                }
                return currentSimContext;
            }
        }


        // constructors

        public SimulationTime()
        { }

        public SimulationTime(double _value, SimulationTimeUnit unit)
            : this(_value, unit, SimulationContext.GlobalSimContext)
        {

        }
        public SimulationTime(double _value, SimulationTimeUnit unit, SimulationContext context)
        {
            if (_value != 0)
            {
                this.currentSimContext = context;
                SimulationTimeParameters time_params = context.TimeParameters;
                double scale_fac = time_values[(int)unit] / time_params.TimeResolution;
                // linux bug workaround; don't change next two lines
                double tmp = _value * scale_fac + 0.5;
                this._value = (UInt64)tmp;
                time_params.IsTimeResolutionFixed = true;
            }
        }
        public SimulationTime(SimulationTime time)
        {
            this._value = time._value;
            this.currentSimContext = time.currentSimContext;
        }

        public static SimulationTime FromUInt64(UInt64 v)
        {
            if (v != 0)
            {
                SimulationTimeParameters time_params = SimulationContext.GlobalSimContext.TimeParameters;
                time_params.IsTimeResolutionFixed = true;
            }
            SimulationTime t = new SimulationTime(v, false);
            return t;
        }

        // deprecated, use from_value(v)
        public SimulationTime(double v, bool scale)
        {
            if (v != 0)
            {
                SimulationTimeParameters time_params = CurrentSimContext.TimeParameters;
                if (scale)
                {
                    double scale_fac = time_params.DefaultTimeUnit;
                    // linux bug workaround; don't change next two lines
                    double tmp = v * scale_fac + 0.5;
                    this._value = (UInt64)tmp;
                }
                else
                {
                    // linux bug workaround; don't change next two lines
                    double tmp = v + 0.5;
                    this._value = (UInt64)tmp;
                }
                time_params.IsTimeResolutionFixed = true;
            }
        }
        public SimulationTime(UInt64 v, bool scale)
        {
            if (v != 0)
            {
                SimulationTimeParameters time_params = CurrentSimContext.TimeParameters;
                if (scale)
                {
                    double scale_fac = time_params.DefaultTimeUnit;
                    // linux bug workaround; don't change next two lines
                    double tmp = v * scale_fac + 0.5;
                    this._value = (UInt64)tmp;
                }
                else
                {
                    // linux bug workaround; don't change next two lines
                    double tmp = v + 0.5;
                    this._value = (UInt64)tmp;
                }
                time_params.IsTimeResolutionFixed = true;
            }
        }


        public UInt64 Value { get { return _value; } }

        public static implicit operator double(SimulationTime time)
        {
            return time._value;
        }

        public static bool operator ==(SimulationTime left, SimulationTime right)
        {
            return left._value == right._value;
        }

        public static bool operator !=(SimulationTime left, SimulationTime right)
        {
            return left._value != right._value;
        }

        public static bool operator <(SimulationTime left, SimulationTime right)
        {
            return left._value < right._value;
        }

        public static bool operator <=(SimulationTime left, SimulationTime right)
        {
            return left._value <= right._value;
        }

        public static bool operator >(SimulationTime left, SimulationTime right)
        {
            return left._value > right._value;
        }

        public static bool operator >=(SimulationTime left, SimulationTime right)
        {
            return left._value >= right._value;
        }

        public static SimulationTime operator +(SimulationTime left, SimulationTime right)
        {
            return FromUInt64(left._value + right._value);
        }

        public static SimulationTime operator -(SimulationTime left, SimulationTime right)
        {
            return FromUInt64(left._value + right._value);
        }

        public static SimulationTime operator *(SimulationTime left, double right)
        {
            return FromUInt64((UInt64)(left._value * right));
        }

        public static SimulationTime operator /(SimulationTime left, double right)
        {
            return FromUInt64((UInt64)(left._value / right));
        }

        public static double operator /(SimulationTime left, SimulationTime right)
        {
            return (double)left._value / (double)right._value;
        }

        public double ToDouble()   // relative to the time resolution
        {
            return _value;
        }

        public double ToDefaultTimeUnits()
        {
            SimulationTimeParameters time_params = CurrentSimContext.TimeParameters;
            return ((double)_value / (double)time_params.DefaultTimeUnit);
        }

        public double ToSeconds()
        {
            SimulationTimeParameters time_params = CurrentSimContext.TimeParameters;
            return ((double)_value * time_params.TimeResolution * 1e-15);
        }

        public override bool Equals(object obj)
        {
            SimulationTime els = obj as SimulationTime;
            if (els == null)
                return false;
            else
                return els._value == _value;
        }

        public override string ToString()
        {
            UInt64 val = _value;
            if (val == 0)
            {
                return "0 s";
            }
            SimulationTimeParameters time_params = CurrentSimContext.TimeParameters;
            UInt64 tr = (UInt64)(time_params.TimeResolution);
            int n = 0;
            while ((tr % 10) == 0)
            {
                tr /= 10;
                n++;
            }

            while ((val % 10) == 0)
            {
                val /= 10;
                n++;
            }

            StringBuilder result = new StringBuilder();

            result.Append(val);
            if (n >= 15)
            {
                for (int i = n - 15; i > 0; --i)
                {
                    result.Append("0");
                }
                result.Append(" s");
            }
            else
            {
                for (int i = n % 3; i > 0; --i)
                {
                    result.Append("0");
                }
                result.Append(" ");
                result.Append(time_units[n / 3]);
            }

            return result.ToString();
        }



        // functions for accessing the time resolution and default time unit
        public static void SetTimeResolution(double v, SimulationTimeUnit tu)
        {
            // first perform the necessary checks

            // must be positive
            if (v < 0.0)
            {
                throw new ArgumentException("Value should be positive");
            }

            double log10 = Math.Log10(v);
            if (Math.Pow(10, log10) != v)
                throw new ArgumentException("Value should be power of ten");

            SimulationContext simc = SimulationContext.GlobalSimContext;

            // can only be specified during elaboration
            if (simc.IsRunning)
            {
                throw new Exception("simulation running. could not chande time resolution");
            }

            SimulationTimeParameters time_params = simc.TimeParameters;

            // can be specified only once
            if (time_params.IsTimeResolutionSpecified)
            {
                throw new Exception("time resolution already specified");
            }

            // can only be specified before any sc_time is constructed
            if (time_params.IsTimeResolutionFixed)
            {
                throw new Exception("SimulationTime object(s) constructed");
            }

            // must be larger than or equal to 1 fs
            double resolution = v * time_values[(int)tu];
            if (resolution < 1.0)
            {
                throw new ArgumentException("value smaller than 1 fs");
            }

            // recalculate the default time unit
            double time_unit = time_params.DefaultTimeUnit * (time_params.TimeResolution / resolution);
            if (time_unit < 1.0)
            {
                time_params.DefaultTimeUnit = 1;
            }
            else
            {
                time_params.DefaultTimeUnit = (UInt64)time_unit;
            }

            time_params.TimeResolution = resolution;
            time_params.IsTimeResolutionSpecified = true;
        }

        public static SimulationTime GetTimeResolution()
        {
            return FromUInt64(1);
        }

        public static void SetDefaultTimeUnit(double v, SimulationTimeUnit tu)
        {
            // first perform the necessary checks

            // must be positive
            if (v < 0.0)
            {
                throw new ArgumentException("Value should be positive");
            }

            double log10 = Math.Log10(v);
            if (Math.Pow(10, log10) != v)
                throw new ArgumentException("Value should be power of ten");

            SimulationContext simc = SimulationContext.GlobalSimContext;
            SimulationTimeParameters time_params = simc.TimeParameters;

            // can be specified only once
            if (time_params.IsDefaultTimeUnitSpecified)
            {
                throw new Exception("default time unit already specified");
            }

            // can only be specified before any sc_time is constructed
            if (time_params.IsTimeResolutionFixed)
            {
                throw new Exception("SimulationTime object(s) constructed");
            }

            // must be larger than or equal to the time resolution
            double time_unit = (v * time_values[(int)tu]) /
                                    time_params.TimeResolution;
            if (time_unit < 1.0)
            {
                throw new Exception("value smaller than time resolution");
            }

            time_params.DefaultTimeUnit = (UInt64)time_unit;
            time_params.IsDefaultTimeUnitSpecified = true;
        }

        public static SimulationTime GetDefaultTimeUnit()
        {
            return FromUInt64(
                      SimulationContext.GlobalSimContext.TimeParameters.DefaultTimeUnit
                   );
        }
    }


    public class SimulationTimeParameters
    {
        private double time_resolution;		// in femto seconds
        public double TimeResolution
        {
            get { return time_resolution; }
            set { time_resolution = value; }
        }

        private bool time_resolution_specified;
        public bool IsTimeResolutionSpecified
        {
            get { return time_resolution_specified; }
            set { time_resolution_specified = value; }
        }

        private bool time_resolution_fixed;
        public bool IsTimeResolutionFixed
        {
            get { return time_resolution_fixed; }
            set { time_resolution_fixed = value; }
        }

        private UInt64 default_time_unit;		// in time resolution
        public UInt64 DefaultTimeUnit
        {
            get { return default_time_unit; }
            set { default_time_unit = value; }
        }

        private bool default_time_unit_specified;
        public bool IsDefaultTimeUnitSpecified
        {
            get { return default_time_unit_specified; }
            set { default_time_unit_specified = value; }
        }

        public SimulationTimeParameters(double time_resolution, bool time_resolution_fixed, UInt64 default_time_unit)
        {
            this.time_resolution = time_resolution;
            this.time_resolution_specified = true;
            this.time_resolution_fixed = time_resolution_fixed;
            this.default_time_unit = default_time_unit;
            this.default_time_unit_specified = true;
        }

        public SimulationTimeParameters()
        {
            this.time_resolution = (1000);		// default 1 ps
            this.time_resolution_specified = (false);
            this.time_resolution_fixed = (false);
            this.default_time_unit = (1000);		// default 1 ns
            this.default_time_unit_specified = (false);
        }
    }
}
