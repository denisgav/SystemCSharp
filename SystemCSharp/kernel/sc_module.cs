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


using System.Collections.Generic;

namespace sc_core
{
    
    public class sc_bind_proxy
    {
        public sc_interface iface;
        public sc_port_base port;

        public sc_bind_proxy()
        {
            iface = null;
            port = null;
        }
        public sc_bind_proxy(sc_interface iface_)
        {
            iface = iface_;
            port = null;
        }
        public sc_bind_proxy(sc_port_base port_)
        {
            iface = null;
            port = port_;
        }
    }
    

    // ----------------------------------------------------------------------------
    //  CLASS : sc_module_dynalloc_list
    //
    //  Garbage collection for modules dynamically allocated with SC_NEW.
    // ----------------------------------------------------------------------------

    public class sc_module_dynalloc_list
    {

        public sc_module_dynalloc_list()
        {
            m_list = new List<sc_module>();
        }


        //------------------------------------------------------------------------------
        //"~sc_module_dynalloc_list"
        //
        // Note we clear the m_parent field for the module being deleted. This because
        // we process the list front to back so the parent has already been deleted, 
        // and we don't want ~sc_object() to try to access the parent which may 
        // contain garbage.
        //------------------------------------------------------------------------------
        public void Dispose()
        {
            m_list.Clear();
        }

        public void add(sc_module p)
        {
            m_list.Add(p);
        }


        private List<sc_module> m_list = new List<sc_module>();
    }




    // ----------------------------------------------------------------------------
    //  CLASS : sc_module
    //
    //  Base class for all structural entities.
    // ----------------------------------------------------------------------------

    public class sc_module : sc_object, sc_process_host
    {
        private static sc_module_dynalloc_list m_sc_module_dynalloc_list;
        public static sc_module sc_module_dynalloc(sc_module module_)
        {
            if (m_sc_module_dynalloc_list == null)
                m_sc_module_dynalloc_list = new sc_module_dynalloc_list();
            m_sc_module_dynalloc_list.add(module_);
            return module_;
        }

        public static readonly sc_bind_proxy SC_BIND_PROXY_NIL = new sc_bind_proxy();

        // to generate unique names for objects in an MT-Safe way

        public string gen_unique_name(string basename_, bool preserve_first)
        {
            return m_name_gen.gen_unique_name(basename_, preserve_first);
        }


        public override string kind()
        {
            return "sc_module";
        }


        public virtual void before_end_of_elaboration()
        {
        }


        // We push the sc_module instance onto the stack of open objects so 
        // that any objects that are created in before_end_of_elaboration have
        // the proper parent. After the call we pop the hierarchy.
        public virtual void construction_done()
        {
            hierarchy_scope scope = new hierarchy_scope(this);
            before_end_of_elaboration();
        }

        // called by elaboration_done (does nothing by default)

        // called by elaboration_done (does nothing by default)

        public virtual void end_of_elaboration()
        {
        }


        // We push the sc_module instance onto the stack of open objects so 
        // that any objects that are created in end_of_elaboration have
        // the proper parent. After the call we pop the hierarchy.
        public void elaboration_done(ref bool error_)
        {
            if (!m_end_module_called)
            {
                string msg = string.Format("module {0}", name());
                sc_report_handler.report(sc_core.sc_severity.SC_WARNING, "module construction not properly completed: did you forget to add a sc_module_name parameter to your module constructor?", msg);
                if (error_)
                {
                    sc_report_handler.report(sc_core.sc_severity.SC_WARNING, "hierarchical name as shown may be incorrect due to previous errors", "");
                }
                error_ = true;
            }
            hierarchy_scope scope = new hierarchy_scope(this);
            end_of_elaboration();
        }

        // called by start_simulation (does nothing by default)

        public virtual void start_of_simulation()
        {
        }

        public virtual void start_simulation()
        {
            hierarchy_scope scope = new hierarchy_scope(this);
            start_of_simulation();
        }

        // called by simulation_done (does nothing by default)

        public virtual void end_of_simulation()
        {
        }

        public virtual void simulation_done()
        {
            hierarchy_scope scope = new hierarchy_scope(this);
            end_of_simulation();
        }


        // ----------------------------------------------------------------------------
        //  CLASS : sc_module
        //
        //  Base class for all structural entities.
        // ----------------------------------------------------------------------------

        protected void sc_module_init()
        {
            simcontext().get_module_registry().insert(this);
            simcontext().hierarchy_push(this);
            m_end_module_called = false;
            m_module_name_p = null;
            m_port_vec = new List<sc_port_base>();
            m_port_index = 0;
            
            m_name_gen = new sc_name_gen();
        }

