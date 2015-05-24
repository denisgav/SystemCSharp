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


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
namespace sc_core
{

    public class sc_curr_proc_info
    {
        public sc_process_b process_handle;
        public sc_curr_proc_kind kind;
        public sc_curr_proc_info()
        {
            process_handle = null;
            kind = sc_curr_proc_kind.SC_NO_PROC_;
        }
    }

    public enum sc_stop_mode // sc_stop modes:
    {
        SC_STOP_FINISH_DELTA,
        SC_STOP_IMMEDIATE
    }

    public enum sc_starvation_policy
    {
        SC_EXIT_ON_STARVATION,
        SC_RUN_TO_TIME
    }

    // ----------------------------------------------------------------------------
    //  CLASS : sc_simcontext
    //
    //  The simulation context.
    // ----------------------------------------------------------------------------

    public class sc_simcontext : IDisposable
    {

        public static string sc_get_current_process_name()
        {
            sc_process_b active_p;
            string result; // name of active process.

            active_p = sc_get_curr_simcontext().get_curr_proc_info().process_handle;
            if (active_p != null)
                result = active_p.name();
            else
                result = "** NONE **";
            return result;
        }


        public void set_curr_proc(sc_process_b process_h)
        {
            m_curr_proc_info.process_handle = process_h;
            m_curr_proc_info.kind = process_h.proc_kind();
            m_current_writer = m_write_check ? process_h : null;
        }
        public void execute_method_next(sc_method_process method_h)
        {
            m_runnable.execute_method_next(method_h);
        }
        public void execute_thread_next(sc_thread_process thread_h)
        {
            m_runnable.execute_thread_next(thread_h);
        }
        public void preempt_with(sc_thread_process thread_h)
        {
            sc_thread_process active_p;
            sc_curr_proc_info caller_info = new sc_curr_proc_info(); // process info for caller.

            // Determine the active process and take the thread to be run off the
            // run queue, if its there, since we will be explicitly causing its 
            // execution.

            active_p = sc_get_current_process_b() as sc_thread_process;
            if (thread_h.next_runnable() != null)
                remove_runnable_thread(thread_h);

            // THE CALLER IS A METHOD:
            //
            //   (a) Set the current process information to our thread.
            //   (b) If the method was called by an invoker thread push that thread
            //       onto the front of the run queue, this will cause the method
            //       to be resumed after this thread waits.
            //   (c) Invoke our thread directly by-passing the run queue.
            //   (d) Restore the process info to the caller.
            //   (e) Check to see if the calling method should throw an exception
            //       because of activity that occurred during the preemption.

            if (active_p == null)
            {
                Stack<sc_thread_process> invokers_p;
                sc_thread_process invoke_thread_p;
                sc_method_process method_p;
                method_p = sc_get_current_process_b() as sc_method_process;

                invokers_p = get_active_invokers();
                caller_info = m_curr_proc_info;
                if (invokers_p.Count != 0)
                {
                    invoke_thread_p = invokers_p.Peek();
                    execute_thread_next(invoke_thread_p);
                }
                set_curr_proc((sc_process_b)thread_h);
                m_cor_pkg.yield(thread_h.get_cor());
                m_curr_proc_info = caller_info;
                method_p.check_for_throws();
            }

            // CALLER IS A THREAD, BUT NOT THE THREAD TO BE RUN:
            //
            //   (a) Push the calling thread onto the front of the runnable queue
            //       so it be the first thread to be run after this thread.
            //   (b) Push the thread to be run onto the front of the runnable queue so 
            //       it will execute when we suspend the calling thread.
            //   (c) Suspend the active thread.

            else if (active_p != thread_h)
            {
                execute_thread_next(active_p);
                execute_thread_next(thread_h);
                active_p.suspend_me();
            }

            // CALLER IS THE THREAD TO BE RUN:
            //
            //   (a) Push the thread to be run onto the front of the runnable queue so 
            //       it will execute when we suspend the calling thread.
            //   (b) Suspend the active thread.

            else
            {
                execute_thread_next(thread_h);
                active_p.suspend_me();
            }
        }
        public void push_runnable_method(sc_method_process method_h)
        {
            m_runnable.push_back_method(method_h);
        }
        public void push_runnable_method_front(sc_method_process method_h)
        {
            m_runnable.push_front_method(method_h);
        }
        public void push_runnable_thread(sc_thread_process thread_h)
        {
            m_runnable.push_back_thread(thread_h);
        }
        public void push_runnable_thread_front(sc_thread_process thread_h)
        {
            m_runnable.push_front_thread(thread_h);
        }
        public void remove_runnable_method(sc_method_process method_h)
        {
            m_runnable.remove_method(method_h);
        }
        public void remove_runnable_thread(sc_thread_process thread_h)
        {
            m_runnable.remove_thread(thread_h);
        }
        public Stack<sc_thread_process> get_active_invokers()
        {
            return m_active_invokers;
        }

        private void init()
        {

            // ALLOCATE VARIOUS MANAGERS AND REGISTRIES:

            m_object_manager = new sc_object_manager();
            m_module_registry = new sc_module_registry(this);
            //---------------------------------------------------------------------------------------
            //m_port_registry = new sc_port_registry(this);
            //m_export_registry = new sc_export_registry(this);
            //m_prim_channel_registry = new sc_prim_channel_registry(this);
            //---------------------------------------------------------------------------------------
            m_phase_cb_registry = new sc_phase_callback_registry(this);
            m_name_gen = new sc_name_gen();
            m_process_table = new sc_process_table();
            m_current_writer = null;


            // CHECK FOR ENVIRONMENT VARIABLES THAT MODIFY SIMULATOR EXECUTION:

            m_write_check = true;


            // FINISH INITIALIZATIONS:

            reset_curr_proc();
            m_next_proc_id = -1;
            m_timed_events = new List<sc_event_timed>();
            m_something_to_trace = false;
            m_runnable = new sc_runnable();
            m_collectable = new List<sc_process_b>();
            m_time_params = new sc_time_params();
            m_curr_time = sc_time.SC_ZERO_TIME;
            m_max_time = sc_time.SC_ZERO_TIME;
            m_change_stamp = 0;
            m_delta_count = 0;
            m_forced_stop = false;
            m_paused = false;
            m_ready_to_simulate = false;
            m_elaboration_done = false;
            m_execution_phase = execution_phases.phase_initialize;
            m_error = null;
            m_cor_pkg = null;
            m_method_invoker_p = null;
            m_cor = null;
            m_in_simulator_control = false;
            m_start_of_simulation_called = false;
            m_end_of_simulation_called = false;
            m_simulation_status = sc_status.SC_ELABORATION;
        }
        private void clean()
        {
            //-----------------------------------------------------------------------
            /*
            if (m_object_manager != null)
                m_object_manager.Dispose();
            if (m_module_registry != null)
                m_module_registry.Dispose();
            if (m_port_registry != null)
                m_port_registry.Dispose();
            if (m_export_registry != null)
                m_export_registry.Dispose();
            if (m_prim_channel_registry != null)
                m_prim_channel_registry.Dispose();
            if (m_phase_cb_registry != null)
                m_phase_cb_registry.Dispose();
            if (m_name_gen != null)
                m_name_gen.Dispose();
            if (m_process_table != null)
                m_process_table.Dispose();
            m_child_objects.resize(0);
            m_delta_events.resize(0);
            if (m_timed_events != null)
                m_timed_events.Dispose();
            for (int i = m_trace_files.Count - 1; i >= 0; --i)
            {
                if (m_trace_files[i] != null)
                    m_trace_files[i].Dispose();
            }
            m_trace_files.resize(0);
            if (m_runnable != null)
                m_runnable.Dispose();
            if (m_collectable != null)
                m_collectable.Dispose();
            if (m_time_params != null)
                m_time_params.Dispose();
            if (m_cor_pkg != null)
                m_cor_pkg.Dispose();
            if (m_error != null)
                m_error.Dispose();
            */
            //-----------------------------------------------------------------------
        }


