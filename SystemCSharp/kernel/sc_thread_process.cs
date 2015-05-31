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


using System.Diagnostics;
using System.Collections.Generic;
using System;
namespace sc_core
{
    public class sc_thread_process : sc_process_b
    {
        public void set_stack_size(uint size)
        {
            Debug.Assert(size != 0);
            m_stack_size = size;
        }
        public void suspend_me()
        {
            // remember, if we're currently unwinding

            bool unwinding_preempted = m_unwinding;

            sc_simcontext simc_p = simcontext();
            sc_cor cor_p = simc_p.next_cor();

            // do not switch, if we're about to execute next (e.g. suicide)

            if (m_cor_p != cor_p)
            {
                simc_p.cor_pkg().yield(cor_p);
            }

            // IF THERE IS A THROW TO BE DONE FOR THIS PROCESS DO IT NOW:
            //
            // (1) Optimize THROW_NONE for speed as it is the normal case.
            // (2) If this thread is already unwinding then suspend_me() was
            //     called from the catch clause to throw an exception on another
            //     process, so just go back to the catch clause.

            if (m_throw_status == process_throw_type.THROW_NONE)
                return;

            if (m_unwinding)
                return;

            switch (m_throw_status)
            {
                case process_throw_type.THROW_ASYNC_RESET:
                case process_throw_type.THROW_SYNC_RESET:
                    if (m_reset_event_p != null)
                        m_reset_event_p.notify();
                    throw new sc_unwind_exception(this, true);

                case process_throw_type.THROW_USER:
                    m_throw_status = m_active_areset_n != 0 ? process_throw_type.THROW_ASYNC_RESET : process_throw_type.THROW_SYNC_RESET;
                    throw new sc_unwind_exception(this, true);
                    break;

                case process_throw_type.THROW_KILL:
                    throw new sc_unwind_exception(this, false);

                default: // THROWING_NOW
                    Debug.Assert(unwinding_preempted);
                    break;
            }
        }
        public void wait(sc_event e)
        {
            if (m_unwinding)
                sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "wait() not allowed during unwinding", name());

            m_event_p = e; // for cleanup.
            e.add_dynamic(this);
            m_trigger_type = trigger_t.EVENT;
            suspend_me();
        }
        public void wait(sc_event_or_list el)
        {
            if (m_unwinding)
                sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "wait() not allowed during unwinding", name());