        // constructor

        //
        // *  This form of the constructor assumes that the user has
        // *  used an sc_module_name parameter for his/her constructor.
        // *  That parameter object has been pushed onto the stack,
        // *  and can be looked up by calling the 
        // *  top_of_module_name_stack() function of the object manager.
        // *  This technique has two advantages:
        // *
        // *  1) The user no longer has to write sc_module(name) in the
        // *     constructor initializer.
        // *  2) The user no longer has to call end_module() at the end
        // *     of the constructor -- a common negligence.
        // *
        // *  But it is important to note that sc_module_name may be used
        // *  in the user's constructor's parameter. If it is used anywhere
        // *  else, unexpected things will happen. The error-checking
        // *  mechanism builtin here cannot hope to catch all misuses.
        // *
        //


        public sc_module()
            : this(sc_simcontext.sc_get_curr_simcontext().get_object_manager().top_of_module_name_stack())
        {
            //--!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //--!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //--!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //--!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //--!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //--!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            //sensitive(this), sensitive_pos(this), sensitive_neg(this), m_end_module_called(false), m_port_vec(), m_port_index(0), m_name_gen(0), m_module_name_p(0)

            //--!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //--!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //--!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //--!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //--!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //--!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            
            sc_module_name mod_name = simcontext().get_object_manager().top_of_module_name_stack();
            if (null == mod_name || null != mod_name.name())
                sc_core.sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "an sc_module_name parameter for your constructor is required", "");
            sc_module_init();
            mod_name.set_module(this);
            m_module_name_p = mod_name; // must come after sc_module_init call.
        }

        public void defunct()
        {
            throw new System.NotImplementedException();
        }

        public sc_module(sc_module_name nm)
            : base(nm == null? "" : nm.ToString())
        {
            //--!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //--!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //--!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //--!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //--!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //--!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            //, sensitive(this), sensitive_pos(this), sensitive_neg(this), m_end_module_called(false), m_port_vec(), m_port_index(0), m_name_gen(0), m_module_name_p(0)

            //--!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //--!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //--!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //--!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //--!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //--!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            //     For those used to the old style of passing a name to sc_module,
            //       this constructor will reduce the chance of making a mistake

            //     When this form is used, we better have a fresh sc_module_name
            //       on the top of the stack
            sc_module_name mod_name = nm;
            if (null == mod_name || null == mod_name.name())
            {
                sc_report_handler.report(sc_severity.SC_ERROR, "an sc_module_name parameter for your constructor is required", "");
                mod_name = new sc_module_name(sc_simcontext.sc_gen_unique_name(""));
            }
            sc_module_init();
            mod_name.set_module(this);
            m_module_name_p = mod_name; // must come after sc_module_init call.
        }



        // destructor

        // -------------------------------------------------------------------- 

        public override void Dispose()
        {
            m_port_vec = null;
            orphan_child_objects();
            if (m_module_name_p != null)
            {
                m_module_name_p.clear_module(this); // must be before end_module()
                end_module();
            }
            simcontext().get_module_registry().remove(this);
            base.Dispose();
        }


        // this must be called by user-defined modules
        public virtual void end_module()
        {
            if (!m_end_module_called)
            {
                //	 TBD: Can check here to alert the user that end_module
                //                was not called for a previous module.
                sc_simcontext.sc_get_curr_simcontext().hierarchy_pop();
                sc_simcontext.sc_get_curr_simcontext().reset_curr_proc();
                /*
                sensitive.reset();
                sensitive_pos.reset();
                sensitive_neg.reset();
                */
                m_end_module_called = true;
                m_module_name_p = null; // make sure we are not called in ~sc_module().
            }
        }


        // to prevent initialization for SC_METHODs and SC_THREADs

        protected void dont_initialize()
        {
            sc_process_handle last_proc = sc_process_handle.sc_get_last_created_process_handle();
            last_proc.dont_initialize(true);
        }

