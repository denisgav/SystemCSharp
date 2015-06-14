//****************************************************************************
//
//  sc_writer_policy.h -- The sc_signal<T> writer policy definition
//
//  Original Author: Philipp A: Hartmann, OFFIS
//
//  CHANGE LOG IS AT THE END OF THE FILE
// ****************************************************************************

namespace sc_core
{

    public enum sc_writer_policy
    {
        SC_ONE_WRITER = 0, ///< unique writer (from a unique port)
        SC_MANY_WRITERS = 1, ///< allow multiple writers (with different ports)
        SC_UNCHECKED_WRITERS = 3 ///< even allow delta cycle conflicts (non-standard)
    }

    public interface sc_check_write
    {
        bool check_write(sc_object target, bool value_changed); // value_changed -  target
        void update();
    }

    public interface sc_check_port
    {
        bool check_port(sc_object target, sc_port_base port, bool is_output);
        void update();
    }


    public class sc_writer_policy_nocheck_write : sc_check_write
    {

        public virtual bool check_write(sc_object target, bool value_changed)
        {
            return true;
        }

        public virtual void update()
        {
        }
    }

    public class sc_writer_policy_check_write : sc_check_write
    {
        public virtual bool check_write(sc_object target, bool value_changed)
        { return true; }

        public virtual void update()
        {
        }
        protected sc_writer_policy_check_write()
            : this(false)
        {
        }
        protected sc_writer_policy_check_write(bool check_delta)
        {
            m_check_delta = check_delta;
            m_writer_p = null;
        }
        protected readonly bool m_check_delta;
        protected sc_object m_writer_p;
    }

    public class sc_writer_policy_check_delta : sc_writer_policy_check_write
    {

        public sc_writer_policy_check_delta()
            : base(true)
        {
        }

        public new bool check_write(sc_object target, bool value_changed)
        {
            if (value_changed)
                return check_write(target, true);
            return true;
        }

        public new void update()
        {
            m_writer_p = null;
        }
    }

    public class sc_writer_policy_nocheck_port : sc_check_port
    {
        public virtual bool check_port(sc_object target, sc_port_base port, bool is_output)
        {
            return true;
        }


        public void update()
        {
        }
    }

    public abstract class sc_writer_policy_check_port : sc_check_port
    {

		public sc_writer_policy_check_port(sc_port_base m_output)
        {
            m_output = null;
        }
        protected sc_port_base m_output;

        public virtual bool check_port(sc_object target, sc_port_base port, bool is_output)
        {
            if (is_output && sc_simcontext.sc_get_curr_simcontext().write_check())
            {
                // an out or inout port; only one can be connected
                if (m_output != null)
                {
                    //*/*/*/*/*/*/*/*/*//*//*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*//**/*/*/*/
                    //sc_signal.sc_signal_invalid_writer(target, m_output, port, false);
                    //*/*/*/*/*/*/*/*/*//*//*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*//**/*/*/*/
                    return false;
                }
                else
                {
                    m_output = port;
                }
            }
            return true;
        }

        public void update()
        {
        }
    }

    public interface sc_writer_policy_check : sc_check_port, sc_check_write
    {
		sc_writer_policy writer_policy {
			get;
		}
    }

	public class sc_writer_policy_check_one_writer : sc_writer_policy_check
	{
		public sc_writer_policy_check_one_writer(sc_port_base m_output)
		{
			m_output = null;
		}
		protected sc_port_base m_output;

		public virtual bool check_port(sc_object target, sc_port_base port, bool is_output)
		{
			if (is_output && sc_simcontext.sc_get_curr_simcontext().write_check())
			{
				// an out or inout port; only one can be connected
				if (m_output != null)
				{
                    //*/*/*/*/*/*/*/*/*//*//*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*//**/*/*/*/
                    //sc_signal.sc_signal_invalid_writer(target, m_output, port, false);
                    //*/*/*/*/*/*/*/*/*//*//*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*/*//**/*/*/*/
					return false;
				}
				else
				{
					m_output = port;
				}
			}
			return true;
		}

		public virtual bool check_write(sc_object target, bool value_changed)
		{ return true; }



		public virtual void update()
		{
		}

		public virtual sc_writer_policy writer_policy {
			get{ return sc_writer_policy.SC_ONE_WRITER;}
		}
	}

	public class sc_writer_policy_check_many_writers : sc_writer_policy_check
	{
		public virtual bool check_port(sc_object target, sc_port_base port, bool is_output)
		{
			return true;
		}

		public virtual bool check_write(sc_object target, bool value_changed)
		{
			if (value_changed)
				return check_write(target, true);
			return true;
		}

		public virtual void update()
		{
			m_writer_p = null;
		}

		public virtual sc_writer_policy writer_policy {
			get{ return sc_writer_policy.SC_MANY_WRITERS;}
		}

		public sc_writer_policy_check_many_writers(bool check_delta)
		{
			m_check_delta = check_delta;
			m_writer_p = null;
		}
		protected readonly bool m_check_delta;
		protected sc_object m_writer_p;
	}

	public class sc_writer_policy_check_unchecked : sc_writer_policy_check
	{
		public virtual bool check_port(sc_object target, sc_port_base port, bool is_output)
		{
			return true;
		}

		public virtual bool check_write(sc_object target, bool value_changed)
		{
			return true;
		}

		public virtual void update()
		{
		}

		public virtual sc_writer_policy writer_policy {
			get{ return sc_writer_policy.SC_UNCHECKED_WRITERS;}
		}

		public sc_writer_policy_check_unchecked()
		{
		}
	}

} // namespace sc_core
