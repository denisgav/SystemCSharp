using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemCSharp.Kernel
{
    /// <summary>
    /// Attribute classes.
    /// </summary>
    public class AttributeBase
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public AttributeBase(string name)
        {
            this.name = name;
        }

        public AttributeBase(AttributeBase attr)
            : this(attr.Name)
        {            
        }
    }

    /// <summary>
    /// Attribute classes.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Attribute<T> : AttributeBase
    {
        private T _value;

        public T Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public Attribute(string name)
            :base(name)
        {
        }

        public Attribute(string name, T _value)
            : base(name)
        {
            this._value = _value;
        }

        public Attribute(Attribute<T> a)
            : base(a)
        {
            this._value = a._value;
        }        
    }
}