        public sc_simcontext()
        {
            m_object_manager = null;
            m_module_registry = null;
            //-----------------------------------------------------------------------
            /*
            m_port_registry = 0;
            m_export_registry = 0;
            m_prim_channel_registry = 0;
            */
            //-----------------------------------------------------------------------
            m_phase_cb_registry = null;
            m_name_gen = null;
            m_process_table = null;
            m_curr_proc_info = new sc_curr_proc_info();
            m_current_writer = null;
            m_write_check = false;
            m_next_proc_id = -1;
            m_child_events = new List<sc_event>();
            m_child_objects = new List<sc_object>();
            m_delta_events = new List<sc_event>();
            m_timed_events = null;
            //m_trace_files = new List();
            m_something_to_trace = false;
            m_runnable = null;
            m_collectable = null;
            m_time_params = new sc_time_params();
            m_curr_time = sc_time.SC_ZERO_TIME;
            m_max_time = sc_time.SC_ZERO_TIME;
            m_change_stamp = 0;
            m_delta_count = 0;
            m_forced_stop = false;
            m_paused = false;
            m_ready_to_simulate = false;
            m_elaboration_done = false;
            m_execution_phase = execution_phases.phase_initialize;
            m_error = null;
            m_in_simulator_control = false;
            m_end_of_simulation_called = false;
            m_simulation_status = sc_status.SC_ELABORATION;
            m_start_of_simulation_called = false;
            m_cor_pkg = null;
            m_cor = null;
            init();
        }
        public virtual void Dispose()
        {
            clean();
        }

        public void initialize()
        {
            initialize(false);
        }

        public void initialize(bool no_crunch)
        {
            m_in_simulator_control = true;
            elaborate();

            prepare_to_simulate();
            initial_crunch(no_crunch);
            m_in_simulator_control = false;
        }
        public void cycle(sc_time t)
        {
            sc_time next_event_time = new sc_time();

            m_in_simulator_control = true;
            crunch();


            if (m_something_to_trace)
            {
                trace_cycle(false); // delta cycle?
            }

            m_curr_time += t;
            if (next_time(out next_event_time) && next_event_time <= m_curr_time)
            {
                sc_report_handler.report(sc_core.sc_severity.SC_WARNING, "the simulation contains timed-events but they are ignored by sc_cycle() ==> the simulation will be incorrect", "");
            }
            m_in_simulator_control = false;

        }

        // +----------------------------------------------------------------------------
        // |"sc_simcontext::simulate"
        // | 
        // | This method runs the simulation for the specified amount of time.
        // |
        // | Notes:
        // |   (1) This code always run with an SC_EXIT_ON_STARVATION starvation policy,
        // |       so the simulation time on return will be the minimum of the 
        // |       simulation on entry plus the duration, and the maximum time of any 
        // |       event present in the simulation. If the simulation policy is
        // |       SC_RUN_TO_TIME starvation it is implemented by the caller of this 
        // |       method, e.g., sc_start(), by artificially setting the simulation
        // |       time forward after this method completes.
        // |
        // | Arguments:
        // |     duration = amount of time to simulate.
        // +----------------------------------------------------------------------------
        public void simulate(sc_time duration)
        {
            initialize(true);

            if (sim_status() != sc_simcontext.SC_SIM_OK)
            {
                return;
            }

            sc_time non_overflow_time = sc_simcontext.sc_max_time() - m_curr_time;
            if (duration > non_overflow_time)
            {
                sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "simulation time value overflow, simulation aborted", "");
                return;
            }
            else if (duration < sc_time.SC_ZERO_TIME)
            {
                sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "negative simulation interval specified in sc_start call", "");
            }

            m_in_simulator_control = true;
            m_paused = false;

            sc_time until_t = m_curr_time + duration;
            sc_time t = new sc_time(); // current simulaton time.

            // IF DURATION WAS ZERO WE ONLY CRUNCH ONCE:
            //
            // We duplicate the code so that we don't add the overhead of the
            // check to each loop in the do below.
            if (duration == sc_time.SC_ZERO_TIME)
            {
                m_in_simulator_control = true;
                crunch(true);
                if (m_error != null)
                {
                    m_in_simulator_control = false;
                    return;
                }
                if (m_something_to_trace)
                    trace_cycle(false); // delta cycle?
                if (m_forced_stop)
                {
                    do_sc_stop_action();
                    return;
                }
                // return via implicit pause
                goto exit_pause;
            }

            // NON-ZERO DURATION: EXECUTE UP TO THAT TIME, OR UNTIL EVENT STARVATION:

            do
            {

                crunch();
                if (m_error != null)
                {
                    m_in_simulator_control = false;
                    return;
                }
                if (m_something_to_trace)
                {
                    trace_cycle(false);
                }
                // check for call(s) to sc_stop() or sc_pause().
                if (m_forced_stop)
                {
                    do_sc_stop_action();
                    return;
                }
                if (m_paused) // return explicit pause
                    goto exit_pause;

                t = m_curr_time;

                do
                {
                    // See note 1 above:

                    if (!next_time(out t) || (t > until_t))
                        goto exit_time;
                    if (t > m_curr_time)
                    {

                        m_curr_time = t;

                        m_change_stamp++;
                    }

                    // PROCESS TIMED NOTIFICATIONS AT THE CURRENT TIME

                    do
                    {
                        sc_event_timed et = m_timed_events[m_timed_events.Count - 1];
                        sc_event e = et.Event();
                        /*
                        if (et != null)
                            et.Dispose();
                        */
                        if (e != null)
                        {
                            e.trigger();
                        }
                    } while ((m_timed_events.Count != 0) && m_timed_events[m_timed_events.Count - 1].notify_time() == t);

                } while (m_runnable.IsEmpty);
            } while (t < until_t); // hold off on the delta for the until_t time.