        // positional binding code - used by operator ()
        //---------------------------------------------------------------------------------
        protected void positional_bind(sc_interface interface_)
        {
            if (m_port_index == (int)m_port_vec.Count)
            {
                string msg = (m_port_index == 0) ? string.Format("module {0} has no ports", name()) : string.Format("all ports of module {0} are bound", name());
                sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "bind interface to port failed", msg);
            }
            int status = (m_port_vec)[m_port_index].pbind(interface_);
            if (status != 0)
            {
                string msg = string.Empty;
                switch (status)
                {
                    case 1:
                        msg = string.Format("port {0} of module {1} is already bound", m_port_index, name());
                        break;
                    case 2:
                        msg = string.Format("type mismatch on port {0} of module {1}", m_port_index, name());
                        break;
                    default:
                        msg = string.Format("unknown error");
                        break;
                }

                sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "bind interface to port failed", msg);
            }
            ++m_port_index;
        }

        protected void positional_bind(sc_port_base port_)
        {
            if (m_port_index == (int)m_port_vec.Count)
            {
                string msg = (m_port_index == 0) ? string.Format("module {0} has no ports", name()) : string.Format("all ports of module {0} are bound", name());
                sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "bind interface to port failed", msg);
            }
            int status = (m_port_vec)[m_port_index].pbind(port_);
            if (status != 0)
            {
                string msg = string.Empty;
                switch (status)
                {
                    case 1:
                        msg = string.Format("port {0} of module {1} is already bound", m_port_index, name());
                        break;
                    case 2:
                        msg = string.Format("type mismatch on port {0} of module {1}", m_port_index, name());
                        break;
                    default:
                        msg = string.Format("unknown error");
                        break;
                }
                sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "bind interface to port failed", msg);
            }
            ++m_port_index;
        }

        // set reset sensitivity for SC_xTHREADs

        // set SC_THREAD asynchronous reset sensitivity
        /*
        protected void async_reset_signal_is(sc_in<bool> port, bool level)
        {
            sc_reset.reset_signal_is(true, new sc_in(port), level);
        }
        protected void async_reset_signal_is(sc_inout<bool> port, bool level)
        {
            sc_reset.reset_signal_is(true, new sc_inout(port), level);
        }
        protected void async_reset_signal_is(sc_out<bool> port, bool level)
        {
            sc_reset.reset_signal_is(true, new sc_out(port), level);
        }
        protected void async_reset_signal_is(sc_signal_in_if<bool> iface, bool level)
        {
            sc_reset.reset_signal_is(true, new sc_signal_in_if(iface), level);
        }

        // set SC_THREAD synchronous reset sensitivity

        protected void reset_signal_is(sc_in<bool> port, bool level)
        {
            sc_reset.reset_signal_is(false, new sc_in(port), level);
        }
        protected void reset_signal_is(sc_inout<bool> port, bool level)
        {
            sc_reset.reset_signal_is(false, new sc_inout(port), level);
        }
        protected void reset_signal_is(sc_out<bool> port, bool level)
        {
            sc_reset.reset_signal_is(false, new sc_out(port), level);
        }
        protected void reset_signal_is(sc_signal_in_if<bool> iface, bool level)
        {
            sc_reset.reset_signal_is(false, new sc_signal_in_if(iface), level);
        }
        */
        // static sensitivity for SC_THREADs and SC_CTHREADs

        protected void wait()
        {
            sc_wait.wait(simcontext());
        }

        // dynamic sensitivity for SC_THREADs and SC_CTHREADs

        protected void wait(sc_event e)
        {
            sc_wait.wait(e, simcontext());
        }

        protected void wait(sc_event_or_list el)
        {
            sc_wait.wait(el, simcontext());
        }

        protected void wait(sc_event_and_list el)
        {
            sc_wait.wait(el, simcontext());
        }

        protected void wait(sc_time t)
        {
            sc_wait.wait(t, simcontext());
        }

        protected void wait(double v, sc_time_unit tu)
        {
            sc_wait.wait(new sc_time(v, tu, simcontext()), simcontext());
        }

        protected void wait(sc_time t, sc_event e)
        {
            sc_wait.wait(new sc_time(t), e, simcontext());
        }

        protected void wait(double v, sc_time_unit tu, sc_event e)
        {
            sc_wait.wait(new sc_time(v, tu, simcontext()), e, simcontext());
        }

        protected void wait(sc_time t, sc_event_or_list el)
		{
			sc_wait.wait(t, el, simcontext());
		}

        protected void wait(double v, sc_time_unit tu, sc_event_or_list el)
        {
            sc_wait.wait(new sc_time(v, tu, simcontext()), el, simcontext());
        }

        protected void wait(sc_time t, sc_event_and_list el)
        {
            sc_wait.wait(t, el, simcontext());
        }

        protected void wait(double v, sc_time_unit tu, sc_event_and_list el)
        {
            sc_wait.wait(new sc_time(v, tu, simcontext()), el, simcontext());
        }


        // static sensitivity for SC_METHODs

        protected void next_trigger()
        {
            sc_wait.next_trigger(simcontext());
        }


        // dynamic sensitivty for SC_METHODs

        protected void next_trigger(sc_event e)
        {
            sc_wait.next_trigger(e, simcontext());
        }

        protected void next_trigger(sc_event_or_list el)
        {
            sc_wait.next_trigger(el, simcontext());
        }

        protected void next_trigger(sc_event_and_list el)
        {
            sc_wait.next_trigger(el, simcontext());
        }

        protected void next_trigger(sc_time t)
        {
            sc_wait.next_trigger(t, simcontext());
        }

        protected void next_trigger(double v, sc_time_unit tu)
        {
            sc_wait.next_trigger(new sc_time(v, tu, simcontext()), simcontext());
        }

        protected void next_trigger(sc_time t, sc_event e)
        {
            sc_wait.next_trigger(t, e, simcontext());
        }

        protected void next_trigger(double v, sc_time_unit tu, sc_event e)
        {
            sc_wait.next_trigger(new sc_time(v, tu, simcontext()), e, simcontext());
        }

        protected void next_trigger(sc_time t, sc_event_or_list el)
        {
            sc_wait.next_trigger(t, el, simcontext());
        }

        protected void next_trigger(double v, sc_time_unit tu, sc_event_or_list el)
        {
            sc_wait.next_trigger(new sc_time(v, tu, simcontext()), el, simcontext());
        }

        protected void next_trigger(sc_time t, sc_event_and_list el)
        {
            sc_wait.next_trigger(t, el, simcontext());
        }

        protected void next_trigger(double v, sc_time_unit tu, sc_event_and_list el)
        {
            sc_wait.next_trigger(new sc_time(v, tu, simcontext()), el, simcontext());
        }


        // for SC_METHODs and SC_THREADs and SC_CTHREADs

        protected bool timed_out()
        {
            return sc_wait.timed_out();
        }


        // for SC_CTHREADs

        protected void halt()
        {
            sc_cthread_process.halt(simcontext());
        }

        /*
        protected void wait(int n)
        {
            sc_wait.wait(n, simcontext());
        }
        
        protected void at_posedge(sc_signal_in_if<bool> s)
        {
            sc_wait.at_posedge(s, simcontext());
        }

        protected void at_posedge(sc_signal_in_if<sc_dt.sc_logic> s)
        {
            global::sc_core.at_posedge(new sc_signal_in_if(s), simcontext());
        }

        protected void at_negedge(sc_signal_in_if<bool> s)
        {
    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
    //ORIGINAL LINE: TangibleTempGlobal.sc_core::at_negedge(s, simcontext());
            global::sc_core.at_negedge(new sc_signal_in_if(s), simcontext());
        }

        protected void at_negedge(sc_signal_in_if<sc_dt.sc_logic> s)
        {
    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
    //ORIGINAL LINE: TangibleTempGlobal.sc_core::at_negedge(s, simcontext());
            global::sc_core.at_negedge(new sc_signal_in_if(s), simcontext());
        }

        // Catch uses of watching:
        protected void watching(bool UnnamedParameter1) // expr
            {
    //C++ TO C# CONVERTER TODO TASK: There is no direct equivalent in C# to the C++ __LINE__ macro:
    //C++ TO C# CONVERTER TODO TASK: There is no direct equivalent in C# to the C++ __FILE__ macro:
                global::sc_core.sc_report_handler.report(sc_core.sc_severity.SC_ERROR, SC_ID_WATCHING_NOT_ALLOWED_, "", __FILE__, __LINE__);
            }
        */
        // These are protected so that user derived classes can refer to them.

        /*
        protected sc_sensitive sensitive = new sc_sensitive();
        protected sc_sensitive_pos sensitive_pos = new sc_sensitive_pos();
        protected sc_sensitive_neg sensitive_neg = new sc_sensitive_neg();
        */
        // Function to set the stack size of the current (c)thread process.
        protected void set_stack_size(uint size)
        {
            sc_process_handle proc_h = new sc_process_handle(sc_simcontext.sc_is_running() ? sc_simcontext.sc_get_current_process_handle() : sc_process_handle.sc_get_last_created_process_handle());
            sc_thread_process thread_h; // Current process as thread.


            thread_h = (sc_thread_process)proc_h.get_process_object();
            if (thread_h != null)
            {
                thread_h.set_stack_size(size);
            }
            else
            {
                sc_report_handler.report(sc_core.sc_severity.SC_WARNING, "set_stack_size() is only allowed for SC_THREADs and SC_CTHREADs", "");
            }
        }

        
        public int append_port(sc_port_base port_)
        {
            int index = m_port_vec.Count;
            m_port_vec.Add(port_);
            return index;
        }


        private bool m_end_module_called;
        private List<sc_port_base> m_port_vec;
        private int m_port_index;
        private sc_name_gen m_name_gen;
        private sc_module_name m_module_name_p;

    }


}
