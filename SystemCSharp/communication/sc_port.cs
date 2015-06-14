using System.Diagnostics;
using System.Collections.Generic;

namespace sc_core
{
	public class sc_port<IF> : sc_port_b<IF> where IF : class, sc_interface
    {
        // ----------------------------------------------------------------------------
        //  CLASS : sc_port
        //
        //  Generic port class and base class for other port classes.
        //  N is the maximum number of channels (with interface IF) that can be bound
        //  to this port. N <= 0 means no maximum.
        // ----------------------------------------------------------------------------

        public static void sc_warn_port_constructor()
        {
            sc_report_handler.report(sc_core.sc_severity.SC_INFO, "/IEEE_Std_1666/deprecated", "interface and/or port binding in port constructors is deprecated");
        }

        // constructors

        protected sc_port(int max_size_)
            : this(max_size_, sc_port_policy.SC_ONE_OR_MORE_BOUND)
        {
        }

        protected sc_port(int max_size_, sc_port_policy policy)
            : base(max_size_, policy)
        {
        }

        protected sc_port(string name_, int max_size_)
            : this(name_, max_size_, sc_port_policy.SC_ONE_OR_MORE_BOUND)
        {
        }

        protected sc_port(string name_, int max_size_, sc_port_policy policy)
            : base(name_, max_size_, policy)
        {
        }

        // destructor (does nothing)

        public override void Dispose()
        {
        }

        //C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
        //ORIGINAL LINE: virtual string kind() const
        public override string kind()
        {
            return "sc_port";
        }


    }


    public class sc_port_b<IF> : sc_port_base where IF : class, sc_interface
    {
        IF m_interface;	// first interface in interface vec
        List<IF> m_interface_vec;

        public void bind(IF interface_)
        {
            base.bind(interface_);
        }



        // number of connected interfaces
        public int size()
        {
            return m_interface_vec.Count;
        }




        // allow to call methods provided by interface at index
        public IF get_interface(int index_)
        {
            if (index_ == 0)
            {
                return m_interface;
            }
            else if (index_ < 0 || index_ >= size())
            {
                report_error("get interface failed", "index out of range");
            }
            return m_interface_vec[index_];
        }


        public IF this[int index_]
        {
            get
            {
                return get_interface(index_);
            }
        }



        // get the first interface without checking for nil

        public override sc_interface get_interface()
        {
            return m_interface;
        }



        // constructors

        protected sc_port_b(int max_size_)
            : this(max_size_, sc_port_policy.SC_ONE_OR_MORE_BOUND)
        {
        }

        protected sc_port_b(int max_size_, sc_port_policy policy)
            : base(max_size_, policy)
        {
            m_interface = default(IF);
            m_interface_vec = new List<IF>();
        }

        protected sc_port_b(string name_, int max_size_)
            : this(name_, max_size_, sc_port_policy.SC_ONE_OR_MORE_BOUND)
        {
        }

        protected sc_port_b(string name_, int max_size_, sc_port_policy policy)
            : base(name_, max_size_, policy)
        {
            m_interface = default(IF);
            m_interface_vec = new List<IF>();
        }


        // destructor (does nothing)

        public override void Dispose()
        {
            base.Dispose();
        }


        // called by pbind (for internal use only)
        public override int vbind(sc_interface interface_)
        {
            IF iface = interface_ as IF;
            if (iface == null)
            {
                // type mismatch
                return 2;
            }
            base.bind(iface);
            return 0;
        }

		public override int vbind(sc_port_base parent_)
        {
            sc_port_base parent = parent_ as sc_port_base;
            if (parent == null)
            {
                // type mismatch
                return 2;
            }
            base.bind(parent);
            return 0;
        }


        // called by the sc_sensitive* classes
		protected new void make_sensitive(sc_thread_process handle_p)
        {
            make_sensitive(handle_p, null);
        }

		protected new void make_sensitive(sc_thread_process handle_p, sc_event_finder event_finder_)
        {
            if (m_bind_info == null)
            {
                int if_n = m_interface_vec.Count;
                for (int if_i = 0; if_i < if_n; if_i++)
                {
                    IF iface_p = m_interface_vec[if_i];
                    Debug.Assert(iface_p != null);
                    add_static_event(handle_p, iface_p.default_event());
                }
            }
            else
            {
                base.make_sensitive(handle_p, event_finder_);
            }
        }

		protected new void make_sensitive(sc_method_process handle_p)
        {
            make_sensitive(handle_p, null);
        }

		protected new void make_sensitive(sc_method_process handle_p, sc_event_finder event_finder_)
        {
            if (m_bind_info == null)
            {
                int if_n = m_interface_vec.Count;
                for (int if_i = 0; if_i < if_n; if_i++)
                {
                    IF iface_p = m_interface_vec[if_i];
                    Debug.Assert(iface_p != null);
                    add_static_event(handle_p, iface_p.default_event());
                }
            }
            else
            {
                base.make_sensitive(handle_p, event_finder_);
            }
        }


        // called by complete_binding (for internal use only)
        public override void add_interface(sc_interface interface_)
        {
            IF iface = interface_ as IF;
            Debug.Assert(iface != null);

            // make sure that the interface is not already bound:

            int size = m_interface_vec.Count;
            for (int i = 0; i < size; i++)
            {
                if (iface == m_interface_vec[i])
                {
                    report_error("bind interface to port failed", "interface already bound to port");
                }
            }

            // "bind" the interface and make sure our short cut for 0 is set up.

            m_interface_vec.Add(iface);
            m_interface = m_interface_vec[0];
        }

        public override string if_typename()
        {
			return typeof(IF).Name;
        }
        public override int interface_count()
        {
            return m_interface_vec.Count;
        }

        // disabled

        public override void add_static_event(sc_method_process process_p, sc_event Event)
        {
            throw new System.NotImplementedException();
        }

        public override void add_static_event(sc_thread_process process_p, sc_event Event)
        {
            throw new System.NotImplementedException();
        }
    }
}