        exit_time: // final simulation time update, if needed
            if ((t != null) && (t > m_curr_time && t <= until_t))
            {
                m_curr_time = t;
                m_change_stamp++;
            }
        exit_pause: // call pause callback upon implicit or explicit pause
            m_execution_phase = execution_phases.phase_evaluate;
            m_in_simulator_control = false;


        }

        //------------------------------------------------------------------------------
        //"sc_simcontext::stop"
        //
        // This method stops the simulator after some amount of further processing.
        // How much processing is done depends upon the value of the global variable
        // stop_mode:
        //     SC_STOP_IMMEDIATE - aborts the execution phase of the current delta
        //                         cycle and performs whatever updates are pending.
        //     SC_STOP_FINISH_DELTA - finishes the current delta cycle - both execution
        //                            and updates.
        // If sc_stop is called outside of the purview of the simulator kernel 
        // (e.g., directly from sc_main), the end of simulation notifications 
        // are performed. From within the purview of the simulator kernel, these
        // will be performed at a later time.
        //------------------------------------------------------------------------------

        public void stop()
        {
            if (m_forced_stop)
            {

                //sc_report_handler.report(sc_core.sc_severity.SC_WARNING, "sc_stop has already been called", "");
                return;
            }
            if (sc_simcontext.stop_mode == sc_stop_mode.SC_STOP_IMMEDIATE)
                m_runnable.Init();
            m_forced_stop = true;
            if (!m_in_simulator_control)
            {
                do_sc_stop_action();
            }
        }
        public void end()
        {
            m_simulation_status = sc_status.SC_END_OF_SIMULATION;
            m_ready_to_simulate = false;
            //------------------------------------------------------------------
            // \/\/\/\/\\\/\/\\/\/\/\\/\/\/\/\/\/\/\/\/\/\\/\/\/\/\/\/\\/\/\/\/\
            /*
            m_port_registry.simulation_done();
            m_export_registry.simulation_done();
            m_prim_channel_registry.simulation_done();
            */
            // \/\/\/\/\\\/\/\\/\/\/\\/\/\/\/\/\/\/\/\/\/\\/\/\/\/\/\/\\/\/\/\/\
            //------------------------------------------------------------------
            m_module_registry.simulation_done();

            m_end_of_simulation_called = true;
        }
        public void reset()
        {
            clean();
            init();
        }

        public int sim_status()
        {
            if (m_error != null)
            {
                return SC_SIM_ERROR;
            }
            if (m_forced_stop)
            {
                return SC_SIM_USER_STOP;
            }
            return SC_SIM_OK;
        }

        // IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIII

        public bool elaboration_done()
        {
            return m_elaboration_done;
        }

        public sc_object_manager get_object_manager()
        {
            return m_object_manager;
        }

        public sc_status get_status()
        {
            return m_simulation_status != sc_status.SC_RUNNING ?
                  m_simulation_status :
                    (m_in_simulator_control ? sc_status.SC_RUNNING : sc_status.SC_PAUSED);
        }


        // +----------------------------------------------------------------------------
        // |"sc_simcontext::active_object"
        // | 
        // | This method returns the currently active object with respect to 
        // | additions to the hierarchy. It will be the top of the object hierarchy
        // | stack if it is non-empty, or it will be the active process, or NULL 
        // | if there is no active process.
        // +----------------------------------------------------------------------------
        public sc_object active_object()
        {
            sc_object result_p; // pointer to return.

            result_p = m_object_manager.hierarchy_curr();
            if (result_p == null)
                result_p = (sc_object)get_curr_proc_info().process_handle;
            return result_p;
        }

        public void hierarchy_push(sc_module mod)
        {
            m_object_manager.hierarchy_push(mod);
        }
        public sc_module hierarchy_pop()
        {
            return (sc_module)(m_object_manager.hierarchy_pop());
        }

        public sc_module hierarchy_curr()
        {
            return (sc_module)(m_object_manager.hierarchy_curr());
        }
        public System.Collections.Generic.IEnumerable<sc_object> get_objects()
        {
            return m_object_manager.get_objects();
        }

        public System.Collections.Generic.IEnumerable<sc_event> get_events()
        {
            return m_object_manager.get_events();
        }

        public sc_object find_object(string name)
        {
            sc_report_handler.report(sc_core.sc_severity.SC_INFO, "/IEEE_Std_1666/deprecated", "sc_simcontext::find_object() is deprecated,\n use sc_find_object()");
            return m_object_manager.find_object(name);
        }

        public sc_module_registry get_module_registry()
        {
            return m_module_registry;
        }
        //---------------------------------------------------------------
        // \/\/\/\/\\\/\/\\/\/\/\\/\/\/\/\/\/\/\/\/\/\\/\/\/\/\/\/\\/\/\/\/\
        /*
        public sc_port_registry get_port_registry()
        {
            return m_port_registry;
        }
        public sc_export_registry get_export_registry()
        {
            return m_export_registry;
        }
        public sc_prim_channel_registry get_prim_channel_registry()
        {
            return m_prim_channel_registry;
        }
        */
        // \/\/\/\/\\\/\/\\/\/\/\\/\/\/\/\/\/\/\/\/\/\\/\/\/\/\/\/\\/\/\/\/\
        //------------------------------------------------------------------

        // to generate unique names for objects in an MT-Safe way

        // to generate unique names for objects in an MT-Safe way

        public string gen_unique_name(string basename_)
        {
            return gen_unique_name(basename_, false);
        }


        public string gen_unique_name(string basename_, bool preserve_first)
        {
            return m_name_gen.gen_unique_name(basename_, preserve_first);
        }

        public sc_process_handle create_cthread_process(string name_p, bool free_host, sc_process_call_base method_p, sc_process_host host_p, sc_spawn_options opt_p)
        {
            sc_thread_process handle = new sc_cthread_process(name_p, free_host, method_p, host_p, opt_p);
            if (m_ready_to_simulate)
            {
                handle.prepare_for_simulation();
            }
            else
            {
                m_process_table.push_front(handle);
            }
            return new sc_process_handle(handle);
        }

        public sc_process_handle create_method_process(string name_p, bool free_host, sc_process_call_base method_p, sc_process_host host_p, sc_spawn_options opt_p)
        {
            sc_method_process handle = new sc_method_process(name_p, free_host, method_p, host_p, opt_p);
            if (m_ready_to_simulate) // dynamic process
            {
                if (!handle.dont_initialize())
                {
                    {
                        push_runnable_method(handle);
                    }
                }
                else if (handle.m_static_events.Count == 0)
                {
                    sc_report_handler.report(sc_core.sc_severity.SC_WARNING, "disable() or dont_initialize() called on process with no static sensitivity, it will be orphaned", handle.name());
                }

            }
            else
            {
                m_process_table.push_front(handle);
            }
            return new sc_process_handle(handle);
        }

        public sc_process_handle create_thread_process(string name_p, bool free_host, sc_process_call_base method_p, sc_process_host host_p, sc_spawn_options opt_p)
        {
            sc_thread_process handle = new sc_thread_process(name_p, free_host, method_p, host_p, opt_p);
            if (m_ready_to_simulate) // dynamic process
            {
                handle.prepare_for_simulation();
                if (!handle.dont_initialize())
                {
                    {
                        push_runnable_thread(handle);
                    }
                }
                else if (handle.m_static_events.Count == 0)
                {
                    sc_report_handler.report(sc_core.sc_severity.SC_WARNING, "disable() or dont_initialize() called on process with no static sensitivity, it will be orphaned", handle.name());
                }

            }
            else
            {
                m_process_table.push_front(handle);
            }
            return new sc_process_handle(handle);
        }


        public sc_curr_proc_info get_curr_proc_info()
        {
            return m_curr_proc_info;
        }

        public sc_object get_current_writer()
        {
            return m_current_writer;
        }

        public bool write_check()
        {
            return m_write_check;
        }

        public int next_proc_id()
        {
            return (++m_next_proc_id);
        }
        //-----------------------------------------------------------------------------
        // \/\/\/\/\\\/\/\\/\/\/\\/\/\/\/\/\/\/\/\/\/\\/\/\/\/\/\/\\/\/\/\/\
        /*
        public void add_trace_file(sc_trace_file tf)
        {
            m_trace_files.Add(tf);
            m_something_to_trace = true;
        }
        public void remove_trace_file(sc_trace_file tf)
        {
            m_trace_files.erase(std.remove(m_trace_files.GetEnumerator(), m_trace_files.end(), tf));
            m_something_to_trace = (m_trace_files.Count > 0);
        }
        */
        // \/\/\/\/\\\/\/\\/\/\/\\/\/\/\/\/\/\/\/\/\/\\/\/\/\/\/\/\\/\/\/\/\
        //------------------------------------------------------------------------------
        public sc_time max_time()
        {
            if (m_max_time == sc_time.SC_ZERO_TIME)
            {
                m_max_time = sc_time.from_value(uint.MaxValue);
            }
            return m_max_time;
        }

        public sc_time time_stamp()
        {
            return m_curr_time;
        }


        public ulong change_stamp()
        {
            return m_change_stamp;
        }


        private bool delta_count_warn_delta_count = true;

        public ulong delta_count()
        {
            global::sc_core.sc_report_handler.report(sc_core.sc_severity.SC_INFO, "/IEEE_Std_1666/deprecated", "sc_simcontext::delta_count() is deprecated, use sc_delta_count()");
            return m_delta_count;
        }

        public bool event_occurred(ulong last_change_stamp)
        {
            return m_change_stamp == last_change_stamp;
        }

        public bool evaluation_phase()
        {
            return (m_execution_phase == execution_phases.phase_evaluate) && m_ready_to_simulate;
        }

        public bool is_running()
        {
            sc_report_handler.report(sc_core.sc_severity.SC_INFO, "/IEEE_Std_1666/deprecated", "sc_simcontext::is_running() is deprecated, use sc_is_running()");
            return m_ready_to_simulate;
        }

        public bool update_phase()
        {
            return m_execution_phase == execution_phases.phase_update;
        }

        public bool notify_phase()
        {
            return m_execution_phase == execution_phases.phase_notify;
        }
        public bool get_error()
        {
            return m_error != null;
        }

        public void set_error(sc_report err)
        {
            m_error = err;
        }

        public sc_cor_pkg cor_pkg()
        {
            return m_cor_pkg;
        }

        public void reset_curr_proc()
        {
            m_curr_proc_info.process_handle = null;
            m_curr_proc_info.kind = sc_curr_proc_kind.SC_NO_PROC_;
            m_current_writer = null;
            sc_process_b.m_last_created_process_p = null;
        }

        public sc_method_process pop_runnable_method()
        {
            sc_method_process method_h = m_runnable.pop_method();
            if (method_h == null)
            {
                reset_curr_proc();
                return null;
            }
            set_curr_proc(method_h as sc_process_b);
            return method_h;
        }

        public sc_thread_process pop_runnable_thread()
        {
            sc_thread_process thread_h = m_runnable.pop_thread();
            if (thread_h == null)
            {
                reset_curr_proc();
                return null;
            }
            set_curr_proc(thread_h as sc_process_b);
            return thread_h;
        }

        public sc_cor next_cor()
        {
            if (m_error != null)
            {
                return m_cor;
            }

            sc_thread_process thread_h = pop_runnable_thread();
            while (thread_h != null)
            {
                if (thread_h.get_cor() != null)
                    break;
                thread_h = pop_runnable_thread();
            }

            if (thread_h != null)
            {
                return thread_h.get_cor();
            }
            else
            {
                return m_cor;
            }
        }


        public List<sc_object> get_child_objects()
        {
            sc_report_handler.report(sc_core.sc_severity.SC_INFO, "/IEEE_Std_1666/deprecated", "sc_simcontext::get_child_objects() is deprecated,\n" + " use sc_get_top_level_objects()");
            return m_child_objects;
        }

        public void elaborate()
        {
            if (m_elaboration_done || sim_status() != SC_SIM_OK)
            {
                return;
            }

            // Instantiate the method invocation module
            // (not added to public object hierarchy)

            //-----------------------------------------------------------------------------
            // \/\/\/\/\\\/\/\\/\/\/\\/\/\/\/\/\/\/\/\/\/\\/\/\/\/\/\/\\/\/\/\/\
            /*
            m_method_invoker_p = new sc_invoke_method("$$$$kernel_module$$$$_invoke_method");

            m_simulation_status = sc_status.SC_BEFORE_END_OF_ELABORATION;
            for (int cd = 0; cd != 4; ) // empty
            {
                cd = m_port_registry.construction_done();
                cd += m_export_registry.construction_done();
                cd += m_prim_channel_registry.construction_done();
                cd += m_module_registry.construction_done();

                // check for call(s) to sc_stop
                if (m_forced_stop)
                {
                    do_sc_stop_action();
                    return;
                }

            }
            */
            // \/\/\/\/\\\/\/\\/\/\/\\/\/\/\/\/\/\/\/\/\/\\/\/\/\/\/\/\\/\/\/\/\
            //--------------------------------------------------------------------------

            // SIGNAL THAT ELABORATION IS DONE
            //
            // We set the switch before the calls in case someone creates a process
            // in an end_of_elaboration callback. We need the information to flag
            // the process as being dynamic.

            m_elaboration_done = true;
            m_simulation_status = sc_status.SC_END_OF_ELABORATION;

            //------------------------------------------------------------------
            // \/\/\/\/\\\/\/\\/\/\/\\/\/\/\/\/\/\/\/\/\/\\/\/\/\/\/\/\\/\/\/\/\
            /*
            m_port_registry.elaboration_done();
            m_export_registry.elaboration_done();
            m_prim_channel_registry.elaboration_done();
            
            sc_reset.reconcile_resets();
            */
            // \/\/\/\/\\\/\/\\/\/\/\\/\/\/\/\/\/\/\/\/\/\\/\/\/\/\/\/\\/\/\/\/\
            //------------------------------------------------------------------

            m_module_registry.elaboration_done();

            // check for call(s) to sc_stop
            if (m_forced_stop)
            {
                do_sc_stop_action();
                return;
            }
        }
        public void prepare_to_simulate()
        {
            sc_method_process method_p;
            sc_thread_process thread_p;

            //------------------------------------------------------------------
            // \/\/\/\/\\\/\/\\/\/\/\\/\/\/\/\/\/\/\/\/\/\\/\/\/\/\/\/\\/\/\/\/\
            /*
            if (m_ready_to_simulate || sim_status() != SC_SIM_OK)
            {
                return;
            }
            */
            // \/\/\/\/\\\/\/\\/\/\/\\/\/\/\/\/\/\/\/\/\/\\/\/\/\/\/\/\\/\/\/\/\
            //------------------------------------------------------------------
            // instantiate the coroutine package
            m_cor_pkg = new sc_cor_pkg(this);

            m_cor = m_cor_pkg.get_main();


            // NOTIFY ALL OBJECTS THAT SIMULATION IS ABOUT TO START:

            m_simulation_status = sc_status.SC_START_OF_SIMULATION;
            //------------------------------------------------------------------
            // \/\/\/\/\\\/\/\\/\/\/\\/\/\/\/\/\/\/\/\/\/\\/\/\/\/\/\/\\/\/\/\/\
            /*
            m_port_registry.start_simulation();
            m_export_registry.start_simulation();
            m_prim_channel_registry.start_simulation();
            m_module_registry.start_simulation();
            */
            // \/\/\/\/\\\/\/\\/\/\/\\/\/\/\/\/\/\/\/\/\/\\/\/\/\/\/\/\\/\/\/\/\
            //------------------------------------------------------------------

            m_start_of_simulation_called = true;

            // CHECK FOR CALL(S) TO sc_stop

            if (m_forced_stop)
            {
                do_sc_stop_action();
                return;
            }

            // PREPARE ALL (C)THREAD PROCESSES FOR SIMULATION:

            for (thread_p = m_process_table.thread_q_head(); thread_p != null; thread_p = thread_p.next_exist())
            {
                thread_p.prepare_for_simulation();
            }

            m_simulation_status = sc_status.SC_RUNNING;
            m_ready_to_simulate = true;
            m_runnable.Init();

            // update phase

            m_execution_phase = execution_phases.phase_update;
            //------------------------------------------------------------------
            // \/\/\/\/\\\/\/\\/\/\/\\/\/\/\/\/\/\/\/\/\/\\/\/\/\/\/\/\\/\/\/\/\
            //m_prim_channel_registry.perform_update();
            // \/\/\/\/\\\/\/\\/\/\/\\/\/\/\/\/\/\/\/\/\/\\/\/\/\/\/\/\\/\/\/\/\
            //------------------------------------------------------------------
            m_execution_phase = execution_phases.phase_notify;

            int size;

            // make all method processes runnable

            for (method_p = m_process_table.method_q_head(); method_p != null; method_p = method_p.next_exist())
            {
                if (((method_p.current_state() & (int)process_state.ps_bit_disabled) != 0) || method_p.dont_initialize())
                {
                    if (method_p.m_static_events.Count == 0)
                    {
                        sc_report_handler.report(sc_core.sc_severity.SC_WARNING, "disable() or dont_initialize() called on process with no static sensitivity, it will be orphaned", method_p.name());
                    }
                }
                else if ((method_p.current_state() & (int)process_state.ps_bit_suspended) == 0)
                {
                    push_runnable_method_front(method_p);
                }
                else
                {
                    method_p.m_state |= (int)process_state.ps_bit_ready_to_run;
                }
            }

            // make thread processes runnable
            // (cthread processes always have the dont_initialize flag set)

            for (thread_p = m_process_table.thread_q_head(); thread_p!=null; thread_p = thread_p.next_exist())
            {
                if (((thread_p.m_state & (int)process_state.ps_bit_disabled) != 0) || thread_p.dont_initialize())
                {
                    if (thread_p.m_static_events.Count == 0)
                    {
                        sc_report_handler.report(sc_core.sc_severity.SC_WARNING, "disable() or dont_initialize() called on process with no static sensitivity, it will be orphaned", thread_p.name());
                    }
                }
                else if ((thread_p.m_state & (int)process_state.ps_bit_suspended) == 0)
                {
                    push_runnable_thread_front(thread_p);
                }
                else
                {
                    thread_p.m_state |= (int)process_state.ps_bit_ready_to_run;
                }
            }


            // process delta notifications

            if ((size = m_delta_events.Count) != 0)
            {
                sc_event[] l_delta_events = m_delta_events.ToArray();
                int i = size - 1;
                do
                {
                    l_delta_events[i].trigger();
                } while (--i >= 0);
                m_delta_events.Clear();
            }


        }

        public void initial_crunch(bool no_crunch)
        {
            if (no_crunch || m_runnable.IsEmpty)
            {
                return;
            }

            // run the delta cycle loop

            crunch();
            if (m_error != null)
            {
                return;
            }

            if (m_something_to_trace)
            {
                trace_cycle(false);
            }

            // check for call(s) to sc_stop
            if (m_forced_stop)
            {
                do_sc_stop_action();
            }
        }

        // +----------------------------------------------------------------------------
        // |"sc_simcontext::next_time"
        // | 
        // | This method returns the time of the next event. If there are no events
        // | it returns false.
        // | 
        // | Arguments:
        // |     result = where to place time of the next event, if no event is 
        // |              found this value will not be changed.
        // | Result is true if an event is found, false if not.
        // +----------------------------------------------------------------------------
        //C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
        //ORIGINAL LINE: bool next_time(sc_time& result) const
        public bool next_time(out sc_time result)
        {
            while (m_timed_events.Count != 0)
            {
                sc_event_timed et = m_timed_events[m_timed_events.Count - 1];
                if (et.Event() != null)
                {
                    result = et.notify_time();
                    return true;
                }
                m_timed_events.RemoveAt(m_timed_events.Count - 1);
            }
            result = null;
            return false;
        }

        // Return indication if there are more processes to execute in this delta phase

        public bool pending_activity_at_current_time()
        {
            // \/\/\/\/\\\/\/\\/\/\/\\/\/\/\/\/\/\/\/\/\/\\/\/\/\/\/\/\\/\/\/\/\
            //ORIGINAL : return (m_delta_events.Count != 0) || (m_runnable.IsInitialized && !m_runnable.IsEmpty) || m_prim_channel_registry.pending_updates();
            // \/\/\/\/\\\/\/\\/\/\/\\/\/\/\/\/\/\/\/\/\/\\/\/\/\/\/\/\\/\/\/\/\
            return (m_delta_events.Count != 0) || (m_runnable.IsInitialized && !m_runnable.IsEmpty);
        }


        public virtual void add_child_event(sc_event event_)
        {
            // no check if object_ is already in the set
            m_child_events.Add(event_);
        }
        public virtual void add_child_object(sc_object object_)
        {
            // no check if object_ is already in the set
            m_child_objects.Add(object_);
        }
        public virtual void remove_child_event(sc_event event_)
        {
            int size = m_child_events.Count;
            for (int i = 0; i < size; ++i)
            {
                if (event_ == m_child_events[i])
                {
                    m_child_events.RemoveAt(i);
                    return;
                }
            }
            // no check if event_ is really in the set
        }
        public virtual void remove_child_object(sc_object object_)
        {
            int size = m_child_objects.Count;
            for (int i = 0; i < size; ++i)
            {
                if (object_ == m_child_objects[i])
                {
                    m_child_objects.RemoveAt(i);
                    return;
                }
            }
            // no check if object_ is really in the set
        }


        // +----------------------------------------------------------------------------
        // |"sc_simcontext::crunch"
        // | 
        // | This method implements the simulator's execution of processes. It performs
        // | one or more "delta" cycles. Each delta cycle consists of an evaluation,
        // | an update phase, and a notification phase. During the evaluation phase any 
        // | processes that are ready to run are executed. After all the processes have
        // | been executed the update phase is entered. During the update phase the 
        // | values of any signals that have changed are updated. After the updates
        // | have been performed the notification phase is entered. During that phase
        // | any notifications that need to occur because of of signal values changes
        // | are performed. This will result in the queueing of processes for execution
        // | that are sensitive to those notifications. At that point a delta cycle
        // | is complete, and the process is started again unless 'once' is true.
        // |
        // | Arguments:
        // |     once = true if only one delta cycle is to be performed.
        // +----------------------------------------------------------------------------
        private void crunch()
        {
            crunch(false);
        }

        private void crunch(bool once)
        {
            int num_deltas = 0; // number of delta cycles

            while (true)
            {

                // EVALUATE PHASE

                m_execution_phase = execution_phases.phase_evaluate;
                bool empty_eval_phase = true;
                while (true)
                {

                    // execute method processes

                    m_runnable.toggle_methods();
                    sc_method_process method_h = pop_runnable_method();
                    while (method_h != null)
                    {
                        empty_eval_phase = false;
                        if (!method_h.run_process())
                        {
                            goto ___out;
                        }
                        method_h = pop_runnable_method();
                    }

                    // execute (c)thread processes

                    m_runnable.toggle_threads();
                    sc_thread_process thread_h = pop_runnable_thread();
                    while (thread_h != null)
                    {
                        if (thread_h.get_cor() != null)
                            break;
                        thread_h = pop_runnable_thread();
                    }

                    if (thread_h != null)
                    {
                        empty_eval_phase = false;
                        m_cor_pkg.yield(thread_h.get_cor());
                    }
                    if (m_error != null)
                    {
                        goto ___out;
                    }

                    // check for call(s) to sc_stop
                    if (m_forced_stop)
                    {
                        if (stop_mode == sc_stop_mode.SC_STOP_IMMEDIATE)
                            goto ___out;
                    }

                    // no more runnable processes

                    if (m_runnable.IsEmpty)
                    {
                        break;
                    }
                }

                // remove finally dead zombies:

                foreach (sc_process_b del_p in m_collectable)
                {
                    del_p.reference_decrement();
                }

                m_collectable.Clear();



                // UPDATE PHASE
                //
                // The change stamp must be updated first so that event_occurred()
                // will work.

                m_execution_phase = execution_phases.phase_update;
                if (!empty_eval_phase)
                {
                    //	    SC_DO_PHASE_CALLBACK_(evaluation_done);
                    m_change_stamp++;
                    m_delta_count++;
                }
                // \/\/\/\/\\\/\/\\/\/\/\\/\/\/\/\/\/\/\/\/\/\\/\/\/\/\/\/\\/\/\/\/\
                //m_prim_channel_registry.perform_update();
                // \/\/\/\/\\\/\/\\/\/\/\\/\/\/\/\/\/\/\/\/\/\\/\/\/\/\/\/\\/\/\/\/\

                m_execution_phase = execution_phases.phase_notify;


                if (m_something_to_trace)
                {
                    trace_cycle(true); // delta cycle?
                }


                // check for call(s) to sc_stop
                if (m_forced_stop)
                {
                    break;
                }


                // NOTIFICATION PHASE:
                //
                // Process delta notifications which will queue processes for
                // subsequent execution.

                int size = m_delta_events.Count;
                if (size != 0)
                {
                    sc_event[] l_events = m_delta_events.ToArray();
                    int i = size - 1;
                    do
                    {
                        l_events[i].trigger();
                    } while (--i >= 0);
                    m_delta_events.Clear();
                }

                if (m_runnable.IsEmpty)
                {
                    // no more runnable processes
                    break;
                }

                // if sc_pause() was called we are done.

                if (m_paused)
                    break;

                // IF ONLY DOING ONE CYCLE, WE ARE DONE. OTHERWISE EXECUTE NEW CALLBACKS

                if (once)
                    break;
            }

            // When this point is reached the processing of delta cycles is complete,
        // if the completion was because of an error throw the exception specified
        // by '*m_error'.
        ___out:
            this.reset_curr_proc();
            if (m_error != null) // re-throw propagated error
                throw m_error;
        }

        public virtual int add_delta_event(sc_event e)
        {
            m_delta_events.Add(e);
            return (m_delta_events.Count - 1);
        }
        public virtual void remove_delta_event(sc_event e)
        {
            int i = e.m_delta_event_index;
            int j = m_delta_events.Count - 1;
            Debug.Assert(i >= 0 && i <= j);
            if (i != j)
            {
                sc_event[] l_delta_events = m_delta_events.ToArray();
                l_delta_events[i] = l_delta_events[j];
                l_delta_events[i].m_delta_event_index = i;
            }
            m_delta_events.RemoveAt(m_delta_events.Count - 1);
            e.m_delta_event_index = -1;
        }
        public virtual void add_timed_event(sc_event_timed et)
        {
            m_timed_events.Add(et);
        }


        public virtual void trace_cycle(bool delta_cycle)
        {
            // \/\/\/\/\\\/\/\\/\/\/\\/\/\/\/\/\/\/\/\/\/\\/\/\/\/\/\/\\/\/\/\/\
            /*
            int size;
            if ((size = m_trace_files.Count) != 0)
            {
                sc_trace_file[] l_trace_files = m_trace_files[0];
                int i = size - 1;
                do
                {
                    l_trace_files[i].cycle(delta_cycle);
                } while (--i >= 0);
            }
            */
            // \/\/\/\/\\\/\/\\/\/\/\\/\/\/\/\/\/\/\/\/\/\\/\/\/\/\/\/\\/\/\/\/\
        }

        public virtual void preempt_with(sc_method_process method_h)
        {
            sc_curr_proc_info caller_info = new sc_curr_proc_info(); // process info for caller.
            sc_method_process active_method_h;
            sc_thread_process active_thread_h;

            // Determine the active process and take the thread to be run off the
            // run queue, if its there, since we will be explicitly causing its
            // execution.

            active_method_h = sc_get_current_process_b() as sc_method_process;
            active_thread_h = sc_get_current_process_b() as sc_thread_process;
            if (method_h.next_runnable() != null)
                remove_runnable_method(method_h);

            // CALLER IS THE METHOD TO BE RUN:
            //
            // Should never get here, ignore it unless we are debugging.



            // THE CALLER IS A METHOD:
            //
            //   (a) Set the current process information to our method.
            //   (b) Invoke our method directly by-passing the run queue.
            //   (c) Restore the process info to the caller.
            //   (d) Check to see if the calling method should throw an exception
            //       because of activity that occurred during the preemption.

            else if (active_method_h != null)
            {
                sc_get_curr_simcontext().set_curr_proc((sc_process_b)method_h);
                method_h.run_process();
                sc_get_curr_simcontext().set_curr_proc((sc_process_b)active_method_h);
                active_method_h.check_for_throws();
            }

            // CALLER IS A THREAD:
            //
            //   (a) Use an invocation thread to execute the method.

            else if (active_thread_h != null)
            {
                m_method_invoker_p.invoke_method(method_h);
            }

            // CALLER IS THE SIMULATOR:
            //
            // That is not allowed.

            else
            {

                caller_info = (m_curr_proc_info);
                sc_get_curr_simcontext().set_curr_proc((sc_process_b)method_h);
                method_h.run_process();
                m_curr_proc_info = (caller_info);
            }
        }

        //------------------------------------------------------------------------------
        //"sc_simcontext::requeue_current_process"
        //
        // This method requeues the current process at the beginning of the run queue
        // if it is a thread. This is called by sc_process_handle::throw_it() to assure
        // that a thread that is issuing a throw will execute immediately after the
        // processes it notifies via the throw.
        //------------------------------------------------------------------------------
        public virtual void requeue_current_process()
        {
            sc_thread_process thread_p;
            thread_p = get_curr_proc_info().process_handle as sc_thread_process;
            if (thread_p != null)
            {
                execute_thread_next(thread_p);
            }
        }

        //------------------------------------------------------------------------------
        //"sc_simcontext::suspend_current_process"
        //
        // This method suspends the current process if it is a thread. This is called 
        // by sc_process_handle::throw_it() to allow the processes that have received
        // a throw to execute.
        //------------------------------------------------------------------------------
        public virtual void suspend_current_process()
        {
            sc_thread_process thread_p;
            thread_p = get_curr_proc_info().process_handle as sc_thread_process;
            if (thread_p != null)
            {
                thread_p.suspend_me();
            }
        }

        public virtual void do_sc_stop_action()
        {
            sc_report_handler.report(sc_core.sc_severity.SC_INFO, "/OSCI/SystemC", "Simulation stopped by user.");
            if (m_start_of_simulation_called)
            {
                end();
                m_in_simulator_control = false;
            }
            m_simulation_status = sc_status.SC_STOPPED;

        }

        public virtual void mark_to_collect_process(sc_process_b zombie)
        {
            m_collectable.Add(zombie);
        }


        private enum execution_phases
        {
            phase_initialize = 0,
            phase_evaluate,
            phase_update,
            phase_notify
        }
        private sc_object_manager m_object_manager;

        private sc_module_registry m_module_registry;
        //\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\
        //---------------------------------------------------------------------------------
        //private sc_port_registry m_port_registry;
        //private sc_export_registry m_export_registry;
        //private sc_prim_channel_registry m_prim_channel_registry;
        //---------------------------------------------------------------------------------
        //\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\
        public sc_phase_callback_registry m_phase_cb_registry;

        private sc_name_gen m_name_gen;

        private sc_process_table m_process_table;
        private sc_curr_proc_info m_curr_proc_info = new sc_curr_proc_info();
        private sc_object m_current_writer;
        private bool m_write_check;
        private int m_next_proc_id;

        private Stack<sc_thread_process> m_active_invokers = new Stack<sc_thread_process>();

        private List<sc_event> m_child_events = new List<sc_event>();
        private List<sc_object> m_child_objects = new List<sc_object>();

        private List<sc_event> m_delta_events = new List<sc_event>();
        private List<sc_event_timed> m_timed_events;

        //---------------------------------------------------------------------------------
        //\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\
        //private List<sc_trace_file> m_trace_files = new List<sc_trace_file>();
        //\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\
        //---------------------------------------------------------------------------------
        private bool m_something_to_trace;

        private sc_runnable m_runnable;
        private List<sc_process_b> m_collectable;

        public sc_time_params m_time_params;
        private sc_time m_curr_time = new sc_time();
        private sc_time m_max_time = new sc_time();

        private sc_invoke_method m_method_invoker_p;
        private ulong m_change_stamp; // "time" change occurred.
        private ulong m_delta_count;
        private bool m_forced_stop;
        private bool m_paused;
        private bool m_ready_to_simulate;
        private bool m_elaboration_done;
        private execution_phases m_execution_phase;
        private sc_report m_error;
        private bool m_in_simulator_control;
        private bool m_end_of_simulation_called;
        private sc_status m_simulation_status;
        private bool m_start_of_simulation_called;

        private sc_cor_pkg m_cor_pkg; // the simcontext's coroutine package
        private sc_cor m_cor; // the simcontext's coroutine


        //------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------

        // simulation status codes

        public const int SC_SIM_OK = 0;
        public const int SC_SIM_ERROR = 1;
        public const int SC_SIM_USER_STOP = 2;

        //------------------------------------------------------------------------------
        //"sc_set_stop_mode"
        //
        // This function sets the mode of operation when sc_stop() is called.
        //     mode = SC_STOP_IMMEDIATE or SC_STOP_FINISH_DELTA.
        //------------------------------------------------------------------------------
        public static void sc_set_stop_mode(sc_stop_mode mode)
        {
            if (sc_is_running())
            {
                sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "attempt to set sc_stop mode  after start will be ignored", "");
            }
            else
            {
                switch (mode)
                {
                    case sc_stop_mode.SC_STOP_IMMEDIATE:
                    case sc_stop_mode.SC_STOP_FINISH_DELTA:
                        stop_mode = mode;
                        break;
                    default:
                        break;
                }
            }
        }
        public static sc_stop_mode sc_get_stop_mode()
        {
            return stop_mode;
        }
        public static void sc_start()
        {
            sc_start(sc_max_time() - sc_time_stamp(), sc_starvation_policy.SC_EXIT_ON_STARVATION);
        }

        // +----------------------------------------------------------------------------
        // |"sc_start"
        // | 
        // | This function starts, or restarts, the execution of the simulator.
        // |
        // | Arguments:
        // |     duration = the amount of time the simulator should execute.
        // |     p        = event starvation policy.
        // +----------------------------------------------------------------------------
        private static bool sc_start_init_delta_or_pending_updates;// = (starting_delta == 0 && exit_time == sc_time.SC_ZERO_TIME);

        public static void sc_start(sc_time duration)
        {
            sc_start(duration, sc_starvation_policy.SC_RUN_TO_TIME);
        }
        public static void sc_start(sc_time duration, sc_starvation_policy p)
        {
            sc_simcontext context_p; // current simulation context.
            sc_time entry_time = new sc_time(); // simulation time upon entry.
            sc_time exit_time = new sc_time(); // simulation time to set upon exit.
            ulong starting_delta; // delta count upon entry.
            int status; // current simulation status.

            // Set up based on the arguments passed to us:

            context_p = sc_get_curr_simcontext();
            starting_delta = sc_delta_count();

            entry_time = context_p.m_curr_time;
            if (p == sc_starvation_policy.SC_RUN_TO_TIME)
                exit_time = context_p.m_curr_time + duration;


            status = context_p.sim_status();
            if (status != SC_SIM_OK)
            {
                if (status == SC_SIM_USER_STOP)
                    sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "sc_start called after sc_stop has been called", "");
                if (status == SC_SIM_ERROR)
                    sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "sc_start called after sc_stop has been called", "");
                return;
            }
            //------------------------------------------------------------------
            // \/\/\/\/\\\/\/\\/\/\/\\/\/\/\/\/\/\/\/\/\/\\/\/\/\/\/\/\\/\/\/\/\
            //if (context_p.m_prim_channel_registry.pending_updates())
            //sc_start_init_delta_or_pending_updates = true;
            // \/\/\/\/\\\/\/\\/\/\/\\/\/\/\/\/\/\/\/\/\/\\/\/\/\/\/\/\\/\/\/\/\
            //------------------------------------------------------------------


            context_p.simulate(duration);

            // Re-check the status:

            status = context_p.sim_status();

            // Update the current time to the exit time if that is the starvation
            // policy:

            if (p == sc_starvation_policy.SC_RUN_TO_TIME && !context_p.m_paused && status == SC_SIM_OK)
            {
                context_p.m_curr_time = exit_time;
            }

            // If there was no activity and the simulation clock did not move warn
            // the user, except if we're in a first sc_start(SC_ZERO_TIME) for
            // initialisation (only) or there have been pending updates:

            if (!sc_start_init_delta_or_pending_updates && starting_delta == sc_delta_count() && context_p.m_curr_time == entry_time && status == SC_SIM_OK)
            {
                sc_report_handler.report(sc_core.sc_severity.SC_WARNING, "no activity or clock movement for sc_start() invocation", "");
            }

            // reset init/update flag for subsequent calls
            sc_start_init_delta_or_pending_updates = false;
        }
        public static void sc_start(int duration, sc_time_unit unit)
        {
            sc_start(duration, unit, sc_starvation_policy.SC_RUN_TO_TIME);
        }

        public static void sc_start(int duration, sc_time_unit unit, sc_starvation_policy p)
        {
            sc_start(new sc_time((double)duration, unit), p);
        }

        public static void sc_start(double duration, sc_time_unit unit)
        {
            sc_start(duration, unit, sc_starvation_policy.SC_RUN_TO_TIME);
        }


        public static void sc_start(double duration, sc_time_unit unit, sc_starvation_policy p)
        {
            sc_start(new sc_time(duration, unit), p);
        }

        public static void sc_stop()
        {
            sc_get_curr_simcontext().stop();
        }

        // friend function declarations

        public static ulong sc_delta_count()
        {
            return sc_get_curr_simcontext().m_delta_count;
        }
        public static List<sc_event> sc_get_top_level_events()
        {
            return sc_get_top_level_events(sc_get_curr_simcontext());
        }

        public static List<sc_event> sc_get_top_level_events(sc_simcontext simc_p)
        {
            return simc_p.m_child_events;
        }
        public static List<sc_object> sc_get_top_level_objects()
        {
            return sc_get_top_level_objects(sc_get_curr_simcontext());
        }

        public static List<sc_object> sc_get_top_level_objects(sc_simcontext simc_p)
        {
            return simc_p.m_child_objects;
        }
        public static bool sc_is_running()
        {
            return sc_is_running(sc_get_curr_simcontext());
        }

        public static bool sc_is_running(sc_simcontext simc_p)
        {
            return simc_p.m_ready_to_simulate;
        }
        public static void sc_pause()
        {
            sc_get_curr_simcontext().m_paused = true;
        }
        public static bool sc_end_of_simulation_invoked()
        {
            return sc_get_curr_simcontext().m_end_of_simulation_called;
        }
        public static bool sc_start_of_simulation_invoked()
        {
            return sc_get_curr_simcontext().m_start_of_simulation_called;
        }

        public static bool sc_pending_activity_at_current_time()
        {
            return sc_pending_activity_at_current_time(sc_get_curr_simcontext());
        }

        public static bool sc_pending_activity_at_current_time(sc_simcontext simc_p)
        {
            return simc_p.pending_activity_at_current_time();
        }

        // Return indication if there are timed notifications in the future

        public static bool sc_pending_activity_at_future_time()
        {
            return sc_pending_activity_at_future_time(sc_get_curr_simcontext());
        }

        public static bool sc_pending_activity_at_future_time(sc_simcontext simc_p)
        {
            sc_time ignored = new sc_time();
            return simc_p.next_time(out ignored);
        }

        // Return time of next activity.

        public static sc_time sc_time_to_pending_activity(sc_simcontext simc_p)
        {
            // If there is an activity pending at the current time
            // return a delta of zero.

            sc_time result = sc_time.SC_ZERO_TIME; // time of pending activity.

            if (simc_p.pending_activity_at_current_time())
            {
                return result;
            }

            // Any activity will take place in the future pick up the next event's time.

            else
            {
                result = simc_p.max_time();
                simc_p.next_time(out result);
                result -= sc_simcontext.sc_time_stamp();
            }
            return result;
        }

        //C++ TO C# CONVERTER NOTE: C# has no need of forward class declarations:
        //struct sc_invoke_method;

        // IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIII

        // Not MT safe.


        public static sc_simcontext sc_get_curr_simcontext()
        {
            if (sc_curr_simcontext == null)
            {
                sc_default_global_context = new sc_simcontext();
                sc_curr_simcontext = sc_simcontext.sc_default_global_context;
            }
            return sc_curr_simcontext;
        }
        private static sc_simcontext sc_get_curr_simcontext_sc_default_global_context = new sc_simcontext();

        public static sc_status sc_get_status()
        {
            return sc_simcontext.sc_get_curr_simcontext().get_status();
        }



        public static sc_process_handle sc_get_current_process_handle()
        {
            return (sc_is_running()) ? new sc_process_handle(sc_get_current_process_b()) : sc_process_handle.sc_get_last_created_process_handle();
        }

        // Get the current object hierarchy context
        //
        // Returns a pointer the the sc_object (module or process) that
        // would become the parent object of a newly created element
        // of the SystemC object hierarchy, or NULL.
        //
        public static sc_object sc_get_current_object()
        {
            return sc_get_curr_simcontext().active_object();
        }

        public static sc_process_b sc_get_current_process_b()
        {
            return sc_get_curr_simcontext().get_curr_proc_info().process_handle;
        }

        public static sc_process_b sc_get_curr_process_handle()
        {
            sc_report_handler.report(sc_core.sc_severity.SC_INFO, "/IEEE_Std_1666/deprecated", "sc_get_curr_process_handle deprecated use sc_get_current_process_handle");
            return sc_get_curr_simcontext().get_curr_proc_info().process_handle;
        }

        public static sc_curr_proc_kind sc_get_curr_process_kind()
        {
            return sc_get_curr_simcontext().get_curr_proc_info().kind;
        }


        public static int sc_get_simulator_status()
        {
            return sc_get_curr_simcontext().sim_status();
        }

        // Generates unique names within each module.

        public static string sc_gen_unique_name(string basename_)
        {
            return sc_gen_unique_name(basename_, false);
        }

        public static string sc_gen_unique_name(string basename_, bool preserve_first = true)
        {
            sc_simcontext simc = sc_simcontext.sc_get_curr_simcontext();
            sc_module curr_module = simc.hierarchy_curr();
            if (curr_module != null)
            {
                return curr_module.gen_unique_name(basename_, preserve_first);
            }
            else
            {
                sc_process_b curr_proc_p = sc_simcontext.sc_get_current_process_b();
                if (curr_proc_p != null)
                {
                    return curr_proc_p.gen_unique_name(basename_, preserve_first);
                }
                else
                {
                    return simc.gen_unique_name(basename_, preserve_first);
                }
            }
        }

        // Set the random seed for controlled randomization -- not yet implemented



        // Set the random seed for controlled randomization -- not yet implemented
        public static void sc_set_random_seed(uint seed_)
        {
            sc_report_handler.report(sc_core.sc_severity.SC_WARNING, "not implemented", "void sc_set_random_seed( unsigned int )");
        }

        // The following function is deprecated in favor of sc_start(SC_ZERO_TIME):

        public static void sc_initialize()
        {
            sc_report_handler.report(sc_core.sc_severity.SC_INFO, "/IEEE_Std_1666/deprecated", "sc_initialize() is deprecated: use sc_start(SC_ZERO_TIME)");

            sc_get_curr_simcontext().initialize();
        }

        public static sc_time sc_max_time()
        {
            return sc_get_curr_simcontext().max_time();
        }
        public static sc_time sc_time_stamp()
        {
            return sc_get_curr_simcontext().time_stamp();
        }

        public static double sc_simulation_time()
        {
            sc_report_handler.report(sc_core.sc_severity.SC_INFO, "/IEEE_Std_1666/deprecated", "sc_simulation_time() is deprecated use sc_time_stamp()");
            return sc_get_curr_simcontext().time_stamp().to_default_time_units();
        }

        public static sc_event sc_find_event(string name)
        {
            return sc_get_curr_simcontext().get_object_manager().find_event(name);
        }

        public static sc_object sc_find_object(string name)
        {
            return sc_get_curr_simcontext().get_object_manager().find_object(name);
        }

        public static bool sc_is_unwinding()
        {
            return sc_get_current_process_handle().is_unwinding();
        }
        public static bool sc_pending_activity()
        {
            return sc_pending_activity(sc_get_curr_simcontext());
        }


        public static bool sc_pending_activity(sc_simcontext simc_p)
        {
            return sc_pending_activity_at_current_time(simc_p) || sc_simcontext.sc_pending_activity_at_future_time(simc_p);
        }

        public static bool sc_hierarchical_name_exists(string name)
        {
            return sc_find_object(name) != null || sc_simcontext.sc_find_event(name) != null;
        }

        // The following variable controls whether process control corners should
        // be considered errors or not. See sc_simcontext.cpp for details on what
        // happens if this value is set to true.

        //C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
        //extern bool sc_allow_process_control_corners;

        public static sc_stop_mode stop_mode = sc_stop_mode.SC_STOP_FINISH_DELTA;



        public static int sc_notify_time_compare(object p1, object p2)
        {
            sc_event_timed et1 = (sc_event_timed)(p1);
            sc_event_timed et2 = (sc_event_timed)(p2);

            sc_time t1 = et1.notify_time();
            sc_time t2 = et2.notify_time();

            if (t1 < t2)
            {
                return 1;
            }
            else if (t1 > t2)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        internal static sc_simcontext sc_default_global_context = new sc_simcontext();
        public static sc_simcontext sc_curr_simcontext = sc_default_global_context;

        // The following function has been deprecated in favor of sc_start(duration):

        public static void sc_cycle(sc_time duration)
        {
            sc_report_handler.report(sc_core.sc_severity.SC_INFO, "/IEEE_Std_1666/deprecated", "sc_cycle is deprecated: use sc_start(sc_time)");
            sc_get_curr_simcontext().cycle(duration);
        }

        public static void sc_defunct_process_function(sc_module UnnamedParameter1)
        {
            // This function is pointed to by defunct sc_thread_process'es and
            // sc_cthread_process'es. In a correctly constructed world, this
            // function should never be called; hence the assert.
            Debug.Assert(false);
        }

        public static bool sc_allow_process_control_corners = false;

        // pretty-print a combination of sc_status bits (i.e. a callback mask)

        // The state transition diagram for the interaction of disable and suspend
        // when sc_allow_process_control_corners is true is shown below:
        //
        // ......................................................................
        // .         ENABLED                    .           DISABLED            .
        // .                                    .                               .
        // .                 +----------+    disable      +----------+          .
        // .   +------------>|          |-------.-------->|          |          .
        // .   |             | runnable |       .         | runnable |          .
        // .   |     +-------|          |<------.---------|          |------+   .
        // .   |     |       +----------+     enable      +----------+      |   .
        // .   |     |          |    ^          .            |    ^         |   .
        // .   |     |  suspend |    | resume   .    suspend |    | resume  |   .
        // .   |     |          V    |          .            V    |         |   .
        // .   |     |       +----------+    disable      +----------+      |   .
        // .   |     |       | suspend  |-------.-------->| suspend  |      |   .
        // . t |   r |       |          |       .         |          |      | r .
        // . r |   u |       |  ready   |<------.---------|  ready   |      | u .
        // . i |   n |       +----------+     enable      +----------+      | n .
        // . g |   / |         ^                .                           | / .
        // . g |   w |  trigger|                .                           | w .
        // . e |   a |         |                .                           | a .
        // . r |   i |       +----------+    disable      +----------+      | i .
        // .   |   t |       | suspend  |-------.-------->| suspend  |      | t .
        // .   |     |       |          |       .         |          |      |   .
        // .   |     |       | waiting  |<------.---------| waiting  |      |   .
        // .   |     |       +----------+     enable      +----------+      |   .
        // .   |     |          |    ^          .            |    ^         |   .
        // .   |     |  suspend |    | resume   .    suspend |    | resume  |   .
        // .   |     |          V    |          .            V    |         |   .
        // .   |     |       +----------+    disable      +----------+      |   .
        // .   |     +------>|          |-------.-------->|          |      |   .
        // .   |             | waiting  |       .         | waiting  |      |   .
        // .   +-------------|          |<------.---------|          |<-----+   .
        // .                 +----------+     enable      +----------+          .
        // .                                    .                               .
        // ......................................................................

        // ----------------------------------------------------------------------------

        public static string print_status_expression(sc_status s)
        {
            List<sc_status> bits = new List<sc_status>();
            uint is_set = (uint)sc_status.SC_ELABORATION;

            StringBuilder res = new StringBuilder(); ;

            // collect bits
            while (is_set <= (int)sc_status.SC_STATUS_LAST)
            {
                if (((int)s & is_set) != 0)
                    bits.Add((sc_status)is_set);
                is_set <<= 1;
            }
            if (((int)s & ~(int)sc_status.SC_STATUS_ANY) != 0) // remaining bits
                bits.Add((sc_status)(s & ~sc_status.SC_STATUS_ANY));

            // print expression
            int i = 0;
            int n = bits.Count;
            if (n > 1)
                res.Append("(");
            for (; i < n - 1; ++i)
                res.AppendFormat("{0}|", bits[i]);
            res.Append(bits[i]);
            if (n > 1)
                res.Append(")");
            return res.ToString();
        }

    }

    public class sc_process_table : IDisposable
    {


        // IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIII

        public sc_process_table()
        {
            m_method_q = null;
            m_thread_q = null;
        }
        public virtual void Dispose()
        {

            sc_method_process method_next_p;
            sc_method_process method_now_p;

            for (method_now_p = m_method_q; method_now_p != null; method_now_p = method_next_p)
            {
                method_next_p = method_now_p.next_exist();
                if (method_now_p != null)
                    method_now_p.Dispose();
            }

            if (m_thread_q != null)
            {
                Console.WriteLine("WATCH OUT!! In sc_process_table destructor.  Threads and cthreads are not actually getting deleted here.  Some memory may leak. Look at the comments here in kernel/sc_simcontext.cpp for more details.");
            }

        }
        public void push_front(sc_method_process handle_)
        {
            handle_.set_next_exist(m_method_q);
            m_method_q = handle_;
        }
        public void push_front(sc_thread_process handle_)
        {
            handle_.set_next_exist(m_thread_q);
            m_thread_q = handle_;
        }
        public virtual sc_thread_process thread_q_head()
        {
            return m_thread_q;
        }

        public virtual sc_method_process method_q_head()
        {
            return m_method_q;
        }

        private sc_method_process m_method_q;
        private sc_thread_process m_thread_q;
    }


    // +============================================================================
    // | CLASS sc_invoke_method - class to invoke sc_method's to support 
    // |                          sc_simcontext::preempt_with().
    // +============================================================================
    public class sc_invoke_method : sc_core.sc_module
    {
        public sc_invoke_method(sc_core.sc_module_name UnnamedParameter1)
        {
            // remove from object hierarchy
            detach();
        }

        public override void Dispose()
        {
            m_invokers.Clear();
        }

        // Method to call to execute a method's semantics. 

        public void invoke_method(sc_method_process method_h)
        {
            sc_process_handle invoker_h = new sc_process_handle(); // handle for invocation thread to use.
            int invokers_n = 0; // number of invocation threads available.

            m_method = method_h;

            // There is not an invocation thread to use, so allocate one.

            invokers_n = m_invokers.Count;
            if (invokers_n == 0)
            {
                sc_spawn_options options = new sc_spawn_options();
                options.dont_initialize();
                options.set_stack_size(0x100000);
                options.set_sensitivity(m_dummy);

                //---------------------------------------------------------------------------------
                //\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\
                //invoker_h = sc_spawn(bind(ref this.invoker, this), sc_gen_unique_name("invoker"), options);
                //((sc_process_b)invoker_h).detach();
                //\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\
                //---------------------------------------------------------------------------------
            }

            // There is an invocation thread to use, use the last one on the list.

            else
            {
                invoker_h = m_invokers[invokers_n - 1];
                m_invokers.RemoveAt(m_invokers.Count - 1);
            }

            // Fire off the invocation thread to invoke the method's semantics,
            // When it blocks put it onto the list of invocation threads that
            // are available.

            sc_get_curr_simcontext().preempt_with((sc_thread_process)invoker_h.get_process_object());
            m_invokers.Add(invoker_h);
        }

        // Thread to call method from:

        public void invoker()
        {
            sc_simcontext csc_p = sc_get_curr_simcontext();
            sc_process_b me = sc_simcontext.sc_get_current_process_b();

            for (; ; )
            {
                csc_p.set_curr_proc((sc_process_b)m_method);
                csc_p.get_active_invokers().Push((sc_thread_process)me);
                m_method.run_process();
                csc_p.set_curr_proc(me);
                csc_p.get_active_invokers().Pop();
                sc_wait.wait();
            }
        }

        public sc_event m_dummy = new sc_event(); // dummy event to wait on.
        public sc_method_process m_method;
        public List<sc_process_handle> m_invokers = new List<sc_process_handle>(); // list of invoking threads.
    }

} // namespace sc_core
