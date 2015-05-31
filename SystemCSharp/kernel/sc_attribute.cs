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
