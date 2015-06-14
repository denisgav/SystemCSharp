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
using System;
namespace sc_core
{

    // ----------------------------------------------------------------------------
    //  CLASS : sc_object
    //
    //  Abstract base class of all SystemC `simulation' objects.
    // ----------------------------------------------------------------------------

    public class sc_object
    {
        public static sc_object sc_get_parent(sc_object obj_p)
        {
            return obj_p.get_parent_object();
        }

        public const char SC_HIERARCHY_CHAR = '.';

        // This will be gotten rid after multiple-processes
        //   are implemented.  This is to fix some regression
        //   problems. 
        public static bool sc_enable_name_checking = true;

        internal static int sc_object_num = 0;

        internal static string sc_object_newname()
        {
            return string.Format("{0}", sc_object_num);
        }


        internal static bool object_name_illegal_char(char ch)
        {
            return (ch == SC_HIERARCHY_CHAR) || char.IsWhiteSpace(ch);
        }

        public string name()
        {
            return m_name;
        }

        public string basename()
        {
            string[] name_parts = m_name.Split(new char[] { SC_HIERARCHY_CHAR }, StringSplitOptions.RemoveEmptyEntries);
            if (name_parts.Length >= 2)
                return name_parts[name_parts.Length - 2];
            else
                return name_parts[name_parts.Length - 1];
        }

        public virtual string print()
        {
            return name();
        }


        public virtual string dump()
        {
            return string.Format("name = {0} \nkind = \n", name(), kind());
        }

        /*
        public virtual void trace(sc_trace_file tf)
        {
            // This space is intentionally left blank
        }
        */
        public virtual string kind()
        {
            return "sc_object";
        }

        public sc_simcontext simcontext()
        {
            if (m_simc == null)
                m_simc = sc_simcontext.sc_get_curr_simcontext();
            return m_simc;
        }


        public bool add_attribute(ref sc_attr_base attribute_)
        {
            if (m_attr_cltn_p == null)
                m_attr_cltn_p = new sc_attr_cltn();
            return (m_attr_cltn_p.push_back(attribute_));
        }


        public sc_attr_base get_attribute(string name_)
        {
            if (m_attr_cltn_p == null)
                m_attr_cltn_p = new sc_attr_cltn();
            return m_attr_cltn_p[name_];
        }


        public sc_attr_base remove_attribute(string name_)
        {
            if (m_attr_cltn_p != null)
                return (m_attr_cltn_p.remove(name_));
            else
                return null;
        }


        public void remove_all_attributes()
        {
            if (m_attr_cltn_p != null)
                m_attr_cltn_p.remove_all();
        }


        public int num_attributes()
        {
            if (m_attr_cltn_p != null)
                return (m_attr_cltn_p.size());
            else
                return 0;
        }


        public sc_attr_cltn attr_cltn()
        {
            if (m_attr_cltn_p == null)
                m_attr_cltn_p = new sc_attr_cltn();
            return m_attr_cltn_p;
        }


        public virtual List<sc_event> get_child_events()
        {
            return m_child_events;
        }


        public virtual List<sc_object> get_child_objects()
        {
            return m_child_objects;
        }


        private bool get_parent_warn_sc_get_parent_deprecated = true;


        public sc_object get_parent()
        {

            if (get_parent_warn_sc_get_parent_deprecated)
            {
                get_parent_warn_sc_get_parent_deprecated = false;
                sc_report_handler.report(sc_core.sc_severity.SC_INFO, "/IEEE_Std_1666/deprecated", "sc_object::get_parent() is deprecated, " + "use get_parent_object() instead");
            }
            return get_parent_object();
        }

        public sc_object get_parent_object()
        {
            return m_parent;
        }


