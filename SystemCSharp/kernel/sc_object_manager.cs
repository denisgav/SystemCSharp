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
using System.Diagnostics;
using System;

namespace sc_core
{
    public class sc_object_manager : IDisposable
    {
        protected class sc_object_manager_table_entry : IDisposable
        {
            public sc_object_manager_table_entry(sc_event m_event_p = null, sc_object m_object_p = null)
            {
                this.m_event_p = m_event_p;
                this.m_object_p = m_object_p;
            }

            private sc_event m_event_p; // if non-null this is an sc_event.
            public sc_event Event
            {
                get { return m_event_p; }
                set { m_event_p = value; }
            }

            private sc_object m_object_p; // if non-null this is an sc_object.
            public sc_object Object
            {
                get { return m_object_p; }
                set { m_object_p = value; }
            }

            // Track whether Dispose has been called.
            private bool disposed = false;

            // +----------------------------------------------------------------------------
            // |"sc_object_manager::~sc_object_manager"
            // | 
            // | This is the object instance destructor for this class. It goes through
            // | each sc_object instance in the instance table and sets its m_simc field
            // | to NULL.
            // +----------------------------------------------------------------------------

            // Implement IDisposable.
            // Do not make this method virtual.
            // A derived class should not be able to override this method.
            public void Dispose()
            {
                Dispose(true);
                // This object will be cleaned up by the Dispose method.
                // Therefore, you should call GC.SupressFinalize to
                // take this object off the finalization queue
                // and prevent finalization code for this object
                // from executing a second time.
                GC.SuppressFinalize(this);
            }

            // Dispose(bool disposing) executes in two distinct scenarios.
            // If disposing equals true, the method has been called directly
            // or indirectly by a user's code. Managed and unmanaged resources
            // can be disposed.
            // If disposing equals false, the method has been called by the
            // runtime from inside the finalizer and you should not reference
            // other objects. Only unmanaged resources can be disposed.
            protected virtual void Dispose(bool disposing)
            {
                // Check to see if Dispose has already been called.
                if (!this.disposed)
                {
                    // If disposing equals true, dispose all managed
                    // and unmanaged resources.
                    if (disposing)
                    {
                        // Dispose managed resources.
                        if (m_event_p != null)
                            m_event_p.Dispose();
                        if (m_event_p != null)
                            m_event_p.Dispose();
                    }

                    // Call the appropriate methods to clean up
                    // unmanaged resources here.
                    // If disposing is false,
                    // only the following code is executed.

                    // Note disposing has been done.
                    disposed = true;

                }
            }

            // Use C# destructor syntax for finalization code.
            // This destructor will run only if the Dispose method
            // does not get called.
            // It gives your base class the opportunity to finalize.
            // Do not provide destructors in types derived from this class.
            ~sc_object_manager_table_entry()
            {
                // Do not re-create Dispose clean-up code here.
                // Calling Dispose(false) is optimal in terms of
                // readability and maintainability.
                Dispose(false);
            }
        }

        // ----------------------------------------------------------------------------
        //  CLASS : sc_object_manager
        //
        //  Manager of objects.
        // ----------------------------------------------------------------------------

        public sc_object_manager()
        {
            m_instance_table = new Dictionary<string, sc_object_manager_table_entry>();
            m_module_name_stack = new Stack<sc_module_name>();
            m_object_stack = new Stack<sc_object>();
        }



        // +----------------------------------------------------------------------------
        // |"sc_object_manager::find_event"
        // | 
        // | This method returns the sc_event with the supplied name, or a NULL if
        // | the event does not exist.
        // |
        // | Arguments:
        // |     name = name of the event
        // | Result is a pointer to the event or NULL if it does not exist.
        // +----------------------------------------------------------------------------
        public sc_event find_event(string name)
        {
            if (m_instance_table.ContainsKey(name))
                return m_instance_table[name].Event;

            return null;
        }


        // +----------------------------------------------------------------------------
        // |"sc_object_manager::find_object"
        // | 
        // | This method returns the sc_object with the supplied name, or a NULL if
        // | the object does not exist.
        // |
        // | Arguments:
        // |     name = name of the object
        // | Result is a pointer to the object or NULL if it does not exist.
        // +----------------------------------------------------------------------------
        public sc_object find_object(string name)
        {
            if (m_instance_table.ContainsKey(name))
                return m_instance_table[name].Object;

            return null;
        }

