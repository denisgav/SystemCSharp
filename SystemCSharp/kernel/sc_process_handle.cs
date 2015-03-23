using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sc_core
{
    public class sc_process_handle
    {
        protected sc_process_b m_target_p;   // Target for this object instance.

        protected static List<sc_event> empty_event_vector;  // If m_target_p == 0.
        protected static List<sc_object> empty_object_vector; // If m_target_p == 0.
        protected static sc_event non_event;           // If m_target_p == 0.

        public static bool operator ==(sc_process_handle left, sc_process_handle right)
        {
            return (left.m_target_p != null) && (right.m_target_p != null) &&
                (left.m_target_p == right.m_target_p);
        }

        public static bool operator !=(sc_process_handle left, sc_process_handle right)
        {
            return (left.m_target_p == null) || (right.m_target_p == null) ||
                (left.m_target_p != right.m_target_p);
        }


        //------------------------------------------------------------------------------
        //"sc_process_handle::sc_process_handle - non-pointer constructor"
        //
        // This version of the object instance constructor for this class creates
        // an object instance whose target needs to be supplied via an assignment.
        //------------------------------------------------------------------------------
        public sc_process_handle()
        {
            m_target_p = null;
        }

        //------------------------------------------------------------------------------
        //"sc_process_handle::sc_process_handle - pointer constructor"
        //
        // This version of the object instance constructor for this class creates
        // an object instance whose target is the supplied sc_object instance.
        // The supplied sc_object must in fact be an sc_process_b instance.
        //     object_p -> sc_object instance this is handle for.
        //------------------------------------------------------------------------------
        public sc_process_handle(sc_object object_p)
        {
            m_target_p = object_p as sc_process_b;
            if (m_target_p != null) m_target_p.reference_increment();
        }

        //------------------------------------------------------------------------------
        //"sc_process_handle::sc_process_handle - pointer constructor"
        //
        // This version of the object instance constructor for this class creates
        // an object instance whose target is the supplied sc_process_b instance.
        // This saves a dynamic cast compared to the sc_object* case.
        //     process_p -> process instance this is handle for.
        //------------------------------------------------------------------------------
        public sc_process_handle(sc_process_b process_p)
        {
            m_target_p = process_p;
            if (m_target_p != null) m_target_p.reference_increment();
        }

        //------------------------------------------------------------------------------
        //"sc_process_handle::sc_process_handle - copy constructor"
        //
        // This version of the object instance constructor for this class provides
        // the copy constructor for the class. It clones the supplied original
        // handle and increments the references to its target.
        //     orig = sc_process_handle object instance to be copied from.
        //------------------------------------------------------------------------------
        public sc_process_handle(sc_process_handle orig)
        {
            m_target_p = orig.m_target_p;
            if (m_target_p != null) m_target_p.reference_increment();
        }



        //------------------------------------------------------------------------------
        //"sc_process_handle::~sc_process_handle"
        //
        // This is the object instance destructor for this class. It decrements
        // the reference count for its target.
        //------------------------------------------------------------------------------
        public virtual void Dispose()
        {
            if (m_target_p != null) m_target_p.reference_decrement();
        }

        //------------------------------------------------------------------------------
        //"sc_process_handle::inline methods"
        //
        // These are short inline methods.
        //------------------------------------------------------------------------------

        // disable this object instance's target.

        public void disable(sc_descendant_inclusion_info descendants)
        {
            if (m_target_p != null)
                m_target_p.disable_process(descendants);
            else
                sc_report_handler.report(sc_core.sc_severity.SC_WARNING, "attempt to use an empty process handle ignored", "");
        }

        // call dont_initialize() on this object instance's target.

        public void dont_initialize(bool dont)
        {
            if (m_target_p != null)
                m_target_p.dont_initialize(dont);
            else
                sc_report_handler.report(sc_core.sc_severity.SC_WARNING, "attempt to use an empty process handle ignored", "");
        }

        // dump the status of this object instance's target:

        public string dump_state()
        {
            return (m_target_p != null) ? m_target_p.dump_state() : "NO TARGET";
        }

        // return whether this object instance's target is dynamic or not.

        public bool dynamic()
        {
            return (m_target_p != null) ? m_target_p.dynamic() : false;
        }

        // enable this object instance's target.

        public void enable(sc_descendant_inclusion_info descendants)
        {
            if (m_target_p != null)
                m_target_p.enable_process(descendants);
            else
                sc_report_handler.report(sc_core.sc_severity.SC_WARNING, "attempt to use an empty process handle ignored", "enable()");
        }

        // return the child objects for this object instance's target.

        public List<sc_event> get_child_events()
        {
            return (m_target_p != null) ? m_target_p.get_child_events() : empty_event_vector;
        }

        // return the child objects for this object instance's target.

        public List<sc_object> get_child_objects()
        {
            return (m_target_p != null) ? m_target_p.get_child_objects() : empty_object_vector;
        }

        // return the parent object for this object instance's target.

        public sc_object get_parent_object()
        {
            return (m_target_p != null) ? m_target_p.get_parent_object() : null;
        }

        // return this object instance's target.

        public sc_object get_process_object()
        {
            return m_target_p;
        }

        // return whether this object instance is unwinding or not.

        public bool is_unwinding()
        {
            if (m_target_p != null)
                return m_target_p.is_unwinding();
            else
            {
                return false;
            }
        }

        // kill this object instance's target.

        public void kill(sc_descendant_inclusion_info descendants)
        {
            if (m_target_p != null)
                m_target_p.kill_process(descendants);
            else
                sc_report_handler.report(sc_core.sc_severity.SC_WARNING, "attempt to use an empty process handle ignored", "kill()");
        }

        // return the name of this object instance's target.

        public string name()
        {
            return (m_target_p != null) ? m_target_p.name() : "";
        }

        // return the process kind for this object instance's target.

        public sc_curr_proc_kind proc_kind()
        {
            return (m_target_p != null) ? m_target_p.proc_kind() : sc_curr_proc_kind.SC_NO_PROC_;
        }

        // reset this object instance's target.

        public void reset(sc_descendant_inclusion_info descendants)
        {
            if (m_target_p != null)
                m_target_p.reset_process(reset_type.reset_asynchronous, descendants);
            else
                sc_report_handler.report(sc_core.sc_severity.SC_WARNING, "attempt to use an empty process handle ignored", "reset()");
        }

        // return the reset event for this object instance's target.

        public sc_event reset_event()
        {
            if (m_target_p != null)
                return m_target_p.reset_event();
            else
            {
                sc_report_handler.report(sc_core.sc_severity.SC_WARNING, "attempt to use an empty process handle ignored", "reset()");
                return sc_process_handle.non_event;
            }
        }

        // resume this object instance's target.

        public void resume(sc_descendant_inclusion_info descendants)
        {
            if (m_target_p != null)
                m_target_p.resume_process(descendants);
            else
                sc_report_handler.report(sc_core.sc_severity.SC_WARNING, "attempt to use an empty process handle ignored", "resume()");
        }

        // suspend this object instance's target.

        public void suspend(sc_descendant_inclusion_info descendants)
        {
            if (m_target_p != null)
                m_target_p.suspend_process(descendants);
            else
                sc_report_handler.report(sc_core.sc_severity.SC_WARNING, "attempt to use an empty process handle ignored", "suspend()");
        }

        // turn sync_reset off for this object instance's target.

        public void sync_reset_off(sc_descendant_inclusion_info descendants)
        {
            if (m_target_p != null)
                m_target_p.reset_process(reset_type.reset_synchronous_off, descendants);
            else
                sc_report_handler.report(sc_core.sc_severity.SC_WARNING, "attempt to use an empty process handle ignored", "sync_reset_off()");
        }

        // turn sync_reset on for this object instance's target.

        public void sync_reset_on(sc_descendant_inclusion_info descendants)
        {
            if (m_target_p != null)
            {
                m_target_p.reset_process(reset_type.reset_synchronous_on, descendants);
            }
            else
            {
                sc_report_handler.report(sc_core.sc_severity.SC_WARNING, "attempt to use an empty process handle ignored", "sync_reset_on()");
            }
        }

        // terminate this object instance's target.

        public bool terminated()
        {
            return (m_target_p != null) ? m_target_p.terminated() : false;
        }

        // return the termination event for this object instance's target.

        public sc_event terminated_event()
        {
            if (m_target_p != null)
                return m_target_p.terminated_event();
            else
            {
                sc_report_handler.report(sc_core.sc_severity.SC_WARNING, "attempt to use an empty process handle ignored", "terminated_event()");
                return sc_process_handle.non_event;
            }
        }

        // return true if this object instance has a target, false it not.

        public bool valid()
        {
            return m_target_p != null;
        }


        //------------------------------------------------------------------------------
        //"sc_process_b::last_created_process_handle"
        //
        // This method returns the kind of this process.
        //------------------------------------------------------------------------------
        public static sc_process_handle last_created_process_handle()
        {
            return new sc_process_handle(sc_process_b.m_last_created_process_p);
        }

        public static sc_process_handle sc_get_last_created_process_handle()
        {
            return last_created_process_handle();
        }
    }

}
