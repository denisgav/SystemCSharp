using sc_core;
namespace sc_dt
{

    // ----------------------------------------------------------------------------
    //  ENUM : sc_logic_value_t
    //
    //  Enumeration of values for sc_logic.
    // ----------------------------------------------------------------------------

    public enum sc_logic_value_t
    {
        Log_0 = 0,
        Log_1,
        Log_Z,
        Log_X
    }

    public class sc_logic
    {
        // conversion tables

        public static readonly sc_logic_value_t[] char_to_logic = { sc_logic_value_t.Log_0, sc_logic_value_t.Log_1, sc_logic_value_t.Log_Z, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_0, sc_logic_value_t.Log_1, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_Z, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_Z, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X };

        public const string logic_to_char = "01ZX";

        public static readonly sc_logic_value_t[,] and_table = { { sc_logic_value_t.Log_0, sc_logic_value_t.Log_0, sc_logic_value_t.Log_0, sc_logic_value_t.Log_0 }, { sc_logic_value_t.Log_0, sc_logic_value_t.Log_1, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X }, { sc_logic_value_t.Log_0, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X }, { sc_logic_value_t.Log_0, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X } };

        public static readonly sc_logic_value_t[,] or_table = { { sc_logic_value_t.Log_0, sc_logic_value_t.Log_1, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X }, { sc_logic_value_t.Log_1, sc_logic_value_t.Log_1, sc_logic_value_t.Log_1, sc_logic_value_t.Log_1 }, { sc_logic_value_t.Log_X, sc_logic_value_t.Log_1, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X }, { sc_logic_value_t.Log_X, sc_logic_value_t.Log_1, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X } };

        public static readonly sc_logic_value_t[,] xor_table = { { sc_logic_value_t.Log_0, sc_logic_value_t.Log_1, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X }, { sc_logic_value_t.Log_1, sc_logic_value_t.Log_0, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X }, { sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X }, { sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X } };

        public static readonly sc_logic_value_t[] not_table = { sc_logic_value_t.Log_1, sc_logic_value_t.Log_0, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X };


        private sc_logic_value_t m_val;

        // constructors

        public sc_logic()
        {
            m_val = sc_logic_value_t.Log_X;
        }

        public sc_logic(sc_logic a)
        {
            m_val = a.m_val;
        }

        public sc_logic(sc_logic_value_t v)
        {
            m_val = to_value(v);
        }

        public sc_logic(bool a)
        {
            m_val = to_value(a);
        }

        public sc_logic(sbyte a)
        {
            m_val = to_value(a);
        }

        public sc_logic(int a)
        {
            m_val = to_value(a);
        }

        public sc_logic(sc_bit a)
        {
            m_val = to_value(a.to_bool());
        }

        public override string ToString()
        {
            return string.Format("'{0}'", m_val);
        }

