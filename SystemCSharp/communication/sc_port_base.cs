//****************************************************************************
//
//  The following code is derived, directly or indirectly, from the SystemC
//  source code Copyright (c) 1996-2014 by all Contributors.
//  All Rights reserved.
//
//  The contents of this file are subject to the restrictions and limitations
//  set forth in the SystemC Open Source License (the "License");
//  You may not use this file except in compliance with such restrictions and
//  limitations. You may obtain instructions on how to receive a copy of the
//  License at http://www.accellera.org/. Software distributed by Contributors
//  under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF
//  ANY KIND, either express or implied. See the License for the specific
//  language governing rights and limitations under the License.
//
// ****************************************************************************


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace sc_core
{
    public enum sc_port_policy
    {
        SC_ONE_OR_MORE_BOUND, // Default
        SC_ZERO_OR_MORE_BOUND,
        SC_ALL_BOUND
    }

    // ----------------------------------------------------------------------------
    //  STRUCT : sc_bind_info
    // ----------------------------------------------------------------------------

    public class sc_bind_info
    {

        public sc_bind_info(int max_size_)
            : this(max_size_, sc_port_policy.SC_ONE_OR_MORE_BOUND)
        {
        }

        public sc_bind_info(int max_size_, sc_port_policy policy_)
        {
            m_max_size = max_size_;
            m_policy = policy_;
            vec = new List<sc_bind_elem>();
            has_parent = false;
            last_add = -1;
            is_leaf = true;
            complete = false;
            thread_vec = new List<sc_bind_ef>();
            method_vec = new List<sc_bind_ef>();
        }

        // destructor

        // destructor

        public void Dispose()
        {
            for (int i = size() - 1; i >= 0; --i)
            {
                //if (vec[i] != null)
                //    vec[i].Dispose();
            }
        }

        public int max_size()
        {
            return m_max_size != 0 ? m_max_size : (int)vec.Count;
        }

        public sc_port_policy policy()
        {
            return m_policy;
        }

        public int size()
        {
            return vec.Count;
        }

        public int m_max_size;
        public sc_port_policy m_policy;
        public List<sc_bind_elem> vec = new List<sc_bind_elem>();
        public bool has_parent;
        public int last_add;
        public bool is_leaf;
        public bool complete;

        public List<sc_bind_ef> thread_vec = new List<sc_bind_ef>();
        public List<sc_bind_ef> method_vec = new List<sc_bind_ef>();
    }

    // ----------------------------------------------------------------------------
    //  STRUCT : sc_bind_ef
    // ----------------------------------------------------------------------------

    public class sc_bind_ef
    {
        // constructor

        public sc_bind_ef(sc_process_b handle_, sc_event_finder event_finder_)
        {
            handle = handle_;
            event_finder = event_finder_;
        }

        public void Dispose()
        {
        }

        public sc_process_b handle;
        public sc_event_finder event_finder;
    }

    // ----------------------------------------------------------------------------
    //  STRUCT : sc_bind_elem
    // ----------------------------------------------------------------------------

    public partial class sc_bind_elem
    {
        public sc_interface iface;
        public sc_port_base parent;

        public sc_bind_elem()
        {
            iface = null;
            parent = null;
        }
        public sc_bind_elem(sc_interface interface_)
        {
            iface = interface_;
            parent = null;
        }
        public sc_bind_elem(sc_port_base parent_)
        {
            iface = null;
            parent = parent_;
        }
    }

    public class sc_port_registry
    {

        // ----------------------------------------------------------------------------
        //  CLASS : sc_port_registry
        //
        //  Registry for all ports.
        //  FOR INTERNAL USE ONLY!
        // ----------------------------------------------------------------------------

        public void insert(sc_port_base port_)
        {
            if (sc_simcontext.sc_is_running())
            {
                port_.report_error("insert port failed", "simulation running");
            }

            if (m_simc.elaboration_done())
            {
                port_.report_error("insert port failed", "elaboration done");
            }


            // check if port_ is already inserted
            for (int i = size() - 1; i >= 0; --i)
            {
                if (port_ == m_port_vec[i])
                {
                    port_.report_error("insert port failed", "port already inserted");
                }
            }


            // append the port to the current module's vector of ports
            sc_module curr_module = m_simc.hierarchy_curr();
            if (curr_module == null)
            {
                port_.report_error("port specified outside of module");
            }
            curr_module.append_port(port_);

            // insert
            m_port_vec.Add(port_);
        }
        public void remove(sc_port_base port_)
        {
            int i;
            for (i = size() - 1; i >= 0; --i)
            {
                if (port_ == m_port_vec[i])
                {
                    break;
                }
            }
            if (i == -1)
            {
                port_.report_error("remove port failed", "port not registered");
            }

            // remove
            m_port_vec.RemoveAt(i);
        }

        public int size()
        {
            return m_port_vec.Count;
        }


        // constructor

        // constructor

        public sc_port_registry(sc_simcontext simc_)
        {
            m_construction_done = 0;
            m_port_vec = new List<sc_port_base>();
            m_simc = simc_;
        }

        // destructor

        public void Dispose()
        {
        }

        // called when by construction_done and elaboration done

		public void complete_binding()
        {
            for (int i = size() - 1; i >= 0; --i)
            {
                m_port_vec[i].complete_binding();
            }
        }

        // called when construction is done

		public bool construction_done()
        {
            if (size() == m_construction_done)
                // nothing has been updated
                return true;

            for (int i = size() - 1; i >= m_construction_done; --i)
            {
                m_port_vec[i].construction_done();
            }

            m_construction_done = size();
            return false;
        }

        // called when elaboration is done

		public void elaboration_done()
        {
            complete_binding();

            for (int i = size() - 1; i >= 0; --i)
            {
                m_port_vec[i].elaboration_done();
            }
        }

        // called before simulation begins

		public void start_simulation()
        {
            for (int i = size() - 1; i >= 0; --i)
            {
                m_port_vec[i].start_simulation();
            }
        }

        // called after simulation ends

		public void simulation_done()
        {
            for (int i = size() - 1; i >= 0; --i)
            {
                m_port_vec[i].simulation_done();
            }
        }


        // This is a static member function.

		public static void replace_port(sc_port_registry UnnamedParameter1)
        {
        }


        private int m_construction_done;
        private List<sc_port_base> m_port_vec = new List<sc_port_base>();
        private sc_simcontext m_simc;
    }


    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    //  BEWARE: Ports can only be created and bound during elaboration.
    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!


    // ----------------------------------------------------------------------------
    //  CLASS : sc_port_base
    //
    //  Abstract base class for class sc_port_b.
    // ----------------------------------------------------------------------------

    public abstract class sc_port_base : sc_object
    {

        // return number of interfaces that will be bound, or are bound:

        public int bind_count()
        {
            if (m_bind_info != null)
                return m_bind_info.size();
            else
                return interface_count();
        }

        // get the first interface without checking for nil
        public abstract sc_interface get_interface();

        public override string kind()
        {
            return "sc_port_base";
        }


        // constructors

        // constructors

        protected sc_port_base(int max_size_)
            : this(max_size_, sc_port_policy.SC_ONE_OR_MORE_BOUND)
        {
        }

        protected sc_port_base(int max_size_, sc_port_policy policy)
            : base(sc_simcontext.sc_gen_unique_name("port"))
        {
            m_bind_info = null;
            simcontext().get_port_registry().insert(this);
            m_bind_info = new sc_bind_info(max_size_, policy);
        }
        protected sc_port_base(string name_, int max_size_)
            : this(name_, max_size_, sc_port_policy.SC_ONE_OR_MORE_BOUND)
        {
        }

        protected sc_port_base(string name_, int max_size_, sc_port_policy policy)
            : base(name_)
        {
            m_bind_info = null;
            simcontext().get_port_registry().insert(this);
            m_bind_info = new sc_bind_info(max_size_, policy);
        }

        // destructor

        public override void Dispose()
        {
            simcontext().get_port_registry().remove(this);
            if (m_bind_info != null)
                m_bind_info.Dispose();
            base.Dispose();
        }

        // bind interface to this port

        protected void bind(sc_interface interface_)
        {
            if (m_bind_info == null)
            {
                // cannot bind an interface after elaboration
                report_error("bind interface to port failed", "simulation running");
            }

            m_bind_info.vec.Add(new sc_bind_elem(interface_));

            if (!m_bind_info.has_parent)
            {
                // add (cache) the interface
                add_interface(interface_);
                m_bind_info.last_add++;
            }
        }

        // bind parent port to this port
        protected void bind(sc_port_base parent_)
        {
            if (m_bind_info == null)
            {
                // cannot bind a parent port after elaboration
                report_error("bind parent port to port failed", "simulation running");
            }

            if (parent_ == this)
            {
                report_error("bind parent port to port failed", "same port");
            }


            m_bind_info.vec.Add(new sc_bind_elem(parent_));
            m_bind_info.has_parent = true;
            parent_.m_bind_info.is_leaf = false;
        }

        // called by pbind (for internal use only)
		public abstract int vbind(sc_interface interface_);
		public abstract int vbind(sc_port_base interface_);

        // called by complete_binding (for internal use only)
        public abstract void add_interface(sc_interface NamelessParameter);
        public abstract int interface_count();
        public abstract string if_typename();

        // called by construction_done (does nothing by default)
        public virtual void before_end_of_elaboration()
        {
        }

        // called by elaboration_done (does nothing)
        public virtual void end_of_elaboration()
        {
        }

        // called by start_simulation (does nothing by default)
        public virtual void start_of_simulation()
        {
        }

        // called by simulation_done (does nothing by default)
        public virtual void end_of_simulation()
        {
        }

        // error reporting

        public void report_error(string id)
        {
            report_error(id, string.Empty);
        }

        public void report_error(string id, string add_msg)
        {
            string msg = string.Empty;
            if (string.IsNullOrEmpty(add_msg) == false)
            {
                msg = string.Format("{0}: port '{1}' ({2})", add_msg, name(), kind());
            }
            else
            {
                msg = string.Format("port '{0}' ({1})", name(), kind());
            }
            sc_report_handler.report(sc_core.sc_severity.SC_ERROR, id, msg);
        }

        // called by the sc_sensitive* classes
        public virtual void make_sensitive(sc_thread_process handle_)
        {
            make_sensitive(handle_, null);
        }

        public virtual void make_sensitive(sc_thread_process handle_, sc_event_finder event_finder_)
        {
            Debug.Assert(m_bind_info != null);
            m_bind_info.thread_vec.Add(new sc_bind_ef((sc_process_b)handle_, event_finder_));
        }
        public virtual void make_sensitive(sc_method_process handle_)
        {
            make_sensitive(handle_, null);
        }

        public virtual void make_sensitive(sc_method_process handle_, sc_event_finder event_finder_)
        {
            Debug.Assert(m_bind_info != null);
            m_bind_info.method_vec.Add(new sc_bind_ef((sc_process_b)handle_, event_finder_));
        }

        public abstract void add_static_event(sc_method_process process_p, sc_event Event);
        public abstract void add_static_event(sc_thread_process process_p, sc_event Event);


        // called by class sc_module for positional binding

        // called by class sc_module for positional binding

        public int pbind(sc_interface interface_)
        {
            if (m_bind_info == null)
            {
                // cannot bind an interface after elaboration
                report_error("bind interface to port failed", "simulation running");
            }

            if (m_bind_info.size() != 0)
            {
                // first interface already bound
                return 1;
            }

            return vbind(interface_);
        }
        public int pbind(sc_port_base parent_)
        {
            if (m_bind_info == null)
            {
                // cannot bind a parent port after elaboration
                report_error("bind parent port to port failed", "simulation running");
            }

            if (m_bind_info.size() != 0)
            {
                // first interface already bound
                return 1;
            }

            return vbind(parent_);
        }


        // support methods

        // support methods

        public int first_parent()
        {
            for (int i = 0; i < m_bind_info.size(); ++i)
            {
                if (m_bind_info.vec[i].parent != null)
                {
                    return i;
                }
            }
            return -1;
        }
        public void insert_parent(int i)
        {
            List<sc_bind_elem> vec = m_bind_info.vec;

            sc_port_base parent = vec[i].parent;


            // IF OUR PARENT HAS NO BINDING THEN IGNORE IT:
            //
            // Note that the zeroing of the parent pointer must occur before this
            // test

            vec[i].parent = null;
            if (parent.m_bind_info.vec.Count == 0)
                return;

            vec[i].iface = parent.m_bind_info.vec[0].iface;
            int n = parent.m_bind_info.size() - 1;
            if (n > 0)
            {
                // resize the bind vector (by adding new elements)
                for (int k = 0; k < n; ++k)
                {
                    vec.Add(new sc_bind_elem());
                }
                // move elements in the bind vector
                for (int k = m_bind_info.size() - n - 1; k > i; --k)
                {
                    vec[k + n].iface = vec[k].iface;
                    vec[k + n].parent = vec[k].parent;
                }
                // insert parent interfaces into the bind vector
                for (int k = i + 1; k <= i + n; ++k)
                {
                    vec[k].iface = parent.m_bind_info.vec[k - i].iface;
                    vec[k].parent = null;
                }
            }
        }

        // called when construction is done
        public void construction_done()
        {
            sc_module parent = (sc_module)(get_parent_object());
            sc_object.hierarchy_scope scope = new sc_object.hierarchy_scope(parent);
            before_end_of_elaboration();
        }

        // called when elaboration is done

        // called when elaboration is done

        public void complete_binding()
        {
            string msg_buffer = new string(new char[128]); // For error message construction.

            // IF BINDING HAS ALREADY BEEN COMPLETED IGNORE THIS CALL:

            Debug.Assert(m_bind_info != null);
            if (m_bind_info.complete)
            {
                return;
            }

            // COMPLETE BINDING OF OUR PARENT PORTS SO THAT WE CAN USE THAT INFORMATION:

            int i = first_parent();
            while (i >= 0)
            {
                m_bind_info.vec[i].parent.complete_binding();
                insert_parent(i);
                i = first_parent();
            }

            // LOOP OVER BINDING INFORMATION TO COMPLETE THE BINDING PROCESS:

            int size;
            for (int j = 0; j < m_bind_info.size(); ++j)
            {
                sc_interface iface = m_bind_info.vec[j].iface;

                // if the interface is zero this was for an unbound port.
                if (iface == null)
                    continue;

                // add (cache) the interface
                if (j > m_bind_info.last_add)
                {
                    add_interface(iface);
                }

                // only register "leaf" ports (ports without children)
                if (m_bind_info.is_leaf)
                {
                    iface.register_port(this, if_typename());
                }

                // complete static sensitivity for methods
                size = m_bind_info.method_vec.Count;
                for (int k = 0; k < size; ++k)
                {
                    sc_bind_ef p = m_bind_info.method_vec[k];
                    sc_event @event = (p.event_finder != null) ? p.event_finder.find_event(iface) : iface.default_event();
                    p.handle.add_static_event(@event);
                }

                // complete static sensitivity for threads
                size = m_bind_info.thread_vec.Count;
                for (int k = 0; k < size; ++k)
                {
                    sc_bind_ef p = m_bind_info.thread_vec[k];
                    sc_event @event = (p.event_finder != null) ? p.event_finder.find_event(iface) : iface.default_event();
                    p.handle.add_static_event(@event);
                }

            }

            // MAKE SURE THE PROPER NUMBER OF BINDINGS OCCURRED:
            //
            // Make sure there are enough bindings, and not too many.

            int actual_binds = interface_count();

            if (actual_binds > m_bind_info.max_size())
            {
                msg_buffer = string.Format("{0:D} binds exceeds maximum of {1:D} allowed", actual_binds, m_bind_info.max_size());
                report_error("complete binding failed", msg_buffer);
            }
            switch (m_bind_info.policy())
            {
                case sc_port_policy.SC_ONE_OR_MORE_BOUND:
                    if (actual_binds < 1)
                    {
                        report_error("complete binding failed", "port not bound");
                    }
                    break;
                case sc_port_policy.SC_ALL_BOUND:
                    if (actual_binds < m_bind_info.max_size() || actual_binds < 1)
                    {
                        msg_buffer = string.Format("{0:D} actual binds is less than required {1:D}", actual_binds, m_bind_info.max_size());
                        report_error("complete binding failed", msg_buffer);
                    }
                    break;
                default: // SC_ZERO_OR_MORE_BOUND:
                    break;
            }


            // CLEAN UP: FREE BINDING STORAGE:

            size = m_bind_info.method_vec.Count;
            for (int k = 0; k < size; ++k)
            {
                if (m_bind_info.method_vec[k] != null)
                    m_bind_info.method_vec[k].Dispose();
            }
            m_bind_info.method_vec.Clear();

            size = m_bind_info.thread_vec.Count;
            for (int k = 0; k < size; ++k)
            {
                if (m_bind_info.thread_vec[k] != null)
                    m_bind_info.thread_vec[k].Dispose();
            }
            m_bind_info.thread_vec.Clear();

            m_bind_info.complete = true;
        }
        public void elaboration_done()
        {
            Debug.Assert(m_bind_info != null && m_bind_info.complete);
            if (m_bind_info != null)
                m_bind_info.Dispose();
            m_bind_info = null;

            sc_module parent = (sc_module)(get_parent_object());
            sc_object.hierarchy_scope scope = new sc_object.hierarchy_scope(parent);
            end_of_elaboration();
        }

        // called before simulation starts
        public void start_simulation()
        {
            sc_module parent = (sc_module)(get_parent_object());
            sc_object.hierarchy_scope scope = new sc_object.hierarchy_scope(parent);
            start_of_simulation();
        }

        // called after simulation ends
        public void simulation_done()
        {
            sc_module parent = (sc_module)(get_parent_object());
            sc_object.hierarchy_scope scope = new sc_object.hierarchy_scope(parent);
            end_of_simulation();
        }


        protected sc_bind_info m_bind_info;

    }

}
