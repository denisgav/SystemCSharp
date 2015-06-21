using System.Diagnostics;

namespace sc_core
{
	public enum sc_sensitive_mode
	{
		SC_NONE_,
		SC_METHOD_,
		SC_THREAD_
	}

	public class sc_sensitive
	{
		internal static void warn_no_parens ()
		{
			sc_report_handler.report (sc_core.sc_severity.SC_INFO, "/IEEE_Std_1666/deprecated", "use of () to specify sensitivity is deprecated, use << instead");
		}

		internal static void sc_deprecated_sensitive_pos()
		{
			sc_report_handler.report(sc_core.sc_severity.SC_INFO, "/IEEE_Std_1666/deprecated", "sc_sensitive_pos is deprecated use sc_sensitive << with pos() instead");

		}

		internal static void sc_deprecated_sensitive_neg()
		{
			sc_report_handler.report(sc_core.sc_severity.SC_INFO, "/IEEE_Std_1666/deprecated", "sc_sensitive_neg is deprecated use sc_sensitive << with neg() instead");
		}

		public static sc_method_process as_method_handle( sc_process_b handle_ )
		{
			return  handle_ as sc_method_process;
		}

		public static sc_thread_process as_thread_handle( sc_process_b handle_ )
		{
			return handle_ as sc_thread_process;
		}


		

		protected sc_module m_module;
		protected sc_sensitive_mode m_mode = sc_sensitive_mode.SC_NONE_;
		protected sc_process_b m_handle;


		public sc_sensitive (sc_module module_)
		{
			m_module = module_;
			m_mode = sc_sensitive_mode.SC_NONE_;
			m_handle = null;
		}

		public virtual void Dispose ()
		{
		}

		public virtual void reset ()
		{
			m_mode = sc_sensitive_mode.SC_NONE_;
		}

		public virtual void make_static_sensitivity (sc_process_b handle_, sc_event event_)
		{
			handle_.add_static_event (event_);
		}

		public virtual void make_static_sensitivity (sc_process_b handle_, sc_interface interface_)
		{
			handle_.add_static_event (interface_.default_event ());
		}

		public virtual void make_static_sensitivity (sc_process_b handle_, sc_port_base port_)
		{
			sc_method_process handle_m = as_method_handle (handle_);
			if (handle_m != null) {
				port_.make_sensitive (handle_m);
				return;
			}
			sc_thread_process handle_t = as_thread_handle (handle_);
			port_.make_sensitive (handle_t);
		}

		public virtual void make_static_sensitivity (sc_process_b handle_, sc_event_finder event_finder_)
		{
			if (sc_simcontext.sc_is_running ()) {
				handle_.add_static_event (event_finder_.find_event ());
			} else {
				sc_method_process handle_m = as_method_handle (handle_);
				if (handle_m != null) {
					event_finder_.port ().make_sensitive (handle_m, event_finder_);
					return;
				}
				sc_thread_process handle_t = as_thread_handle (handle_);
				event_finder_.port ().make_sensitive (handle_t, event_finder_);
			}
		}
		// changing between process handles
		public virtual sc_sensitive add (sc_process_handle handle_)
		{
			switch (handle_.proc_kind ()) {
			case sc_curr_proc_kind.SC_CTHREAD_PROC_:
			case sc_curr_proc_kind.SC_THREAD_PROC_:
				m_mode = sc_sensitive_mode.SC_THREAD_;
				break;
			case sc_curr_proc_kind.SC_METHOD_PROC_:
				m_mode = sc_sensitive_mode.SC_METHOD_;
				break;
			default:
				Debug.Assert (false);
				break;
			}
			m_handle = handle_.m_target_p;

			return this;

		}

		public virtual sc_sensitive add (sc_event event_)
		{
			// check
			if (sc_simcontext.sc_is_running ()) {
				sc_report_handler.report (sc_core.sc_severity.SC_ERROR, "make sensitive failed", "simulation running");
			}

			// make sensitive
			switch (m_mode) {
			case sc_sensitive_mode.SC_METHOD_:
			case sc_sensitive_mode.SC_THREAD_:
				{
					m_handle.add_static_event (event_);
					break;
				}
			case sc_sensitive_mode.SC_NONE_:
				// do nothing 
				break;
			}

			return this;
		}

