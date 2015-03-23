namespace sc_dt
{

    // classes defined in this module
    //C++ TO C# CONVERTER NOTE: C# has no need of forward class declarations:
    //class sc_logic;


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

    // ----------------------------------------------------------------------------
    //  CLASS : sc_logic
    //
    //  Four-valued logic type.
    // ----------------------------------------------------------------------------

    public class sc_logic
    {

        public static readonly sc_logic_value_t[] char_to_logic = new sc_logic_value_t[]
{
     sc_logic_value_t.Log_0, sc_logic_value_t.Log_1, sc_logic_value_t.Log_Z, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X ,
     sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X ,
     sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X ,
     sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X ,
     sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X ,
     sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X ,
     sc_logic_value_t.Log_0, sc_logic_value_t.Log_1, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X ,
     sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X ,
     sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X ,
     sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X ,
     sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X ,
     sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_Z, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X ,
     sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X ,
     sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X ,
     sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X ,
     sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_Z, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X 
};

        public static readonly char[] logic_to_char = new char[] { '0', '1', 'Z', 'X' };

        public static readonly sc_logic_value_t[][] and_table = new sc_logic_value_t[][]
{
    new sc_logic_value_t[] { sc_logic_value_t.Log_0, sc_logic_value_t.Log_0, sc_logic_value_t.Log_0, sc_logic_value_t.Log_0 },
    new sc_logic_value_t[] { sc_logic_value_t.Log_0, sc_logic_value_t.Log_1, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X },
    new sc_logic_value_t[] { sc_logic_value_t.Log_0, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X },
    new sc_logic_value_t[] { sc_logic_value_t.Log_0, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X }
};

        public static readonly sc_logic_value_t[][] or_table = new sc_logic_value_t[][]
{
    new sc_logic_value_t[] { sc_logic_value_t.Log_0, sc_logic_value_t.Log_1, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X },
    new sc_logic_value_t[] { sc_logic_value_t.Log_1, sc_logic_value_t.Log_1, sc_logic_value_t.Log_1, sc_logic_value_t.Log_1 },
    new sc_logic_value_t[] { sc_logic_value_t.Log_X, sc_logic_value_t.Log_1, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X },
    new sc_logic_value_t[] { sc_logic_value_t.Log_X, sc_logic_value_t.Log_1, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X }
};

        public static readonly sc_logic_value_t[][] xor_table = new sc_logic_value_t[][]
{
    new sc_logic_value_t[] { sc_logic_value_t.Log_0, sc_logic_value_t.Log_1, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X },
    new sc_logic_value_t[] { sc_logic_value_t.Log_1, sc_logic_value_t.Log_0, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X },
    new sc_logic_value_t[] { sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X },
    new sc_logic_value_t[] { sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X }
};

        public static readonly sc_logic_value_t[] not_table = { sc_logic_value_t.Log_1, sc_logic_value_t.Log_0, sc_logic_value_t.Log_X, sc_logic_value_t.Log_X };

        // support methods


        // ----------------------------------------------------------------------------
        //  CLASS : sc_logic
        //
        //  Four-valued logic type.
        // ----------------------------------------------------------------------------

        // support methods

        private static void invalid_value(sc_logic_value_t v)
        {
            string msg = string.Format("sc_logic( {0} )", v);
            sc_core.sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "value is not valid", msg);
        }
        private static void invalid_value(sbyte c)
        {
            string msg = string.Format("sc_logic( {0} )", c);
            sc_core.sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "value is not valid", msg);
        }
        private static void invalid_value(int i)
        {
            string msg = string.Format("sc_logic( {0} )", i);
            sc_core.sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "value is not valid", msg);
        }

        public static sc_logic_value_t to_value(sc_logic_value_t v)
        {
            if (v < sc_logic_value_t.Log_0 || v > sc_logic_value_t.Log_X)
            {
                invalid_value(v);
            }
            return v;
        }

        public static sc_logic_value_t to_value(bool b)
        {
            return (b ? sc_logic_value_t.Log_1 : sc_logic_value_t.Log_0);
        }

        public static sc_logic_value_t to_value(char c)
        {
            sc_logic_value_t v;
            int index = (int)c;
            if (index > 127)
            {
                invalid_value(c);
                v = sc_logic_value_t.Log_X;
            }
            else
            {
                v = char_to_logic[index];
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
            return (sc_logic_value_t)i;
        }

        private void invalid_01()
        {
            if (m_val == sc_logic_value_t.Log_Z)
            {
                sc_core.sc_report_handler.report(sc_core.sc_severity.SC_WARNING, "sc_logic value 'Z' cannot be converted to bool", "");
            }
            else
            {
                sc_core.sc_report_handler.report(sc_core.sc_severity.SC_WARNING, "sc_logic value 'X' cannot be converted to bool", "");
            }
        }
        // conversion tables



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


        public sc_logic CopyFrom(sc_logic a)
        {
            m_val = a.m_val;
            return this;
        }



        public sc_logic_value_t value()
        {
            return m_val;
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
            return logic_to_char[(int)m_val];
        }


        private sc_logic_value_t m_val;


    }
    // #endif

} // namespace sc_dt