            el.add_dynamic(this);
            m_event_list_p = el;
            m_trigger_type = trigger_t.OR_LIST;
            suspend_me();
        }
        public void wait(sc_event_and_list el)
        {
            if (m_unwinding)
                sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "wait() not allowed during unwinding", name());

            el.add_dynamic(this);
            m_event_list_p = el;
            m_event_count = el.size();
            m_trigger_type = trigger_t.AND_LIST;
            suspend_me();
        }
        public void wait(sc_time t)
        {
            if (m_unwinding)
                sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "wait() not allowed during unwinding", name());

            m_timeout_event_p.notify_internal(new sc_time(t));
            m_timeout_event_p.add_dynamic(this);
            m_trigger_type = trigger_t.TIMEOUT;
            suspend_me();
        }
        public void wait(sc_time t, sc_event e)
        {
            if (m_unwinding)
                sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "wait() not allowed during unwinding", name());

            m_timeout_event_p.notify_internal(new sc_time(t));
            m_timeout_event_p.add_dynamic(this);
            e.add_dynamic(this);
            m_event_p = e;
            m_trigger_type = trigger_t.EVENT_TIMEOUT;
            suspend_me();
        }
        public void wait(sc_time t, sc_event_or_list el)
        {
            if (m_unwinding)
                sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "wait() not allowed during unwinding", name());

            m_timeout_event_p.notify_internal(new sc_time(t));
            m_timeout_event_p.add_dynamic(this);
            el.add_dynamic(this);
            m_event_list_p = el;
            m_trigger_type = trigger_t.OR_LIST_TIMEOUT;
            suspend_me();
        }
        public void wait(sc_time t, sc_event_and_list el)
        {
            if (m_unwinding)
                sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "wait() not allowed during unwinding", name());


            m_timeout_event_p.notify_internal(new sc_time(t));
            m_timeout_event_p.add_dynamic(this);
            el.add_dynamic(this);
            m_event_list_p = el;
            m_event_count = el.size();
            m_trigger_type = trigger_t.AND_LIST_TIMEOUT;
            suspend_me();
        }
        public void wait_cycles(int n = 1)
        {
            if (m_unwinding)
                sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "wait() not allowed during unwinding", name());

            m_wait_cycle_n = n - 1;
            suspend_me();
        }
        public void add_monitor(sc_process_monitor monitor_p)
        {
            m_monitor_q.Add(monitor_p);
        }
        public void remove_monitor(sc_process_monitor monitor_p)
        {
            for (int mon_i = 0; mon_i < m_monitor_q.Count; )
            {
                if (m_monitor_q[mon_i] == monitor_p)
                {
                    m_monitor_q.RemoveAt(mon_i);
                }
                else
                {
                    mon_i++;
                }
            }
        }


        public void trigger_static()
        {
            // No need to try queueing this thread if one of the following is true:
            //    (a) its disabled
            //    (b) its already queued for execution
            //    (c) its waiting on a dynamic event
            //    (d) its wait count is not satisfied

            if (((m_state & (int)process_state.ps_bit_disabled) != 0) || is_runnable() || m_trigger_type != trigger_t.STATIC)
                return;


            if (m_wait_cycle_n > 0)
            {
                --m_wait_cycle_n;
                return;
            }

            // If we get here then the thread is has satisfied its wait criteria, if 
            // its suspended mark its state as ready to run. If its not suspended then 
            // push it onto the runnable queue.

            if ((m_state & (int)process_state.ps_bit_suspended) != 0)
            {
                m_state = m_state | (int)process_state.ps_bit_ready_to_run;
            }
            else
            {
                simcontext().push_runnable_thread(this);
            }
        }
        public override void disable_process(sc_descendant_inclusion_info descendants)
        {

            // IF NEEDED PROPOGATE THE DISABLE REQUEST THROUGH OUR DESCENDANTS:

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
                        report_error("Undefined process control interaction", "attempt to disable a thread with timeout wait");
                        break;
                    default:
                        break;
                }
            }

            // DISABLE OUR OBJECT INSTANCE:

            m_state = m_state | (int)process_state.ps_bit_disabled;

            // IF THIS CALL IS BEFORE THE SIMULATION DON'T RUN THE THREAD:

            if (!sc_simcontext.sc_is_running())
            {
                m_state = m_state | (int)process_state.ps_bit_ready_to_run;
                simcontext().remove_runnable_thread(this);
            }
        }
        public override void enable_process(sc_descendant_inclusion_info descendants)
        {

            // IF NEEDED PROPOGATE THE ENABLE REQUEST THROUGH OUR DESCENDANTS:

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
            if (m_state == (int)process_state.ps_bit_ready_to_run && sc_simcontext.sc_allow_process_control_corners)
            {
                m_state = (int)process_state.ps_normal;
                if (next_runnable() == null)
                    simcontext().push_runnable_thread(this);
            }
        }
        public override void kill_process(sc_descendant_inclusion_info descendants)
        {

            // IF THE SIMULATION HAS NOT BEEN INITIALIZED YET THAT IS AN ERROR:

            if (!sc_simcontext.sc_is_running())
            {
                report_error("a process may not be killed before it is initialized");
            }

            // IF NEEDED PROPOGATE THE KILL REQUEST THROUGH OUR DESCENDANTS:

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
                //C++ TO C# CONVERTER TODO TASK: There is no direct equivalent in C# to the C++ __LINE__ macro:
                //C++ TO C# CONVERTER TODO TASK: There is no direct equivalent in C# to the C++ __FILE__ macro:
                sc_report_handler.report(sc_core.sc_severity.SC_WARNING, "kill/reset ignored during unwinding", name());
                return;
            }

            if ((m_state & (int)process_state.ps_bit_zombie) != 0)
                return;

            // SET UP TO KILL THE PROCESS IF SIMULATION HAS STARTED:
            //
            // If the thread does not have a stack don't try the throw!

            if (sc_simcontext.sc_is_running() && m_has_stack)
            {
                m_throw_status = process_throw_type.THROW_KILL;
                m_wait_cycle_n = 0;
                simcontext().preempt_with(this);
            }

            // IF THE SIMULATION HAS NOT STARTED REMOVE TRACES OF OUR PROCESS FROM 
            // EVENT QUEUES, ETC.:

            else
            {
                disconnect_process();
            }
        }
        public void prepare_for_simulation()
        {
            m_cor_p = simcontext().cor_pkg().create(m_stack_size, sc_thread_process.sc_thread_cor_fn, this, this);
            m_cor_p.stack_protect(true);
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
                report_error("attempt to disable a thread with timeout wait", "call to resume() on a disabled suspended thread");
            }

            // CLEAR THE SUSPENDED BIT:

            m_state = m_state & ~(int)process_state.ps_bit_suspended;

            // RESUME OBJECT INSTANCE IF IT IS READY TO RUN:

            if ((m_state & (int)process_state.ps_bit_ready_to_run) != 0)
            {
                m_state = m_state & ~(int)process_state.ps_bit_ready_to_run;
                if (next_runnable() == null)
                    simcontext().push_runnable_thread(this);
                remove_dynamic_events(); // order important.
            }
        }
        public override void Dispose()
        {

            // DESTROY THE COROUTINE FOR THIS THREAD:

            if (m_cor_p != null)
            {
                m_cor_p.stack_protect(false);
                m_cor_p = null;
            }

            base.Dispose();
        }

        public void signal_monitors(int type)
        {
            int mon_n; // # of monitors present.

            mon_n = m_monitor_q.Count;
            for (int mon_i = 0; mon_i < mon_n; mon_i++)
                m_monitor_q[mon_i].signal(this, type);
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
            //   (a) if this thread has a reset_signal_is specification 
            //   (b) if this thread is in synchronous reset

            if (!sc_simcontext.sc_allow_process_control_corners && m_has_reset_signal)
            {
                report_error("Undefined process control interaction", "attempt to suspend a thread that has a reset signal");
            }
            else if (!sc_simcontext.sc_allow_process_control_corners && m_sticky_reset)
            {
                report_error("Undefined process control interaction", "attempt to suspend a thread in synchronous reset");
            }

            // SUSPEND OUR OBJECT INSTANCE:
            //
            // (1) If we are on the runnable queue then set suspended and ready_to_run,
            //     and remove ourselves from the run queue.
            // (2) If this is a self-suspension then a resume should cause immediate
            //     scheduling of the process, and we need to call suspend_me() here.

            m_state = m_state | (int)process_state.ps_bit_suspended;
            if (next_runnable() != null)
            {
                m_state = m_state | (int)process_state.ps_bit_ready_to_run;
                simcontext().remove_runnable_thread(this);
            }
            if (sc_simcontext.sc_get_current_process_b() == this as sc_process_b)
            {
                m_state = m_state | (int)process_state.ps_bit_ready_to_run;
                suspend_me();
            }
        }

        //-----------------------------------------------------------------------------
        //\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\
        /*
        public void throw_reset(bool async)
        {
            // IF THE PROCESS IS CURRENTLY UNWINDING OR IS ALREADY A ZOMBIE
            // IGNORE THE RESET:

            if (m_unwinding)
            {
                sc_report_handler.report(sc_core.sc_severity.SC_WARNING, "kill/reset ignored during unwinding", name());
                return;
            }

            if (m_state & process_state.ps_bit_zombie)
                return;


            // Set the throw type and clear any pending dynamic events: 

            m_throw_status = async ? process_throw_type.THROW_ASYNC_RESET : process_throw_type.THROW_SYNC_RESET;
            m_wait_cycle_n = 0;

            // If this is an asynchronous reset:
            //
            //   (a) Cancel any dynamic events 
            //   (b) Set the thread up for execution:
            //         (i) If we are in the execution phase do it now.
            //         (ii) If we are not queue it to execute next when we hit
            //              the execution phase.

            if (async)
            {
                m_state = m_state & ~process_state.ps_bit_ready_to_run;
                remove_dynamic_events();
                if (simcontext().evaluation_phase())
                {
                    simcontext().preempt_with(this);
                }
                else
                {
                    if (is_runnable())
                        simcontext().remove_runnable_thread(this);
                    simcontext().execute_thread_next(this);
                }
            }
        }
        public void throw_user(sc_throw_it_helper helper, sc_descendant_inclusion_info descendants)
        {

            // IF THE SIMULATION IS NOT ACTAULLY RUNNING THIS IS AN ERROR:

            if (sc_simcontext.sc_get_status() != sc_status.SC_RUNNING)
            {
                report_error(SC_ID_THROW_IT_WHILE_NOT_RUNNING_);
            }

            // IF NEEDED PROPOGATE THE THROW REQUEST THROUGH OUR DESCENDANTS:

            if (descendants == sc_descendant_inclusion_info.SC_INCLUDE_DESCENDANTS)
            {
                const List<sc_object> children = get_child_objects();
                int child_n = children.Count;

                for (int child_i = 0; child_i < child_n; child_i++)
                {
                    sc_process_b child_p = children[child_i] as sc_process_b;
                    if (child_p != null)
                    {
                        child_p.throw_user(new sc_throw_it_helper(helper), descendants);
                    }
                }
            }

            // IF THE PROCESS IS CURRENTLY UNWINDING IGNORE THE THROW:

            if (m_unwinding)
            {
                sc_report_handler.report(sc_core.sc_severity.SC_WARNING, "kill/reset ignored during unwinding", name());
                return;
            }

            // SET UP THE THROW REQUEST FOR THIS OBJECT INSTANCE AND QUEUE IT FOR
            // EXECUTION:

            if (m_has_stack)
            {
                remove_dynamic_events();
                m_throw_status = process_throw_type.THROW_USER;
                if (m_throw_helper_p != 0)
                    m_throw_helper_p = null;
                m_throw_helper_p = helper.clone();
                simcontext().preempt_with(this);
            }
            else
            {
                sc_report_handler.report(sc_core.sc_severity.SC_WARNING, "throw_it on method/non-running process is being ignored ", name());
            }
        }
        */
        //\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\
        //-----------------------------------------------------------------------------
        

        public bool trigger_dynamic(sc_event e)
        {
            // No time outs yet, and keep gcc happy.

            m_timed_out = false;

            // Escape cases:
            //   (a) If this thread issued the notify() don't schedule it for
            //       execution, but leave the sensitivity in place.
            //   (b) If this thread is already runnable can't trigger an event.

            // not possible for thread processes!

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

            // If we get here then the thread is has satisfied its wait criteria, if 
            // its suspended mark its state as ready to run. If its not suspended then 
            // push it onto the runnable queue.

            if ((m_state & (int)process_state.ps_bit_suspended) != 0)
            {
                m_state = m_state | (int)process_state.ps_bit_ready_to_run;
            }
            else
            {
                simcontext().push_runnable_thread(this);
            }

            return true;
        }
        public void set_next_exist(sc_thread_process next_p)
        {
            m_exist_p = next_p;
        }
        public void set_next_runnable(sc_thread_process next_p)
        {
            m_runnable_p = next_p;
        }


        public static void sc_thread_cor_fn(object arg)
        {
            sc_simcontext simc_p = sc_simcontext.sc_get_curr_simcontext();
            sc_thread_process thread_h = (arg) as sc_thread_process;

            // PROCESS THE THREAD AND PROCESS ANY EXCEPTIONS THAT ARE THROWN:

            while (true)
            {

                //try
                //{
                    thread_h.semantics();
                //}
                //-----------------------------------------------------------------------------
                //\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\
                /*
                catch (sc_user)
                {
                    continue;
                }
                catch (sc_halt)
                {
                    Console.Writeline("Terminating process " + thread_h.name());
                }
                catch (sc_unwind_exception ex)
                {
                    ex.clear();
                    if (ex.is_reset())
                        continue;
                }
                catch
                {
                    sc_report err_p = sc_simcontext.sc_handle_exception();
                    thread_h.simcontext().set_error(err_p);
                }
                */
                //\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\
                //-----------------------------------------------------------------------------
                //catch (Exception ex)
                //{
                //    Console.WriteLine("Terminating process " + thread_h == null ? "" : thread_h.name());
                //}
                break;
            }

            sc_process_b active_p = sc_simcontext.sc_get_current_process_b();
            // REMOVE ALL TRACES OF OUR THREAD FROM THE SIMULATORS DATA STRUCTURES:

            thread_h.disconnect_process();

            // IF WE AREN'T ACTIVE MAKE SURE WE WON'T EXECUTE:

            if (thread_h.next_runnable() != null)
            {
                simc_p.remove_runnable_thread(thread_h);
            }

            // IF WE ARE THE ACTIVE PROCESS ABORT OUR EXECUTION:


            if (active_p == (sc_process_b)thread_h)
            {

                sc_core.sc_cor x = simc_p.next_cor();
                simc_p.cor_pkg().abort(x);
            }

        }

        public static void sc_set_stack_size(sc_thread_process thread_h, uint size)
        {
            thread_h.set_stack_size(size);
        }

        public static sc_cor get_cor_pointer(sc_process_b process_p)
        {
            sc_thread_process thread_p = process_p as sc_thread_process;
            return thread_p.m_cor_p;
        }

        public sc_thread_process(string name_p, bool free_host, sc_process_call_base method_p, sc_process_host host_p, sc_spawn_options opt_p)
            : base((string.IsNullOrEmpty(name_p) == false) ? name_p : sc_simcontext.sc_gen_unique_name("thread_p"), true, free_host, method_p, host_p, opt_p)
        {
            m_cor_p = null;
            m_monitor_q = new List<sc_process_monitor>();
            m_stack_size = 0x50000;
            m_wait_cycle_n = 0;

            if ((sc_module)(host_p) != null && sc_simcontext.sc_is_running())
            {
                report_error("call to SC_THREAD in sc_module while simulation running");
            }

            // INITIALIZE VALUES:
            //
            // If there are spawn options use them.

            m_process_kind = sc_curr_proc_kind.SC_THREAD_PROC_;

            if (opt_p != null)
            {
                m_dont_init = opt_p.m_dont_initialize;
                if (opt_p.m_stack_size != 0)
                    m_stack_size = opt_p.m_stack_size;

                //-----------------------------------------------------------------------------
                //\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\
                /*
                // traverse event sensitivity list
                for (uint i = 0; i < opt_p.m_sensitive_events.size(); i++)
                {
                    sc_sensitive.make_static_sensitivity(this, opt_p.m_sensitive_events[i]);
                }

                // traverse port base sensitivity list
                for (uint i = 0; i < opt_p.m_sensitive_port_bases.size(); i++)
                {
                    sc_sensitive.make_static_sensitivity(this, opt_p.m_sensitive_port_bases[i]);
                }

                // traverse interface sensitivity list
                for (uint i = 0; i < opt_p.m_sensitive_interfaces.size(); i++)
                {
                    sc_sensitive.make_static_sensitivity(this, opt_p.m_sensitive_interfaces[i]);
                }

                // traverse event finder sensitivity list
                for (uint i = 0; i < opt_p.m_sensitive_event_finders.size(); i++)
                {
                    sc_sensitive.make_static_sensitivity(this, opt_p.m_sensitive_event_finders[i]);
                }

                // process any reset signal specification:

                opt_p.specify_resets();
                */
                //\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\
                //-----------------------------------------------------------------------------
            }

            else
            {
                m_dont_init = false;
            }
        }


        public override string kind()
        {
            return "sc_thread_process";
        }

        sc_thread_process m_exist_p;
        sc_thread_process m_runnable_p;

        public virtual sc_thread_process next_exist()
        {
            return m_exist_p;
        }

        public virtual sc_thread_process next_runnable()
        {
            return m_runnable_p;
        }

        public virtual sc_cor get_cor()
        {
            return m_cor_p;
        }

        sc_cor m_cor_p; // Thread's coroutine.
        public List<sc_process_monitor> m_monitor_q = new List<sc_process_monitor>(); // Thread monitors.
        uint m_stack_size = 0; // Thread stack size.
        protected int m_wait_cycle_n; // # of waits to be done.



        public override void enable_process()
        {
            throw new System.NotImplementedException();
        }

        public override void kill_process()
        {
            throw new System.NotImplementedException();
        }

        public override void resume_process()
        {
            throw new System.NotImplementedException();
        }

        public override void suspend_process()
        {
            throw new System.NotImplementedException();
        }
    }
}