        public System.Collections.Generic.IEnumerable<sc_object> get_objects()
        {
            foreach (var item in m_instance_table)
            {
                yield return item.Value.Object;
            }
        }

        public System.Collections.Generic.IEnumerable<sc_event> get_events()
        {
            foreach (var item in m_instance_table)
            {
                yield return item.Value.Event;
            }
        }


        // +----------------------------------------------------------------------------
        // |"sc_object_manager::hierarchy_push"
        // | 
        // | This method pushes down the sc_object hierarchy to make the supplied 
        // | object the current object in the hierarchy.
        // |
        // | Arguments:
        // |     object_p -> object to become the new current object in the hierarchy.
        // +----------------------------------------------------------------------------
        public void hierarchy_push(sc_object object_p)
        {
            m_object_stack.Push(object_p);
        }

        // +----------------------------------------------------------------------------
        // |"sc_object_manager::hierarchy_pop"
        // | 
        // | This method pops the current object off the object hierarchy and returns
        // | it.
        // +----------------------------------------------------------------------------
        public sc_object hierarchy_pop()
        {
            if (m_object_stack.Count == 0)
                return null;
            return m_object_stack.Pop();
        }

        // +----------------------------------------------------------------------------
        // |"sc_object_manager::hierarchy_curr"
        // | 
        // | This method returns the current object in the object hierarchy or NULL
        // | if it does not exist.
        // +----------------------------------------------------------------------------
        public sc_object hierarchy_curr()
        {
            if (m_object_stack.Count == 0)
                return null;
            return m_object_stack.Peek();
        }

        // +----------------------------------------------------------------------------
        // |"sc_object_manager::hierarchy_size"
        // | 
        // | This method returns the current size of the object hierarchy stack.
        // +----------------------------------------------------------------------------
        public int hierarchy_size()
        {
            return m_object_stack.Count;
        }


        // +----------------------------------------------------------------------------
        // |"sc_object_manager::push_module_name"
        // | 
        // | This method pushes the supplied entry onto the module name stack.
        // |
        // | Arguments:
        // |     mod_name_p -> entry to push onto the module name stack.
        // +----------------------------------------------------------------------------
        public void push_module_name(sc_module_name mod_name_p)
        {
            m_module_name_stack.Push(mod_name_p);
        }

        // +----------------------------------------------------------------------------
        // |"sc_object_manager::pop_module_name"
        // | 
        // | This method pops an entry off the module name stack and returns it.
        // +----------------------------------------------------------------------------
        public sc_module_name pop_module_name()
        {
            if (m_module_name_stack.Count == 0)
                return null;
            else
                return m_module_name_stack.Pop();
        }

        // +----------------------------------------------------------------------------
        // |"sc_object_manager::top_of_module_name_stack"
        // | 
        // | This method returns the module name that is on the top of the module
        // | name stack.
        // +----------------------------------------------------------------------------
        public sc_module_name top_of_module_name_stack()
        {
            if (m_module_name_stack.Count == 0)
            {
                sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "module name stack is empty: did you forget to add a sc_module_name parameter to your module constructor?", "");
                return null;
            }
            else
                return m_module_name_stack.Peek();
        }



        // +----------------------------------------------------------------------------
        // |"sc_object_manager::create_name"
        // | 
        // | This method creates a hierarchical name based on the name of the active 
        // | object and the supplied leaf name. If the resultant name is not unique it 
        // | will be made unique and a warning message issued.
        // |
        // | Arguments:
        // |     leaf_name = name to use for the leaf of the hierarchy.
        // | Result is an std::string containing the name.
        // +----------------------------------------------------------------------------
        public string create_name(string leaf_name)
        {
            bool clash; // true if path name exists in obj table
            string leafname_string; // string containing the leaf name.
            string parentname_string; // parent path name
            sc_object parent_p; // parent for this instance or NULL.
            string result_orig_string; // save for warning message.
            string result_string; // name to return.

            // CONSTRUCT PATHNAME TO THE NAME TO BE RETURNED:
            //
            // If there is not a leaf name generate one.

            parent_p = sc_simcontext.sc_get_curr_simcontext().active_object();
            parentname_string = parent_p != null ? parent_p.name() : "";
            leafname_string = leaf_name;
            if (parent_p != null)
            {
                result_string = parentname_string;
                result_string += sc_object.SC_HIERARCHY_CHAR;
                result_string += leafname_string;
            }
            else
            {
                result_string = leafname_string;
            }

            // SAVE the original path name

            result_orig_string = result_string;

            // MAKE SURE THE ENTITY NAME IS UNIQUE:
            //
            // If not use unique name generator to make it unique.

            clash = false;
            for (; ; )
            {
                if (m_instance_table.ContainsKey(result_string) == false)
                {
                    break;
                }
                clash = true;

                leafname_string = sc_simcontext.sc_gen_unique_name(leafname_string, false);
                if (parent_p != null)
                {
                    result_string = parentname_string;
                    result_string += sc_object.SC_HIERARCHY_CHAR;
                    result_string += leafname_string;
                }
                else
                {
                    result_string = leafname_string;
                }
            }
            if (clash)
            {
                string message = result_orig_string;
                message += ". Latter declaration will be renamed to ";
                message += result_string;
                sc_report_handler.report(sc_core.sc_severity.SC_WARNING, "object already exists", message);
            }

            return result_string;
        }