		public virtual sc_sensitive add(sc_interface interface_)
		{
			// check
			if(sc_simcontext.sc_is_running())
			{
				sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "make sensitive failed", "simulation running");
			}

			// make sensitive
			switch(m_mode)
			{
			case sc_sensitive_mode.SC_METHOD_:
			case sc_sensitive_mode.SC_THREAD_:
			{
				m_handle.add_static_event(interface_.default_event());
				break;
			}
			case sc_sensitive_mode.SC_NONE_:
				// do nothing 
				break;
			}

			return this;
		}

		public virtual sc_sensitive add(sc_port_base port_)
		{
			// check
			if(sc_simcontext.sc_is_running())
			{
				sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "make sensitive failed", "simulation running");
			}

			// make sensitive
			switch(m_mode)
			{
			case sc_sensitive_mode.SC_METHOD_:
			{
				port_.make_sensitive(as_method_handle(m_handle));
				break;
			}
			case sc_sensitive_mode.SC_THREAD_:
			{
				port_.make_sensitive(as_thread_handle(m_handle));
				break;
			}
			case sc_sensitive_mode.SC_NONE_:
				// do nothing 
				break;
			}

			return this;
		}

		public virtual sc_sensitive add(sc_event_finder event_finder_)
		{
			// check
			if(sc_simcontext.sc_is_running())
			{
				sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "make sensitive failed", "simulation running");
			}

			// make sensitive
			switch(m_mode)
			{
			case sc_sensitive_mode.SC_METHOD_:
			{
				event_finder_.port().make_sensitive(as_method_handle(m_handle), event_finder_);
				break;
			}
			case sc_sensitive_mode.SC_THREAD_:
			{
				event_finder_.port().make_sensitive(as_thread_handle(m_handle), event_finder_);
				break;
			}
			case sc_sensitive_mode.SC_NONE_:
				// do nothing 
				break;
			}

			return this;
		}

	}

	public class sc_sensitive_pos : sc_sensitive
	{
		public sc_sensitive_pos (sc_module module_)
			: base(module_)
		{
		}

		public override void make_static_sensitivity (sc_process_b handle_, sc_port_base port_)
		{
			sc_method_process handle_m = as_method_handle (handle_);
			if (handle_m != null) {
				port_.make_sensitive (handle_m);
				return;
			}
			sc_thread_process handle_t = as_thread_handle (handle_);
			port_.make_sensitive (handle_t);
		}



		public override sc_sensitive add(sc_port_base port_)
		{
			// check
			if(sc_simcontext.sc_is_running())
			{
				sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "make sensitive failed", "simulation running");
			}

			// make sensitive
			switch(m_mode)
			{
				case sc_sensitive_mode.SC_METHOD_:
			{
				port_.make_sensitive(as_method_handle(m_handle), port_.pos());
				break;
			}
				case sc_sensitive_mode.SC_THREAD_:
			{
				port_.make_sensitive(as_thread_handle(m_handle));
				break;
			}
				case sc_sensitive_mode.SC_NONE_:
				// do nothing 
				break;
			}

			return this;
		}

	}

	public class sc_sensitive_neg : sc_sensitive
	{
		public sc_sensitive_neg (sc_module module_)
			: base(module_)
		{
		}

		public override void make_static_sensitivity (sc_process_b handle_, sc_port_base port_)
		{
			sc_method_process handle_m = as_method_handle (handle_);
			if (handle_m != null) {
				port_.make_sensitive (handle_m);
				return;
			}
			sc_thread_process handle_t = as_thread_handle (handle_);
			port_.make_sensitive (handle_t);
		}



		public override sc_sensitive add(sc_port_base port_)
		{
			// check
			if(sc_simcontext.sc_is_running())
			{
				sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "make sensitive failed", "simulation running");
			}

			// make sensitive
			switch(m_mode)
			{
				case sc_sensitive_mode.SC_METHOD_:
			{
				port_.make_sensitive(as_method_handle(m_handle), port_.neg());
				break;
			}
				case sc_sensitive_mode.SC_THREAD_:
			{
				port_.make_sensitive(as_thread_handle(m_handle));
				break;
			}
				case sc_sensitive_mode.SC_NONE_:
				// do nothing 
				break;
			}

			return this;
		}

	}
}