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
    //==============================================================================
    // sc_method_process -
    //
    //==============================================================================
    public class sc_method_process : sc_process_b
    {
        //------------------------------------------------------------------------------
        //"sc_method_process::sc_method_process"
        //
        // This is the object instance constructor for this class.
        //------------------------------------------------------------------------------
        public sc_method_process(string name_p, bool free_host, sc_process_call_base method_p, sc_process_host host_p, sc_spawn_options opt_p)
            : base((string.IsNullOrEmpty(name_p) == false) ? name_p : sc_simcontext.sc_gen_unique_name("method_p"), false, free_host, method_p, host_p, opt_p)
        {
            m_cor = null;
            m_stack_size = 0;
            m_monitor_q = new List<sc_process_monitor>();

            // CHECK IF THIS IS AN sc_module-BASED PROCESS AND SIMUALTION HAS STARTED:

            if ((host_p as sc_module) != null && sc_simcontext.sc_is_running())
            {
                report_error("call to SC_METHOD in sc_module while simulation running", "");
            }

            // INITIALIZE VALUES:
            //
            // If there are spawn options use them.

            m_process_kind = sc_curr_proc_kind.SC_METHOD_PROC_;
            if (opt_p != null)
            {
                m_dont_init = opt_p.m_dont_initialize;

                //---------------------------------------------------------------------------------
                //\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\
                /*
                // traverse event sensitivity list
                for (uint i = 0; i < opt_p.m_sensitive_events.Count; i++)
                {
                    sc_sensitive.make_static_sensitivity(this, ref *opt_p.m_sensitive_events[i]);
                }

                // traverse port base sensitivity list
                for (uint i = 0; i < opt_p.m_sensitive_port_bases.Count; i++)
                {
                    sc_sensitive.make_static_sensitivity(this, ref *opt_p.m_sensitive_port_bases[i]);
                }

                // traverse interface sensitivity list
                for (uint i = 0; i < opt_p.m_sensitive_interfaces.Count; i++)
                {
                    sc_sensitive.make_static_sensitivity(this, ref *opt_p.m_sensitive_interfaces[i]);
                }

                // traverse event finder sensitivity list
                for (uint i = 0; i < opt_p.m_sensitive_event_finders.Count; i++)
                {
                    sc_sensitive.make_static_sensitivity(this, ref *opt_p.m_sensitive_event_finders[i]);
                }
                
                // process any reset signal specification:

                opt_p.specify_resets();
                */
                //---------------------------------------------------------------------------------
                //\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\
            }

            else
            {
                m_dont_init = false;
            }
        }

        //C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
        //ORIGINAL LINE: virtual string kind() const
        public override string kind()
        {
            return "sc_method_process";
        }


        // +----------------------------------------------------------------------------
        // |"sc_method_process::check_for_throws"
        // | 
        // | This method checks to see if this method process should throw an exception
        // | or not. It is called from sc_simcontext::preempt_with() to see if the
        // | thread that was executed during the preemption did a kill or other 
        // | manipulation on this object instance that requires it to throw an 
        // | exception.
        // +----------------------------------------------------------------------------
        public virtual void check_for_throws()
        {
            if (!m_unwinding)
            {
                switch (m_throw_status)
                {
                    case process_throw_type.THROW_ASYNC_RESET:
                        simcontext().preempt_with(this);
                        break;
                    case process_throw_type.THROW_KILL:
                        throw new sc_unwind_exception(this, false);
                    default:
                        break;
                }
            }
        }

        //------------------------------------------------------------------------------
        //"sc_method_process::disable_process"
        //
        // This virtual method disables this process and its children if requested to.
        //     descendants = indicator of whether this process' children should also
        //                   be suspended
        //------------------------------------------------------------------------------
        public override void disable_process()
        {
            disable_process(sc_descendant_inclusion_info.SC_NO_DESCENDANTS);
        }

        public override void disable_process(sc_descendant_inclusion_info descendants)
        {

            // IF NEEDED PROPOGATE THE SUSPEND REQUEST THROUGH OUR DESCENDANTS:

            if (descendants == sc_descendant_inclusion_info.SC_INCLUDE_DESCENDANTS)
            {
                List<sc_object> children = get_child_objects();
                int child_n = children.Count;

                for (int child_i = 0; child_i < child_n; child_i++)
                {
                    sc_process_b child_p = children[child_i] as sc_process_b;
                    if (child_p != null)
                        child_p.disable_process(descendants);
                }
            }

            // DON'T ALLOW CORNER CASE BY DEFAULT:

            if (!sc_simcontext.sc_allow_process_control_corners)
            {
                switch (m_trigger_type)
                {
                    case trigger_t.AND_LIST_TIMEOUT:
                    case trigger_t.EVENT_TIMEOUT:
                    case trigger_t.OR_LIST_TIMEOUT:
                    case trigger_t.TIMEOUT:
                        report_error("Undefined process control interaction", "attempt to disable a method with timeout wait");
                        break;
                    default:
                        break;
                }
            }

            // DISABLE OUR OBJECT INSTANCE:

            m_state = m_state | (int)process_state.ps_bit_disabled;

            // IF THIS CALL IS BEFORE THE SIMULATION DON'T RUN THE METHOD:

            if (!sc_simcontext.sc_is_running())
            {
                sc_simcontext.sc_get_curr_simcontext().remove_runnable_method(this);
            }
        }

        //------------------------------------------------------------------------------
        //"sc_method_process::enable_process"
        //
        // This method enables the execution of this process, and if requested, its
        // descendants. If the process was suspended and has a resumption pending it 
        // will be dispatched in the next delta cycle. Otherwise the state will be
        // adjusted to indicate it is no longer suspended, but no immediate execution
        // will occur.
        //------------------------------------------------------------------------------
        public override void enable_process()
        {
            enable_process(sc_descendant_inclusion_info.SC_NO_DESCENDANTS);
        }

        public override void enable_process(sc_descendant_inclusion_info descendants)
        {

            // IF NEEDED PROPOGATE THE RESUME REQUEST THROUGH OUR DESCENDANTS:

            if (descendants == sc_descendant_inclusion_info.SC_INCLUDE_DESCENDANTS)
            {
                List<sc_object> children = get_child_objects();
                int child_n = children.Count;

                for (int child_i = 0; child_i < child_n; child_i++)
                {
                    sc_process_b child_p = children[child_i] as sc_process_b;
                    if (child_p != null)
                        child_p.enable_process(descendants);
                }
            }

            // ENABLE THIS OBJECT INSTANCE:
            //
            // If it was disabled and ready to run then put it on the run queue.

            m_state = m_state & ~(int)process_state.ps_bit_disabled;
            if (m_state == (int)process_state.ps_bit_ready_to_run)
            {
                m_state = (int)process_state.ps_normal;
                if (next_runnable() == null)
                    simcontext().push_runnable_method(this);
            }
        }

        // +----------------------------------------------------------------------------
        // |"sc_method_process::run_process"
        // | 
        // | This method executes this object instance, including fielding exceptions.
        // |
        // | Result is false if an unfielded exception occurred, true if not.
        // +----------------------------------------------------------------------------
        public virtual bool run_process()
        {
            // Execute this object instance's semantics and catch any exceptions that
            // are generated:

            bool restart = false;
            do
            {
                try
                {
                    semantics();
                    restart = false;
                }
                catch (sc_unwind_exception ex)
                {
                    ex.clear();
                    restart = ex.is_reset();
                }
                catch (Exception ex)
                {
                    //simcontext().set_error(ex);
                    throw ex;
                }
            } while (restart);

            return true;
        }

        //------------------------------------------------------------------------------
        //"sc_method_process::kill_process"
        //
        // This method removes throws a kill for this object instance. It calls the
        // sc_process_b::kill_process() method to perform low level clean up. 
        //------------------------------------------------------------------------------
        public override void kill_process()
        {
            kill_process(sc_descendant_inclusion_info.SC_NO_DESCENDANTS);
        }

        public override void kill_process(sc_descendant_inclusion_info descendants)
        {

            // IF THE SIMULATION HAS NOT BEEN INITIALIZED YET THAT IS AN ERROR:

            if (sc_simcontext.sc_get_status() == sc_status.SC_ELABORATION)
            {
                report_error("a process may not be killed before it is initialized");
            }

            // IF NEEDED, PROPOGATE THE KILL REQUEST THROUGH OUR DESCENDANTS:

            if (descendants == sc_descendant_inclusion_info.SC_INCLUDE_DESCENDANTS)
            {
                List<sc_object> children = get_child_objects();
                int child_n = children.Count;

                for (int child_i = 0; child_i < child_n; child_i++)
                {
                    sc_process_b child_p = children[child_i] as sc_process_b;
                    if (child_p != null)
                        child_p.kill_process(descendants);
                }
            }

            // IF THE PROCESS IS CURRENTLY UNWINDING OR IS ALREADY A ZOMBIE
            // IGNORE THE KILL:

            if (m_unwinding)
            {
                global::sc_core.sc_report_handler.report(sc_core.sc_severity.SC_WARNING, "kill/reset ignored during unwinding", name());
                return;
            }

            if ((m_state & (int)process_state.ps_bit_zombie) != 0)
                return;


            // REMOVE OUR PROCESS FROM EVENTS, ETC., AND IF ITS THE ACTIVE PROCESS
            // THROW ITS KILL.
            //
            // Note we set the throw status to kill regardless if we throw or not.
            // That lets check_for_throws stumble across it if we were in the call
            // chain when the kill call occurred.

            if (next_runnable() != null)
                simcontext().remove_runnable_method(this);
            disconnect_process();

            m_throw_status = process_throw_type.THROW_KILL;
            if (sc_simcontext.sc_get_current_process_b() == this)
            {
                throw new sc_unwind_exception(this, false);
            }
        }
        public sc_method_process next_exist()
        {
            return (sc_method_process)m_exist_p;
        }


        public sc_method_process next_runnable()
        {
            return (sc_method_process)m_runnable_p;
        }

        //------------------------------------------------------------------------------
        //"sc_method_process::clear_trigger"
        //
        // This method clears any pending trigger for this object instance.
        //------------------------------------------------------------------------------
        public virtual void clear_trigger()
        {
            switch (m_trigger_type)
            {
                case trigger_t.STATIC:
                    return;
                case trigger_t.EVENT:
                    m_event_p.remove_dynamic(this);
                    m_event_p = null;
                    break;
                case trigger_t.OR_LIST:
                    m_event_list_p.remove_dynamic(this, null);
                    m_event_list_p.auto_delete();
                    m_event_list_p = null;
                    break;
                case trigger_t.AND_LIST:
                    m_event_list_p.remove_dynamic(this, null);
                    m_event_list_p.auto_delete();
                    m_event_list_p = null;
                    m_event_count = 0;
                    break;
                case trigger_t.TIMEOUT:
                    m_timeout_event_p.cancel();
                    m_timeout_event_p.reset();
                    break;
                case trigger_t.EVENT_TIMEOUT:
                    m_timeout_event_p.cancel();
                    m_timeout_event_p.reset();
                    m_event_p.remove_dynamic(this);
                    m_event_p = null;
                    break;
                case trigger_t.OR_LIST_TIMEOUT:
                    m_timeout_event_p.cancel();
                    m_timeout_event_p.reset();
                    m_event_list_p.remove_dynamic(this, null);
                    m_event_list_p.auto_delete();
                    m_event_list_p = null;
                    break;
                case trigger_t.AND_LIST_TIMEOUT:
                    m_timeout_event_p.cancel();
                    m_timeout_event_p.reset();
                    m_event_list_p.remove_dynamic(this, null);
                    m_event_list_p.auto_delete();
                    m_event_list_p = null;
                    m_event_count = 0;
                    break;
            }
            m_trigger_type = trigger_t.STATIC;
        }
        public virtual void next_trigger(sc_event e)
        {
            clear_trigger();
            e.add_dynamic(this);
            m_event_p = e;
            m_trigger_type = trigger_t.EVENT;
        }
        public virtual void next_trigger(sc_event_or_list el)
        {
            clear_trigger();
            el.add_dynamic(this);
            m_event_list_p = el;
            m_trigger_type = trigger_t.OR_LIST;
        }
        public virtual void next_trigger(sc_event_and_list el)
        {
            clear_trigger();
            el.add_dynamic(this);
            m_event_list_p = el;
            m_event_count = el.size();
            m_trigger_type = trigger_t.AND_LIST;
        }
        public virtual void next_trigger(sc_time t)
        {
            clear_trigger();
            m_timeout_event_p.notify_internal(new sc_time(t));
            m_timeout_event_p.add_dynamic(this);
            m_trigger_type = trigger_t.TIMEOUT;
        }
        public virtual void next_trigger(sc_time t, sc_event e)
        {
            clear_trigger();
            m_timeout_event_p.notify_internal(new sc_time(t));
            m_timeout_event_p.add_dynamic(this);
            e.add_dynamic(this);
            m_event_p = e;
            m_trigger_type = trigger_t.EVENT_TIMEOUT;
        }
        public virtual void next_trigger(sc_time t, sc_event_or_list el)
        {
            clear_trigger();
            m_timeout_event_p.notify_internal(new sc_time(t));
            m_timeout_event_p.add_dynamic(this);
            el.add_dynamic(this);
            m_event_list_p = el;
            m_trigger_type = trigger_t.OR_LIST_TIMEOUT;
        }
        public virtual void next_trigger(sc_time t, sc_event_and_list el)
        {
            clear_trigger();
            m_timeout_event_p.notify_internal(new sc_time(t));
            m_timeout_event_p.add_dynamic(this);
            el.add_dynamic(this);
            m_event_list_p = el;
            m_event_count = el.size();
            m_trigger_type = trigger_t.AND_LIST_TIMEOUT;
        }

        //------------------------------------------------------------------------------
        //"sc_method_process::resume_process"
        //
        // This method resumes the execution of this process, and if requested, its
        // descendants. If the process was suspended and has a resumption pending it 
        // will be dispatched in the next delta cycle. Otherwise the state will be
        // adjusted to indicate it is no longer suspended, but no immediate execution
        // will occur.
        //------------------------------------------------------------------------------
        public override void resume_process()
        {
            resume_process(sc_descendant_inclusion_info.SC_NO_DESCENDANTS);
        }

        public override void resume_process(sc_descendant_inclusion_info descendants)
        {

            // IF NEEDED PROPOGATE THE RESUME REQUEST THROUGH OUR DESCENDANTS:

            if (descendants == sc_descendant_inclusion_info.SC_INCLUDE_DESCENDANTS)
            {
                List<sc_object> children = get_child_objects();
                int child_n = children.Count;

                for (int child_i = 0; child_i < child_n; child_i++)
                {
                    sc_process_b child_p = children[child_i] as sc_process_b;
                    if (child_p != null)
                        child_p.resume_process(descendants);
                }
            }


            // BY DEFAULT THE CORNER CASE IS AN ERROR:

            if (!sc_simcontext.sc_allow_process_control_corners && ((m_state & (int)process_state.ps_bit_disabled) != 0) && ((m_state & (int)process_state.ps_bit_suspended) != 0))
            {
                m_state = m_state & ~(int)process_state.ps_bit_suspended;
                report_error("Undefined process control interaction", "call to resume() on a disabled suspended method");
            }

            // CLEAR THE SUSPENDED BIT:

            m_state = m_state & ~(int)process_state.ps_bit_suspended;

            // RESUME OBJECT INSTANCE:
            //
            // If this is not a self-resume and the method is ready to run then
            // put it on the runnable queue.

            if ((m_state & (int)process_state.ps_bit_ready_to_run) != 0)
            {
                m_state = m_state & ~(int)process_state.ps_bit_ready_to_run;
                if (next_runnable() == null && (sc_simcontext.sc_get_current_process_b() != this as sc_process_b))
                {
                    simcontext().push_runnable_method(this);
                    remove_dynamic_events();
                }
            }
        }
        public void set_next_exist(sc_method_process next_p)
        {
            m_exist_p = next_p;
        }
        public void set_next_runnable(sc_method_process next_p)
        {
            m_runnable_p = next_p;
        }
        //------------------------------------------------------------------------------
        //"sc_method_process::suspend_process"
        //
        // This virtual method suspends this process and its children if requested to.
        //     descendants = indicator of whether this process' children should also
        //                   be suspended
        //------------------------------------------------------------------------------
        public override void suspend_process()
        {
            suspend_process(sc_descendant_inclusion_info.SC_NO_DESCENDANTS);
        }

        public override void suspend_process(sc_descendant_inclusion_info descendants)
        {

            // IF NEEDED PROPOGATE THE SUSPEND REQUEST THROUGH OUR DESCENDANTS:

            if (descendants == sc_descendant_inclusion_info.SC_INCLUDE_DESCENDANTS)
            {
                List<sc_object> children = get_child_objects();
                int child_n = children.Count;

                for (int child_i = 0; child_i < child_n; child_i++)
                {
                    sc_process_b child_p = children[child_i] as sc_process_b;
                    if (child_p != null)
                        child_p.suspend_process(descendants);
                }
            }

            // CORNER CASE CHECKS, THE FOLLOWING ARE ERRORS:
            //   (a) if this method has a reset_signal_is specification
            //   (b) if this method is in synchronous reset

            if (!sc_simcontext.sc_allow_process_control_corners && m_has_reset_signal)
            {
                report_error("Undefined process control interaction", "attempt to suspend a method that has a reset signal");
            }
            else if (!sc_simcontext.sc_allow_process_control_corners && m_sticky_reset)
            {
                report_error("Undefined process control interaction", "attempt to suspend a method in synchronous reset");
            }

            // SUSPEND OUR OBJECT INSTANCE:
            //
            // (1) If we are on the runnable queue then set suspended and ready_to_run,
            //     and remove ourselves from the run queue.
            // (2) If this is a self-suspension then a resume should cause immediate
            //     scheduling of the process.

            m_state = m_state | (int)process_state.ps_bit_suspended;
            if (next_runnable() != null)
            {
                m_state = m_state | (int)process_state.ps_bit_ready_to_run;
                simcontext().remove_runnable_method(this);
            }
            if (sc_simcontext.sc_get_current_process_b() == this as sc_process_b)
            {
                m_state = m_state | (int)process_state.ps_bit_ready_to_run;
            }
        }

        //------------------------------------------------------------------------------
        //"sc_method_process::throw_reset"
        //
        // This virtual method is invoked to "throw" a reset. 
        //
        // If the reset is synchronous this is a no-op, except for triggering the
        // reset event if it is present.
        //
        // If the reset is asynchronous we:
        //   (a) cancel any dynamic waits 
        //   (b) if it is the active process actually throw a reset exception.
        //   (c) if it was not the active process and does not have a static
        //       sensitivity emit an error if corner cases are to be considered
        //       errors.
        //
        // Notes:
        //   (1) If the process had a reset event it will have been triggered in 
        //       sc_process_b::semantics()
        //
        // Arguments:
        //   async = true if this is an asynchronous reset.
        //------------------------------------------------------------------------------
        //\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\
        /*
        public override void throw_reset(bool async)
        {
            // IF THE PROCESS IS CURRENTLY UNWINDING OR IS ALREADY A ZOMBIE
            // IGNORE THE RESET:

            if (m_unwinding)
            {
                //C++ TO C# CONVERTER TODO TASK: There is no direct equivalent in C# to the C++ __LINE__ macro:
                //C++ TO C# CONVERTER TODO TASK: There is no direct equivalent in C# to the C++ __FILE__ macro:
                global::sc_core.sc_report_handler.report(sc_core.sc_severity.SC_WARNING, SC_ID_PROCESS_ALREADY_UNWINDING_, name(), __FILE__, __LINE__);
                return;
            }

            if (m_state & process_state.ps_bit_zombie)
                return;

            // Set the throw status and if its an asynchronous reset throw an
            // exception:

            m_throw_status = async ? process_throw_type.THROW_ASYNC_RESET : process_throw_type.THROW_SYNC_RESET;
            if (async)
            {
                remove_dynamic_events();
                if (GlobalMembersSc_cor_fiber.sc_get_current_process_b() == this)
                {
                    m_throw_status = process_throw_type.THROW_ASYNC_RESET;
                    throw new sc_unwind_exception(this, true);
                }
                else
                {
                    simcontext().preempt_with(this);
                }
            }
        }
        */
        //\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\
        //---------------------------------------------------------------------------------


        //------------------------------------------------------------------------------
        //"sc_method_process::trigger_dynamic"
        //
        // This method sets up a dynamic trigger on an event.
        //
        // Notes:
        //   (1) This method is identical to sc_thread_process::trigger_dynamic(), 
        //       but they cannot be combined as sc_process_b::trigger_dynamic() 
        //       because the signatures things like sc_event::remove_dynamic()
        //       have different overloads for sc_method_process* and sc_thread_process*.
        //       So if you change code here you'll also need to change it in 
        //       sc_thread_process.cpp.
        //
        // Result is true if this process should be removed from the event's list,
        // false if not.
        //
        // If the triggering process is the same process, the trigger is
        // ignored as well, unless SC_ENABLE_IMMEDIATE_SELF_NOTIFICATIONS
        // is defined.
        //------------------------------------------------------------------------------
        public virtual bool trigger_dynamic(sc_event e)
        {
            // No time outs yet, and keep gcc happy.

            m_timed_out = false;

            // Escape cases:
            //   (a) If this method issued the notify() don't schedule it for
            //       execution, but leave the sensitivity in place.
            //   (b) If this method is already runnable can't trigger an event.


            if (is_runnable())
                return true;

            // If a process is disabled then we ignore any events, leaving them enabled:
            //
            // But if this is a time out event we need to remove both it and the
            // event that was being waited for.

            if ((m_state & (int)process_state.ps_bit_disabled) != 0)
            {
                if (e == m_timeout_event_p)
                {
                    remove_dynamic_events(true);
                    return true;
                }
                else
                {
                    return false;
                }
            }


            // Process based on the event type and current process state:
            //
            // Every case needs to set 'rc' and continue on to the end of
            // this method to allow suspend processing to work correctly.

            switch (m_trigger_type)
            {
                case trigger_t.EVENT:
                    m_event_p = null;
                    m_trigger_type = trigger_t.STATIC;
                    break;

                case trigger_t.AND_LIST:
                    --m_event_count;
                    if (m_event_count == 0)
                    {
                        m_event_list_p.auto_delete();
                        m_event_list_p = null;
                        m_trigger_type = trigger_t.STATIC;
                    }
                    else
                    {
                        return true;
                    }
                    break;

                case trigger_t.OR_LIST:
                    m_event_list_p.remove_dynamic(this, e);
                    m_event_list_p.auto_delete();
                    m_event_list_p = null;
                    m_trigger_type = trigger_t.STATIC;
                    break;

                case trigger_t.TIMEOUT:
                    m_trigger_type = trigger_t.STATIC;
                    break;

                case trigger_t.EVENT_TIMEOUT:
                    if (e == m_timeout_event_p)
                    {
                        m_timed_out = true;
                        m_event_p.remove_dynamic(this);
                        m_event_p = null;
                        m_trigger_type = trigger_t.STATIC;
                    }
                    else
                    {
                        m_timeout_event_p.cancel();
                        m_timeout_event_p.reset();
                        m_event_p = null;
                        m_trigger_type = trigger_t.STATIC;
                    }
                    break;

                case trigger_t.OR_LIST_TIMEOUT:
                    if (e == m_timeout_event_p)
                    {
                        m_timed_out = true;
                        m_event_list_p.remove_dynamic(this, e);
                        m_event_list_p.auto_delete();
                        m_event_list_p = null;
                        m_trigger_type = trigger_t.STATIC;
                    }

                    else
                    {
                        m_timeout_event_p.cancel();
                        m_timeout_event_p.reset();
                        m_event_list_p.remove_dynamic(this, e);
                        m_event_list_p.auto_delete();
                        m_event_list_p = null;
                        m_trigger_type = trigger_t.STATIC;
                    }
                    break;

                case trigger_t.AND_LIST_TIMEOUT:
                    if (e == m_timeout_event_p)
                    {
                        m_timed_out = true;
                        m_event_list_p.remove_dynamic(this, e);
                        m_event_list_p.auto_delete();
                        m_event_list_p = null;
                        m_trigger_type = trigger_t.STATIC;
                    }

                    else
                    {
                        --m_event_count;
                        if (m_event_count == 0)
                        {
                            m_timeout_event_p.cancel();
                            m_timeout_event_p.reset();
                            // no need to remove_dynamic
                            m_event_list_p.auto_delete();
                            m_event_list_p = null;
                            m_trigger_type = trigger_t.STATIC;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    break;

                case trigger_t.STATIC:
                    {
                        sc_report_handler.report(sc_core.sc_severity.SC_WARNING, "dynamic event notification encountered when sensitivity is static", name());
                        return true;
                    }
            }

            // If we get here then the method has satisfied its next_trigger, if its
            // suspended mark its state as ready to run. If its not suspended then push
            // it onto the runnable queue.

            if ((m_state & (int)process_state.ps_bit_suspended) != 0)
            {
                m_state = m_state | (int)process_state.ps_bit_ready_to_run;
            }
            else
            {
                simcontext().push_runnable_method(this);
            }

            return true;
        }

        //------------------------------------------------------------------------------
        //"sc_method_process::trigger_static"
        //
        // This inline method adds the current method to the queue of runnable
        // processes, if required.  This is the case if the following criteria
        // are met:
        //   (1) The process is in a runnable state.
        //   (2) The process is not already on the run queue.
        //   (3) The process is expecting a static trigger, 
        //       dynamic event waits take priority.
        //
        //
        // If the triggering process is the same process, the trigger is
        // ignored as well, unless SC_ENABLE_IMMEDIATE_SELF_NOTIFICATIONS
        // is defined.
        //------------------------------------------------------------------------------
        public virtual void trigger_static()
        {
            if (((m_state & (int)process_state.ps_bit_disabled) != 0) || is_runnable() || m_trigger_type != trigger_t.STATIC)
                return;


            // If we get here then the method is has satisfied its wait, if its
            // suspended mark its state as ready to run. If its not suspended then
            // push it onto the runnable queue.

            if ((m_state & (int)process_state.ps_bit_suspended) != 0)
            {
                m_state = m_state | (int)process_state.ps_bit_ready_to_run;
            }
            else
            {
                simcontext().push_runnable_method(this);
            }
        }

        protected sc_cor m_cor; // Thread's coroutine.
        protected int m_stack_size = 0; // Thread stack size.
        protected List<sc_process_monitor> m_monitor_q = new List<sc_process_monitor>(); // Thread monitors.

        // may not be deleted manually (called from sc_process_b)

        //------------------------------------------------------------------------------
        //"sc_method_process::sc_method_process"
        //
        // This is the object instance destructor for this class.
        //------------------------------------------------------------------------------
        public override void Dispose()
        {
            base.Dispose();
        }

        //C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
        //	sc_method_process(sc_method_process NamelessParameter);
        //C++ TO C# CONVERTER NOTE: This 'CopyFrom' method was converted from the original C++ copy assignment operator:
        //ORIGINAL LINE: const sc_method_process& operator = (const sc_method_process&);
        //C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
        //	sc_method_process CopyFrom (sc_method_process NamelessParameter);

    }


}
