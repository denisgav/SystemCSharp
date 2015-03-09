using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemCSharp.Tracing;

namespace SystemCSharp.Kernel
{
    public class SimulationObject
    {
        private string name;
        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }

        public virtual string BaseName
        {
            get
            {
                string[] name_parts = name.Split(new char[] { SimulationContext.HIERARCHY_CHAR }, StringSplitOptions.RemoveEmptyEntries);
                if (name_parts.Length >= 2)
                    return name_parts[name_parts.Length - 2];
                else
                    return name_parts[name_parts.Length - 1];
            }
        }

        private SimulationContext simContext;
        public virtual SimulationContext SimulationContext
        {
            get 
            {
                if (simContext == null)
                    simContext = SimulationContext.GlobalSimContext;
                return simContext; 
            }
        }

        private List<AttributeBase> attributes;
        public List<AttributeBase> Attributes
        {
            get { return attributes; }
            set { attributes = value; }
        }

        public virtual AttributeBase GetAttributeByName(string attrName)
        {
            foreach (AttributeBase a in attributes)
            {
                if(a.Name.Equals(attrName, StringComparison.InvariantCulture))
                    return a;
            }
            return null;
        }

        public virtual void RemoveAttributeByName(string attrName)
        {
            int id = attributes.FindIndex(x => x.Name.Equals(attrName, StringComparison.InvariantCulture));
            if (id >= 0)
                attributes.RemoveAt(id);
        }

        public virtual void RemoveAllAttributes()
        {
            attributes.Clear();
        }



        private List<SimulationObject> childObjects;
        public virtual List<SimulationObject> ChildObjects
        {
            get { return childObjects; }
            set { childObjects = value; }
        }

        private List<SimulationEvent> childEvents;
        public virtual List<SimulationEvent> ChildEvents
        {
            get { return childEvents; }
            set { childEvents = value; }
        }

        private SimulationObject parent;
        public virtual SimulationObject Parent
        {
            get { return parent; }
            set { parent = value; }
        }  
             
        public virtual string Kind
        {
            get { return "SimulationObject"; }
        }

        public override string ToString()
        {
            return Print();
        }

        public virtual string Print()
        {
            return string.Format("Name = {0},\nKind = {1}", Name, Kind);
        }

        public virtual void Trace(TraceFile traceFile)
        {
            //This method has no implementation
        }

        private void Init(string name)
        {
            attributes = new List<AttributeBase>();
            childObjects = new List<SimulationObject>();
            childEvents = new List<SimulationEvent>();

            // SET UP POINTERS TO OBJECT MANAGER, PARENT, AND SIMULATION CONTEXT: 
            //
            // Make the current simcontext the simcontext for this object 

            SimulationContext simContext = this.SimulationContext;
            SimulationObjectManager object_manager = simContext.SimObjectManager;
            parent = simContext.GetActiveObject();

            // CONSTRUCT PATHNAME TO OBJECT BEING CREATED: 
            // 
            // If there is not a leaf name generate one. 

            this.name = name;


            // PLACE THE OBJECT INTO THE HIERARCHY

            object_manager.InsertObject(name, this);
            if (parent != null)
                parent.ChildObjects.Add(this);
            else
                simContext.ChildObjects.Add(this);
        }

        public SimulationObject()
            : this("object")
        { }

        public SimulationObject(string name)
        {
            Init(name);
        }

        public SimulationObject(SimulationObject o)
        {
            Init(o.BaseName);
        }        
    }
}
