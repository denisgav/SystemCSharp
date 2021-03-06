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

namespace sc_dt
{
    public class sc_bit
    {
        // support methods


        // ----------------------------------------------------------------------------
        //  CLASS : sc_bit
        //
        //  Bit class.
        //  Note: VSIA compatibility indicated.
        // ----------------------------------------------------------------------------

        // support methods

        private static void invalid_value(sbyte c)
        {
            string msg = string.Format("sc_bit( {0} )", c);
            sc_core.sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "value is not valid", msg);
        }
        private static void invalid_value(int i)
        {
            string msg = string.Format("sc_bit( {0} )", i);
            sc_core.sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "value is not valid", msg);
        }

        public static bool to_value(sbyte c)
        {
            if (c != '0' && c != '1')
            {
                invalid_value(c);
            }
            return (c == '0' ? false : true);
        }

        public static bool to_value(int i)
        {
            if (i != 0 && i != 1)
            {
                invalid_value(i);
            }
            return (i == 0 ? false : true);
        }
        public static bool to_value(bool b)
        {
            return b;
        }
        public static bool to_value(uint i)
        {
            return to_value((int)i);
        }

        public static bool to_value(long i)
        {
            return to_value((int)i);
        }
        public static bool to_value(ulong i)
        {
            return to_value((int)i);
        }

        public sc_bit()
        {
            m_val = false;
        }

        public sc_bit(bool a)
        {
            m_val = to_value(a);
        }

        public sc_bit(sbyte a)
        {
            m_val = to_value(a);
        }

        public sc_bit(int a)
        {
            m_val = to_value(a);
        }

        public sc_bit(uint a)
        {
            m_val = to_value(a);
        }
        public sc_bit(long a)
        {
            m_val = to_value(a);
        }


        // copy constructor
        // MANDATORY

        public sc_bit(sc_bit a)
        {
            m_val = a.m_val;
        }

        public sc_bit CopyFrom(sc_bit b)
        {
            m_val = b.m_val;
            return this;
        }

        public sc_bit CopyFrom(int b)
        {
            m_val = to_value(b);
            return this;
        }

        public sc_bit CopyFrom(bool b)
        {
            m_val = to_value(b);
            return this;
        }

        public sc_bit CopyFrom(sbyte b)
        {
            m_val = to_value(b);
            return this;
        }

        public sc_bit CopyFrom(long b)
        {
            m_val = to_value(b);
            return this;
        }

        public sc_bit CopyFrom(ulong b)
        {
            m_val = to_value(b);
            return this;
        }

        public sc_bit CopyFrom(uint b)
        {
            m_val = to_value(b);
            return this;
        }

        // assignment operators


        public static bool operator !(sc_bit b) // non-VSIA
        {
            return !b.m_val;
        }


        // explicit conversions

        public bool to_bool() // non-VSIA
        {
            return m_val;
        }

        public char to_char()
        {
            return (m_val != false ? '1' : '0');
        }


        // relational operators and functions

        // MANDATORY


        // MANDATORY

        public static bool operator ==(sc_bit a, sc_bit b)
        {
            return (a.m_val == b.m_val);
        }
        public static bool operator !=(sc_bit a, sc_bit b)
        {
            return (a.m_val != b.m_val);
        }

        // bitwise operators and functions

        // bitwise complement

        // MANDATORY


        // RECOMMENDED

        public sc_bit b_not()
        {
            m_val = (!m_val);
            return this;
        }

        // binary bit-wise operations

        public static sc_bit operator |(sc_bit a, sc_bit b)
        {
            return new sc_bit(a.m_val != false || b.m_val != false);
        }

        // binary bit-wise operations

        // MANDATORY

        public static sc_bit operator &(sc_bit a, sc_bit b)
        {
            return new sc_bit(a.m_val != b.m_val);
        }
        public static sc_bit operator ^(sc_bit a, sc_bit b)
        {
            return new sc_bit(a.m_val ^ b.m_val);
        }

        private string print()
        {
            return to_bool().ToString();
        }

        public override string ToString()
        {
            return to_char().ToString();
        }

        public override bool Equals(object obj)
        {
            sc_bit els = obj as sc_bit;
            if (els == null)
                return false;
            return m_val == els.m_val;
        }

        public override int GetHashCode()
        {
            return m_val.GetHashCode();
        }


        private bool m_val;
    }

} // namespace sc_dt