        // +----------------------------------------------------------------------------
        // |"sc_object_manager::insert_event"
        // | 
        // | This method inserts the supplied sc_event instance into the instance
        // | table using the supplied name.
        // |
        // | Arguments:
        // |     name    =  name of the event to be inserted.
        // |     event_p -> event to be inserted.
        // +----------------------------------------------------------------------------
        public void insert_event(string name, sc_event event_p)
        {
            if (m_instance_table.ContainsKey(name) == true)
                m_instance_table[name].Event = event_p;
            else
                m_instance_table.Add(name, new sc_object_manager_table_entry() { Event = event_p });
        }

        // +----------------------------------------------------------------------------
        // |"sc_object_manager::insert_object"
        // | 
        // | This method inserts the supplied sc_object instance into the instance
        // | table using the supplied name.
        // |
        // | Arguments:
        // |     name     =  name of the event to be inserted.
        // |     object_p -> object to be inserted.
        // +----------------------------------------------------------------------------
        public virtual void insert_object(string name, sc_object object_p)
        {
            if (m_instance_table.ContainsKey(name) == true)
                m_instance_table[name].Object = object_p;
            else
                m_instance_table.Add(name, new sc_object_manager_table_entry() { Object = object_p });
        }

        // +----------------------------------------------------------------------------
        // |"sc_object_manager::remove_event"
        // | 
        // | This method removes the sc_event instance with the supplied name from
        // | the table of instances. Note we just clear the pointer since if the name
        // | was for an sc_object the m_event_p pointer will be null anyway.
        // |
        // | Arguments:
        // |     name = name of the event to be removed.
        // +----------------------------------------------------------------------------
        public void remove_event(string name)
        {
            if (m_instance_table.ContainsKey(name))
                m_instance_table[name].Event = null;
        }

        // +----------------------------------------------------------------------------
        // |"sc_object_manager::remove_object"
        // | 
        // | This method removes the sc_object instance with the supplied name from
        // | the table of instances. Note we just clear the pointer since if the name
        // | was for an sc_event the m_object_p pointer will be null anyway.
        // |
        // | Arguments:
        // |     name = name of the object to be removed.
        // +----------------------------------------------------------------------------
        public virtual void remove_object(string name)
        {
            if (m_instance_table.ContainsKey(name))
                m_instance_table[name].Object = null;
        }

        private Dictionary<string, sc_object_manager_table_entry> m_instance_table = new Dictionary<string, sc_object_manager_table_entry>();
        private Stack<sc_module_name> m_module_name_stack = new Stack<sc_module_name>(); // sc_module_name stack.
        private Stack<sc_object> m_object_stack = new Stack<sc_object>();

        // Track whether Dispose has been called.
        private bool disposed = false;

        // +----------------------------------------------------------------------------
        // |"sc_object_manager::~sc_object_manager"
        // | 
        // | This is the object instance destructor for this class. It goes through
        // | each sc_object instance in the instance table and sets its m_simc field
        // | to NULL.
        // +----------------------------------------------------------------------------

        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    m_instance_table.Clear();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                // If disposing is false,
                // only the following code is executed.

                // Note disposing has been done.
                disposed = true;

            }
        }

        // Use C# destructor syntax for finalization code.
        // This destructor will run only if the Dispose method
        // does not get called.
        // It gives your base class the opportunity to finalize.
        // Do not provide destructors in types derived from this class.
        ~sc_object_manager()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
    }
}