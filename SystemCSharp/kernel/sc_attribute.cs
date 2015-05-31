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


    public class sc_attr_cltn : IDisposable
    {
        public sc_attr_cltn()
        {
            m_cltn = new List<sc_attr_base>();
        }
        public sc_attr_cltn(sc_attr_cltn a)
        {
            m_cltn = a.m_cltn;
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
                    remove_all();
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
        ~sc_attr_cltn()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }

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