        protected sc_object()
        {
            m_attr_cltn_p = null;
            m_child_events = new List<sc_event>();
            m_child_objects = new List<sc_object>();
            m_name = string.Empty;
            m_parent = null;
            m_simc = null;
            sc_object_init(sc_simcontext.sc_gen_unique_name("object"));
        }
        protected sc_object(string nm)
        {
            m_attr_cltn_p = null;
            m_child_events = new List<sc_event>();
            m_child_objects = new List<sc_object>();
            m_name = string.Empty;
            m_parent = null;
            m_simc = null;
            string namebuf = null;
            string p;

            // null name or "" uses machine generated name.

            if (string.IsNullOrEmpty(nm))
                nm = sc_simcontext.sc_gen_unique_name("object");
            p = nm;

            if (sc_object.sc_enable_name_checking)
            {
                string r = nm;
                bool has_illegal_char = false;
                foreach(char c in r)
                {
                    if (sc_object.object_name_illegal_char(c))
                    {
                        has_illegal_char = true;
                        break;
                    }
                }
                if (has_illegal_char)
                {
                    string message = nm;
                    message += " substituted by ";
                    message += namebuf;
                    sc_report_handler.report(sc_core.sc_severity.SC_WARNING, "illegal characters", message);
                }
            }
            sc_object_init(p);
        }

        protected sc_object(sc_object that)
        {
            m_attr_cltn_p = null;
            m_child_events = new List<sc_event>();
            m_child_objects = new List<sc_object>();
            m_name = string.Empty;
            m_parent = null;
            m_simc = null;
            sc_object_init(sc_simcontext.sc_gen_unique_name(that.basename()));
        }


        public virtual void Dispose()
        {
            detach();
            if (m_attr_cltn_p != null)
                m_attr_cltn_p.Dispose();
        }


        // ----------------------------------------------------------------------------
        //  CLASS : sc_object
        //
        //  Abstract base class of all SystemC `simulation' objects.
        // ----------------------------------------------------------------------------

        public virtual void add_child_event(sc_event event_p)
        {
            // no check if event_p is already in the set
            m_child_events.Add(event_p);
        }
        public virtual void add_child_object(sc_object object_)
        {
            // no check if object_ is already in the set
            m_child_objects.Add(object_);
        }

        // +----------------------------------------------------------------------------
        // |"sc_object::remove_child_event"
        // | 
        // | This virtual method removes the supplied event from the list of child
        // | events if it is present.
        // |
        // | Arguments:
        // |     event_p -> event to be removed.
        // | Returns true if the event was present, false if not.
        // +----------------------------------------------------------------------------
        protected virtual bool remove_child_event(sc_event event_p)
        {
            int size = m_child_events.Count;
            for (int i = 0; i < size; ++i)
            {
                if (event_p == m_child_events[i])
                {
                    m_child_events[i] = m_child_events[size - 1];
                    m_child_events.RemoveAt(m_child_events.Count - 1);
                    return true;
                }
            }
            return false;
        }

        // +----------------------------------------------------------------------------
        // |"sc_object::remove_child_object"
        // | 
        // | This virtual method removes the supplied object from the list of child
        // | objects if it is present.
        // |
        // | Arguments:
        // |     object_p -> object to be removed.
        // | Returns true if the object was present, false if not.
        // +----------------------------------------------------------------------------
        public virtual bool remove_child_object(sc_object object_p)
        {
            int size = m_child_objects.Count;
            for (int i = 0; i < size; ++i)
            {
                if (object_p == m_child_objects[i])
                {
                    m_child_objects[i] = m_child_objects[size - 1];
                    m_child_objects.RemoveAt(m_child_objects.Count - 1);
                    object_p.m_parent = null;
                    return true;
                }
            }
            return false;
        }

        //---------------------------------------------------------------------------------
        //\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\
        /*
        protected uint register_simulation_phase_callback(uint mask)
        {
            mask = simcontext().m_phase_cb_registry.register_callback(this, mask);
            return mask;
        }

        protected uint unregister_simulation_phase_callback(uint mask)
        {
            mask = simcontext().m_phase_cb_registry.unregister_callback(this, mask);
            return mask;
        }
        */
        //\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\
        //---------------------------------------------------------------------------------
        protected virtual void simulation_phase_callback()
        {
            sc_report_handler.report(sc_core.sc_severity.SC_WARNING, "empty simulation phase callback called", name());
        }