        public override bool Equals(object obj)
        {
            if (obj is sc_logic)
            {
                sc_logic els = obj as sc_logic;
                if (els != null)
                    return m_val.Equals(els.m_val);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return m_val.GetHashCode();
        }

        // support methods

        private static void invalid_value(sc_logic_value_t v)
        {
            string msg = string.Format("sc_logic( {0} )", v);
            sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "value is not valid", msg);
        }
        private static void invalid_value(sbyte c)
        {
            string msg = string.Format("sc_logic( {0} )", c);
            sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "value is not valid", msg);
        }
        private static void invalid_value(int i)
        {
            string msg = string.Format("sc_logic( {0} )", i);
            sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "value is not valid", msg);
        }

        private static sc_logic_value_t to_value(sc_logic_value_t v)
        {
            if (v < sc_logic_value_t.Log_0 || v > sc_logic_value_t.Log_X)
            {
                invalid_value(v);
            }
            return v;
        }

        private static sc_logic_value_t to_value(bool b)
        {
            return (b ? sc_logic_value_t.Log_1 : sc_logic_value_t.Log_0);
        }

        private static sc_logic_value_t to_value(sbyte c)
        {
            sc_logic_value_t v;
            uint index = (uint)c;
            if (index > 127)
            {
                invalid_value(c);
                v = sc_logic_value_t.Log_X;
            }
            else
            {
                v = sc_logic.char_to_logic[index];
                if (v < sc_logic_value_t.Log_0 || v > sc_logic_value_t.Log_X)
                {
                    invalid_value(c);
                }
            }
            return v;
        }

        private static sc_logic_value_t to_value(int i)
        {
            if (i < 0 || i > 3)
            {
                invalid_value(i);
            }
            return (sc_logic_value_t)(i);
        }

        private void invalid_01()
        {
            if (m_val == sc_logic_value_t.Log_Z)
            {
                sc_report_handler.report(sc_core.sc_severity.SC_WARNING, @"sc_logic value 'Z' cannot be converted to bool", string.Empty);
            }
            else
            {
                sc_report_handler.report(sc_core.sc_severity.SC_WARNING, @"sc_logic value 'X' cannot be converted to bool", string.Empty);
            }
        }

        public sc_logic b_not()
        {
            m_val = sc_logic.not_table[value_idx()];
            return this;
        }


        // explicit conversions
        public sc_logic_value_t value()
        {
            return m_val;
        }

        public int value_idx()
        {
            return (int)m_val;
        }


        public bool is_01()
        {
            return (m_val == sc_logic_value_t.Log_0 || m_val == sc_logic_value_t.Log_1);
        }

        public bool to_bool()
        {
            if (!is_01())
            {
                invalid_01();
            }
            return (m_val != sc_logic_value_t.Log_0);
        }

        public char to_char()
        {
            return sc_logic.logic_to_char[value_idx()];
        }



        // destructor

        public void Dispose()
        {
        }



        // ----------------------------------------------------------------------------

        // bitwise operators

        public static sc_logic operator &(sc_logic a, sc_logic b)
        {
            return new sc_logic(sc_logic.and_table[a.value_idx(), b.value_idx()]);
        }


        // relational operators and functions
        public static sc_logic operator &(sc_logic a, sc_logic_value_t b)
        {
            return (a & new sc_logic(b));
        }
        public static sc_logic operator &(sc_logic_value_t a, sc_logic b)
        {
            return (new sc_logic(a) & b);
        }
        public static sc_logic operator &(sc_logic a, bool b)
        {
            return (a & new sc_logic(b));
        }
        public static sc_logic operator &(bool a, sc_logic b)
        {
            return (new sc_logic(a) & b);
        }
        public static sc_logic operator &(sc_logic a, sbyte b)
        {
            return (a & new sc_logic(b));
        }
        public static sc_logic operator &(sbyte a, sc_logic b)
        {
            return (new sc_logic(a) & b);
        }
        public static sc_logic operator &(sc_logic a, int b)
        {
            return (a & new sc_logic(b));
        }
        public static sc_logic operator &(int a, sc_logic b)
        {
            return (new sc_logic(a) & b);
        }
        public static sc_logic operator |(sc_logic a, sc_logic b)
        {
            return new sc_logic( or_table[a.value_idx(), b.value_idx()]);
        }
        public static sc_logic operator |(sc_logic a, sc_logic_value_t b)
        {
            return (a | new sc_logic(b));
        }
        public static sc_logic operator |(sc_logic_value_t a, sc_logic b)
        {
            return (new sc_logic(a) | b);
        }
        public static sc_logic operator |(sc_logic a, bool b)
        {
            return (a | new sc_logic(b));
        }
        public static sc_logic operator |(bool a, sc_logic b)
        {
            return (new sc_logic(a) | b);
        }
        public static sc_logic operator |(sc_logic a, sbyte b)
        {
            return (a | new sc_logic(b));
        }
        public static sc_logic operator |(sbyte a, sc_logic b)
        {
            return (new sc_logic(a) | b);
        }
        public static sc_logic operator |(sc_logic a, int b)
        {
            return (a | new sc_logic(b));
        }
        public static sc_logic operator |(int a, sc_logic b)
        {
            return (new sc_logic(a) | b);
        }

        public static sc_logic operator ^(sc_logic a, sc_logic b)
        {
            return new sc_logic(xor_table[a.value_idx(), b.value_idx()]);
        }

        public static sc_logic operator ^(sc_logic a, sc_logic_value_t b)
        {
            return (a ^ new sc_logic(b));
        }
        public static sc_logic operator ^(sc_logic_value_t a, sc_logic b)
        {
            return (new sc_logic(a) ^ b);
        }
        public static sc_logic operator ^(sc_logic a, bool b)
        {
            return (a ^ new sc_logic(b));
        }
        public static sc_logic operator ^(bool a, sc_logic b)
        {
            return (new sc_logic(a) ^ b);
        }
        public static sc_logic operator ^(sc_logic a, sbyte b)
        {
            return (a ^ new sc_logic(b));
        }
        public static sc_logic operator ^(sbyte a, sc_logic b)
        {
            return (new sc_logic(a) ^ b);
        }
        public static sc_logic operator ^(sc_logic a, int b)
        {
            return (a ^ new sc_logic(b));
        }
        public static sc_logic operator ^(int a, sc_logic b)
        {
            return (new sc_logic(a) ^ b);
        }

        // ----------------------------------------------------------------------------
        public static bool operator ==(sc_logic a, sc_logic_value_t b)
        {
            return (a == new sc_logic(b));
        }
        public static bool operator ==(sc_logic_value_t a, sc_logic b)
        {
            return (new sc_logic(a) == b);
        }
        public static bool operator ==(sc_logic a, bool b)
        {
            return (a == new sc_logic(b));
        }
        public static bool operator ==(bool a, sc_logic b)
        {
            return (new sc_logic(a) == b);
        }
        public static bool operator ==(sc_logic a, sbyte b)
        {
            return (a == new sc_logic(b));
        }
        public static bool operator ==(sbyte a, sc_logic b)
        {
            return (new sc_logic(a) == b);
        }
        public static bool operator ==(sc_logic a, int b)
        {
            return (a == new sc_logic(b));
        }
        public static bool operator ==(int a, sc_logic b)
        {
            return (new sc_logic(a) == b);
        }
        public static bool operator !=(sc_logic a, sc_logic_value_t b)
        {
            return (a != new sc_logic(b));
        }
        public static bool operator !=(sc_logic_value_t a, sc_logic b)
        {
            return (new sc_logic(a) != b);
        }
        public static bool operator !=(sc_logic a, bool b)
        {
            return (a != new sc_logic(b));
        }
        public static bool operator !=(bool a, sc_logic b)
        {
            return (new sc_logic(a) != b);
        }
        public static bool operator !=(sc_logic a, sbyte b)
        {
            return (a != new sc_logic(b));
        }
        public static bool operator !=(sbyte a, sc_logic b)
        {
            return (new sc_logic(a) != b);
        }
        public static bool operator !=(sc_logic a, int b)
        {
            return (a != new sc_logic(b));
        }
        public static bool operator !=(int a, sc_logic b)
        {
            return (new sc_logic(a) != b);
        }

        
        public sc_logic CopyFrom(sc_logic a)
        {
            m_val = a.m_val;
            return this;
        }

    }
}