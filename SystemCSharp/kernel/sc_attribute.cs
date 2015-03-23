using System.Collections.Generic;

namespace sc_core
{
    public class sc_attr_base
    {


        public sc_attr_base(string name_)
        {
            m_name = name_;
        }
        public sc_attr_base(sc_attr_base a)
        {
            m_name = a.m_name;
        }

        public string name()
        {
            return m_name;
        }


        private string m_name;
    }


    public class sc_attr_cltn
    {
        public sc_attr_cltn()
        {
            m_cltn = new List<sc_attr_base>();
        }
        public sc_attr_cltn(sc_attr_cltn a)
        {
            m_cltn = a.m_cltn;
        }
        public void Dispose()
        {
            remove_all();
        }

        public bool push_back(sc_attr_base attribute_)
        {
            if (attribute_ == null)
            {
                return false;
            }
            for (int i = m_cltn.Count - 1; i >= 0; --i)
            {
                if (attribute_.name() == m_cltn[i].name())
                {
                    return false;
                }
            }
            m_cltn.Add(attribute_);
            return true;
        }


        public sc_attr_base this[string name_]
        {
            get
            {
                for (int i = m_cltn.Count - 1; i >= 0; --i)
                {
                    if (name_ == m_cltn[i].name())
                    {
                        return m_cltn[i];
                    }
                }
                return null;
            }
        }


        public sc_attr_base remove(string name_)
        {
            for (int i = m_cltn.Count - 1; i >= 0; --i)
            {
                if (name_ == m_cltn[i].name())
                {
                    sc_attr_base attribute = m_cltn[i];
                    int idx = m_cltn.IndexOf(attribute);
                    m_cltn.RemoveAt(idx);
                    return attribute;
                }
            }
            return null;
        }


        public void remove_all()
        {
            m_cltn.Clear();
        }

        public int size()
        {
            return m_cltn.Count;
        }

        private List<sc_attr_base> m_cltn = new List<sc_attr_base>();

    }

    public class sc_attribute<T> : sc_attr_base where T : new()
    {

        public sc_attribute(string name_)
            : base(name_)
        {
            value = new T();
        }

        public sc_attribute(string name_, T value_)
            : base(name_)
        {
            value = value_;
        }

        public sc_attribute(sc_attribute<T> a)
            : base(a.name())
        {
            value = a.value;
        }


        public T value = new T();
    }

}

internal static partial class DefineConstants
{
    public const string SC_BOOST_COMPILER = "Unknown ISO C++ Compiler";
    public const string SC_BOOST_STDLIB = "Unknown ISO standard library";
    public const int _WIN32_WINNT = 0x0400;
    public const int SC_ZERO = 0;
    public const int SC_POS = 1;
    public const int SC_NOSIGN = 2;
    public const int BITS_PER_BYTE = 8;
    public const int BYTE_RADIX = 256;
    public const int BYTE_MASK = 255;
    public const int LOG2_BITS_PER_BYTE = 3;
    public const int BYTES_PER_DIGIT_TYPE = 4;
    public const int BITS_PER_DIGIT_TYPE = 32;
    public const int BYTES_PER_DIGIT = 4;
    public const int BITS_PER_DIGIT = 30;
    public const int BITS_PER_CHAR = 8;
    public const int DIGITS_PER_CHAR = 1;
    public const int SC_INTWIDTH = 64;
    public const string SC_KERNEL_EVENT_PREFIX = "$$$$kernel_event$$$$_";
    public const int SC_HAS_PHASE_CALLBACKS_ = 1;
    public const string MSGNL = "\n             ";
    public const string CODENL = "\n  ";
    public const int QUICKTHREADS_STKALIGN = 64;
    public const int QUICKTHREADS_O7 = 16;
    public const int QUICKTHREADS_I6 = 14;
    public const int QUICKTHREADS_I5 = 13;
    public const int QUICKTHREADS_I4 = 12;
    public const int QUICKTHREADS_I3 = 11;
    public const int QUICKTHREADS_I2 = 10;
    public const int QUICKTHREADS_I1 = 9;
    public const int QUICKTHREADS_RPC = 14;
    public const int QUICKTHREADS_POP0 = 13;
    public const int QUICKTHREADS_PC = 12;
    public const int QUICKTHREADS_POP1 = 11;
    public const int QUICKTHREADS_RBP = 10;
    public const int QUICKTHREADS_R12 = 8;
    public const int QUICKTHREADS_R13 = 7;
    public const int QUICKTHREADS_R14 = 6;
    public const int QUICKTHREADS_R15 = 5;
    public const int QUICKTHREADS_RBX = 4;
    public const int QUICKTHREADS_RCX = 3;
    public const int QUICKTHREADS_RDX = 2;
    public const int QUICKTHREADS_RDI = 1;
    public const int QUICKTHREADS_RSI = 0;
    public const int QUICKTHREADS_EBX = 0;
    public const int QUICKTHREADS_EDI = 1;
    public const int QUICKTHREADS_ESI = 2;
    public const int QUICKTHREADS_EBP = 3;
    public const int QUICKTHREADS_POP2 = 6;
    public const int QUICKTHREADS_POPE = 8;
    public const int QUICKTHREADS_ARG0 = 9;
    public const int QUICKTHREADS_ARG1 = 10;
    public const int QUICKTHREADS_ARG2 = 11;
    public const int PPC_STACK_INCR = 16;
    public const int PPC_LINKAGE_AREA = 24;
    public const int PPC_CR_SAVE = 4;
    public const int PPC_LR_SAVE = 8;
    public const int QUICKTHREADS_BLOCKI_CR_SAVE = 8;
    public const int QUICKTHREADS_ARGS_MD = 0;
    public const int SYSTEMC_VERSION = 20140417;
    public const string SC_VERSION_ORIGINATOR = "Accellera";
    public const int SC_VERSION_MAJOR = 2;
    public const int SC_VERSION_MINOR = 3;
    public const int SC_VERSION_PATCH = 1;
    public const int SC_IS_PRERELEASE = 0;
    public const string SC_VERSION_RELEASE_DATE = "SYSTEMC_VERSION";
    public const string SC_VERSION_PRERELEASE = "pub_rev";
    public const int SC_BOOST_MPL_CFG_GCC = 0;
    public const int SC_SIMCONTEXT_TRACING_ = 1;
    public const int SC_DEFAULT_STACK_SIZE_ = 0x50000;
    public const string PRIu64 = "I64u";
    public const int SC_MAXTIME_ALLOWED_ = 1;
    public const int SC_DISABLE_COPYRIGHT_MESSAGE = 0;
}