        //------------------------------------------------------------------------------
        //"sc_object::detach"
        //
        // This method detaches this object instance from the object hierarchy.
        // It is called in two places: ~sc_object() and sc_process_b::kill_process().
        //------------------------------------------------------------------------------
        public virtual void detach()
        {
            if (m_simc != null)
            {

                // REMOVE OBJECT FROM THE OBJECT MANAGER:

                sc_object_manager object_manager = m_simc.get_object_manager();
                object_manager.remove_object(m_name);

                // REMOVE OBJECT FROM PARENT'S LIST OF OBJECTS:

                if (m_parent != null)
                    m_parent.remove_child_object(this);
                else
                    m_simc.remove_child_object(this);
            }
        }

        // +----------------------------------------------------------------------------
        // |"sc_object::orphan_child_events"
        // | 
        // | This method moves the children of this object instance to be children
        // | of the simulator.
        // +----------------------------------------------------------------------------
        public virtual void orphan_child_events()
        {
            List<sc_event> events = get_child_events();

            foreach (sc_event e in events)
            {
                e.set_parent_object(null);
                simcontext().add_child_event(e);
            }
        }

        // +----------------------------------------------------------------------------
        // |"sc_object::orphan_child_objects"
        // | 
        // | This method moves the children of this object instance to be children
        // | of the simulator.
        // +----------------------------------------------------------------------------
        public virtual void orphan_child_objects()
        {
            List<sc_object> children = get_child_objects();

            foreach (sc_object o in children)
            {
                o.m_parent = null;
                simcontext().add_child_object(o);
            }
        }

        protected sc_simcontext sc_get_curr_simcontext()
        { return simcontext(); }

        // +----------------------------------------------------------------------------
        // |"sc_object::sc_object_init"
        // | 
        // | This method initializes this object instance and places it in to the
        // | object hierarchy if the supplied name is not NULL.
        // |
        // | Arguments:
        // |     nm = leaf name for the object.
        // +----------------------------------------------------------------------------
        private void sc_object_init(string nm)
        {
            // SET UP POINTERS TO OBJECT MANAGER, PARENT, AND SIMULATION CONTEXT:
            //
            // Make the current simcontext the simcontext for this object

            m_simc = sc_get_curr_simcontext();
            m_attr_cltn_p = null;
            sc_object_manager object_manager = m_simc.get_object_manager();
            m_parent = m_simc.active_object();

            // CONSTRUCT PATHNAME TO OBJECT BEING CREATED:
            //
            // If there is not a leaf name generate one.

            m_name = object_manager.create_name((string.IsNullOrEmpty(nm) == false) ? nm : sc_object_newname());


            // PLACE THE OBJECT INTO THE HIERARCHY

            object_manager.insert_object(m_name, this);
            if (m_parent != null)
                m_parent.add_child_object(this);
            else
                m_simc.add_child_object(this);
        }

        public void do_simulation_phase_callback()
        {
            simulation_phase_callback();
        }


        // Each simulation object is associated with a simulation context  
        private sc_attr_cltn m_attr_cltn_p; // attributes for this object.
        private List<sc_event> m_child_events = new List<sc_event>(); // list of child events.
        protected List<sc_object> m_child_objects = new List<sc_object>(); // list of child objects.
        private string m_name; // name of this object.
        private sc_object m_parent; // parent for this object.
        private sc_simcontext m_simc; // simcontext ptr / empty indicator

        public class hierarchy_scope
        {
            public hierarchy_scope(sc_object obj)
            {
                scope_ = null;
                if (obj == null)
                    return;

                scope_ = obj as sc_module;
                if (scope_ == null)
                    scope_ = obj.get_parent_object() as sc_module;
                if (scope_ != null)
                    scope_.simcontext().hierarchy_push(scope_);
            }
            public hierarchy_scope(sc_module mod)
            {
                scope_ = mod;
                if (scope_ != null)
                    scope_.simcontext().hierarchy_push(scope_);
            }
            public hierarchy_scope()
            {
                if (scope_ != null)
                    scope_.simcontext().hierarchy_pop();
            }

            private sc_module scope_;
        }

    }

} // namespace sc_core