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
namespace sc_core
{

    // ----------------------------------------------------------------------------
    //  STRUCT : sc_msg_def
    //
    //  Exception message definition structure
    // ----------------------------------------------------------------------------

    public class sc_msg_def
    {
        public string msg_type;
        public uint actions = (int)sc_report_action.SC_DISPLAY;
        public uint[] sev_actions = new uint[(int)sc_severity.SC_MAX_SEVERITY];
        public uint limit;
        public uint[] sev_limit = new uint[(int)sc_severity.SC_MAX_SEVERITY];
        public uint limit_mask; // 0 - limit, 1..4 - sev_limit
        public uint call_count;
        public uint[] sev_call_count = new uint[(int)sc_severity.SC_MAX_SEVERITY];
        public string msg_type_data;

        public int id; // backward compatibility with 2.0+
    }

    public static class sc_report_handler
    {
        public const string SC_ID_REGISTER_ID_FAILED_ = "register_id failed";
        public const string SC_ID_UNKNOWN_ERROR_ = "unknown error";
        public const string SC_ID_WITHOUT_MESSAGE_ = "";
        public const string SC_ID_NOT_IMPLEMENTED_ = "not implemented";
        public const string SC_ID_INTERNAL_ERROR_ = "internal error";
        public const string SC_ID_ASSERTION_FAILED_ = "assertion failed";
        public const string SC_ID_OUT_OF_BOUNDS_ = "out of bounds";

        public delegate void sc_report_handler_proc(sc_report NamelessParameter1, uint NamelessParameter2);

        static sc_report_handler()
        {
            initialize();
        }

        public static void report(sc_severity severity_, string msg_type_, string msg_)
        {
            string file = new System.Diagnostics.StackTrace(true).GetFrame(1).GetFileName();
            string member = new System.Diagnostics.StackTrace(true).GetFrame(1).GetMethod().Name;
            int line = new System.Diagnostics.StackTrace(true).GetFrame(1).GetFileLineNumber();
            report(severity_, msg_type_, msg_, file, member, line);
        }

        public static void report(sc_severity severity_, string msg_type_, string msg_,
                        string file = "",
                        string member = "",
                        int line = 0)
        {
            sc_msg_def md = mdlookup(msg_type_);

            // If the severity of the report is SC_INFO and the maximum verbosity
            // level is less than SC_MEDIUM return without any action.

            if ((severity_ == sc_severity.SC_INFO) && ((uint)sc_verbosity.SC_MEDIUM > (uint)verbosity_level))
                return;

            // Process the report:


            if (md == null)
                md = add_msg_type(msg_type_);

            uint actions = execute(md, severity_);
            sc_report rep = new sc_report(severity_, md, msg_, 0, file, line, member);

            if ((actions & (uint)sc_report_action.SC_CACHE_REPORT) != 0)
                cache_report(new sc_report(rep));

            handler(rep, actions);
        }

        public static uint set_actions(sc_severity severity_)
        {
            return set_actions(severity_, (uint)sc_report_action.SC_UNSPECIFIED);
        }

        public static uint set_actions(sc_severity severity_, uint actions_)
        {
            uint old = sev_actions[(int)severity_];
            sev_actions[(int)severity_] = actions_;
            return old;
        }

        public static uint set_actions(string msg_type_)
        {
            return set_actions(msg_type_, (uint)sc_report_action.SC_UNSPECIFIED);
        }

        public static uint set_actions(string msg_type_, uint actions_)
        {
            sc_msg_def md = mdlookup(msg_type_);

            if (md == null)
                md = add_msg_type(msg_type_);

            uint old = md.actions;
            md.actions = actions_;

            return old;
        }

        public static uint set_actions(string msg_type_, sc_severity severity_)
        {
            return set_actions(msg_type_, severity_, (uint)sc_report_action.SC_UNSPECIFIED);
        }

        public static uint set_actions(string msg_type_, sc_severity severity_, uint actions_)
        {
            sc_msg_def md = mdlookup(msg_type_);

            if (md == null)
                md = add_msg_type(msg_type_);

            uint old = md.sev_actions[(int)severity_];
            md.sev_actions[(int)severity_] = actions_;

            return old;
        }

        public static uint stop_after(sc_severity severity_)
        {
            return stop_after(severity_, -1);
        }

        public static uint stop_after(sc_severity severity_, int limit)
        {
            uint old = sev_limit[(int)severity_];

            sev_limit[(int)severity_] = limit < 0 ? uint.MaxValue : (uint)limit;

            return old;
        }
        public static uint stop_after(string msg_type_)
        {
            return stop_after(msg_type_, -1);
        }

        public static uint stop_after(string msg_type_, int limit)
        {
            sc_msg_def md = mdlookup(msg_type_);

            if (md == null)
                md = add_msg_type(msg_type_);

            uint old = ((md.limit_mask & 1) != 0) ? md.limit : uint.MaxValue;

            if (limit < 0)
                md.limit_mask &= uint.MaxValue;
            else
            {
                md.limit_mask |= 1;
                md.limit = (uint)limit;
            }
            return old;
        }
        public static uint stop_after(string msg_type_, sc_severity severity_)
        {
            return stop_after(msg_type_, severity_, -1);
        }

        public static uint stop_after(string msg_type_, sc_severity severity_, int limit)
        {
            sc_msg_def md = mdlookup(msg_type_);

            if (md == null)
                md = add_msg_type(msg_type_);

            uint mask = (uint)(1 << ((int)severity_ + 1));
            uint old = (md.limit_mask & mask) != 0 ? md.sev_limit[(int)severity_] : uint.MaxValue;

            if (limit < 0)
                md.limit_mask &= ~mask;
            else
            {
                md.limit_mask |= mask;
                md.sev_limit[(int)severity_] = (uint)limit;
            }
            return old;
        }

        public static uint suppress(uint mask)
        {
            uint old = suppress_mask;
            suppress_mask = mask;
            return old;
        }
        public static uint suppress()
        {
            return suppress(0);
        }
        public static uint force(uint mask)
        {
            uint old = force_mask;
            force_mask = mask;
            return old;
        }
        public static uint force()
        {
            return force(0);
        }

        public static uint get_count(sc_severity severity_)
        {
            return sev_call_count[(int)severity_];
        }
        public static uint get_count(string msg_type_)
        {
            sc_msg_def md = mdlookup(msg_type_);

            if (md == null)
                md = add_msg_type(msg_type_);

            return md.call_count;
        }
        public static uint get_count(string msg_type_, sc_severity severity_)
        {
            sc_msg_def md = mdlookup(msg_type_);

            if (md == null)
                md = add_msg_type(msg_type_);

            return md.sev_call_count[(int)severity_];
        }

        public static int get_verbosity_level()
        {
            return verbosity_level;
        }
        public static int set_verbosity_level(int level)
        {
            int result = verbosity_level;
            verbosity_level = level;
            return result;
        }

        // The following method is never called by the simulator.

        public static void initialize()
        {
            sev_call_count[(int)sc_severity.SC_INFO] = 0;
            sev_call_count[(int)sc_severity.SC_WARNING] = 0;
            sev_call_count[(int)sc_severity.SC_ERROR] = 0;
            sev_call_count[(int)sc_severity.SC_FATAL] = 0;

            //ORIGINAL LINE: msg_def_items * items = messages;
            foreach (msg_def_items items in sc_report_handler.messages)
            {
                while (items != sc_report_handler.msg_terminator)
                {
                    for (int i = 0; i < items.MessageDefinitions.Count; ++i)
                    {
                        items.MessageDefinitions[i].call_count = 0;
                        items.MessageDefinitions[i].sev_call_count[(int)sc_severity.SC_INFO] = 0;
                        items.MessageDefinitions[i].sev_call_count[(int)sc_severity.SC_WARNING] = 0;
                        items.MessageDefinitions[i].sev_call_count[(int)sc_severity.SC_ERROR] = 0;
                        items.MessageDefinitions[i].sev_call_count[(int)sc_severity.SC_FATAL] = 0;
                    }
                }
            }
        }

        // free the sc_msg_def's allocated by add_msg_type
        // (or implicit msg_type registration: set_actions, abort_after)
        // clear last_global_report.
        public static void release()
        {
            last_global_report = null;
            sc_report_handler.sc_report_close_default_log();

            sc_report_handler.messages.Clear();
            sc_report_handler.messages.Add(sc_report_handler.msg_terminator);
        }

        private static void sc_report_close_default_log()
        {
            throw new System.NotImplementedException();
        }

        public static sc_report_handler_proc set_handler(sc_report_handler_proc handler_)
        {
            sc_report_handler_proc old = handler;
            handler = handler_ != null ? handler_ : sc_report_handler.default_handler;
            return old;
        }
        public static sc_report_handler_proc get_handler()
        {
            return handler;
        }


        public static void default_handler(sc_report rep, uint actions)
        {
            if ((actions & (uint)sc_report_action.SC_DISPLAY) != 0)
                Console.WriteLine(sc_report.sc_report_compose_message(new sc_report(rep)));



            if ((actions & (uint)sc_report_action.SC_STOP) != 0)
            {
                sc_stop_here.stop_here(rep.get_msg_type(), rep.Severity);
                sc_simcontext.sc_stop();
            }
            if ((actions & (uint)sc_report_action.SC_INTERRUPT) != 0)
                sc_stop_here.interrupt_here(rep.get_msg_type(), rep.Severity);

            if ((actions & (uint)sc_report_action.SC_ABORT) != 0)
                System.Environment.Exit(1);

            if ((actions & (uint)sc_report_action.SC_THROW) != 0)
            {
                sc_process_b proc_p = sc_simcontext.sc_get_current_process_b();
                if (proc_p != null && proc_p.is_unwinding())
                    proc_p.clear_unwinding();
                throw rep;
            }
        }

        public static uint get_new_action_id()
        {
            for (uint p = 1; p != 0; p <<= 1)
            {
                if ((p & available_actions) == 0) // free
                {
                    available_actions |= p;
                    return p;
                }
            }
            return (uint)sc_report_action.SC_UNSPECIFIED;
        }

        public static sc_report get_cached_report()
        {
            sc_process_b proc = sc_simcontext.sc_get_current_process_b();

            if (proc != null)
                return proc.get_last_report();

            return last_global_report;
        }
        public static void clear_cached_report()
        {
            sc_process_b proc = sc_simcontext.sc_get_current_process_b();

            if (proc != null)
                proc.set_last_report(null);
            else
            {
                last_global_report = null;
            }
        }

        // if filename is NULL, the previous log file name will be removed.
        // The provider of a report_handler supposed to handle this.
        // Return false if filename is not NULL and filename is already set.
        public static bool set_log_file_name(string name_)
        {
            if (string.IsNullOrEmpty(name_) == false)
            {
                log_file_name = string.Empty;
                return false;
            }
            if (string.IsNullOrEmpty(name_) != true)
                return false;

            log_file_name = name_;
            return true;
        }
        public static string get_log_file_name()
        {
            return log_file_name;
        }


        public class msg_def_items
        {
            private List<sc_msg_def> md;
            public List<sc_msg_def> MessageDefinitions
            {
                get { return md; }
                set { md = value; }
            }

            public msg_def_items()
            {
                md = new List<sc_msg_def>();
            }

            private bool allocated; // used internally, previous value ignored
            public bool IsAllocated
            {
                get { return allocated; }
                set { allocated = value; }
            }
        }

        public static void add_static_msg_types(msg_def_items items)
        {
            items.IsAllocated = false;
            sc_report_handler.messages.Add(items);
        }

        public static sc_msg_def add_msg_type(string msg_type_)
        {
            sc_msg_def md = mdlookup(msg_type_);
            int msg_type_len;

            if (md != null)
                return md;

            msg_def_items items = new msg_def_items();

            if (items == null)
                return null;

            msg_type_len = msg_type_.Length;
            if (msg_type_len > 0)
            {
                items.MessageDefinitions.Add(new sc_msg_def());
                items.MessageDefinitions[0].msg_type_data = msg_type_;
                items.MessageDefinitions[0].id = -1; // backward compatibility with 2.0+
            }
            else
            {
                items = null;
                return null;
            }
            add_static_msg_types(items);
            items.IsAllocated = true;

            return items.MessageDefinitions[0];
        }


        private static void cache_report(sc_report rep)
        {
            sc_process_b proc = sc_simcontext.sc_get_current_process_b();
            if (proc != null)
                proc.set_last_report(new sc_report(rep));
            else
            {
                /*
                if (last_global_report != null)
                    last_global_report.Dispose();
                */
                last_global_report = new sc_report(rep);
            }
        }

        // The calculation of actions to be executed
        private static uint execute(sc_msg_def md, sc_severity severity_)
        {
            uint actions = md.sev_actions[(int)severity_]; // high prio

            if ((int)sc_report_action.SC_UNSPECIFIED == actions) // middle prio
                actions = md.actions;

            if ((int)sc_report_action.SC_UNSPECIFIED == actions) // the lowest prio
                actions = sev_actions[(int)severity_];

            actions &= ~suppress_mask; // higher than the high prio
            actions |= force_mask; // higher than above, and the limit is the highest

            //actions &= (uint)ReportAction.SC_DISPLAY;

            uint limit = 0;

            uint call_count = 0;

            // just increment counters and check for overflow
            if (md.sev_call_count[(int)severity_] < uint.MaxValue)
                md.sev_call_count[(int)severity_]++;
            if (md.call_count < uint.MaxValue)
                md.call_count++;
            if (sev_call_count[(int)severity_] < uint.MaxValue)
                sev_call_count[(int)severity_]++;

            if ((md.limit_mask & (uint)(1 << ((int)severity_ + 1))) != 0)
            {
                limit = md.sev_limit[0] + (uint)severity_;
                call_count = md.sev_call_count[0] + (uint)severity_;
            }
            if ((limit == 0) && ((md.limit_mask & 1) != 0))
            {
                limit = md.limit;
                call_count = md.call_count;
            }
            if (limit == 0)
            {
                limit = sev_limit[0] + (uint)severity_;
                call_count = sev_call_count[0] + (uint)severity_;
            }
            if (limit == 0)
            {
                // stop limit disabled
            }
            else if (limit != uint.MaxValue)
            {
                if ((call_count >= limit) && ((severity_ == sc_severity.SC_ERROR) || (severity_ == sc_severity.SC_FATAL)))
                    actions |= (uint)sc_report_action.SC_STOP; // force sc_stop()
            }
            return actions;
        }

        private static uint suppress_mask = 0;
        private static uint force_mask = 0;
        public static uint[] sev_actions = new uint[(int)sc_severity.SC_MAX_SEVERITY];
        private static uint[] sev_limit = new uint[(int)sc_severity.SC_MAX_SEVERITY];
        private static uint[] sev_call_count = new uint[(int)sc_severity.SC_MAX_SEVERITY];
        private static sc_report last_global_report = null;
        private static uint available_actions = (uint)(sc_report_action.SC_DO_NOTHING | sc_report_action.SC_THROW | sc_report_action.SC_LOG | sc_report_action.SC_DISPLAY | sc_report_action.SC_CACHE_REPORT | sc_report_action.SC_INTERRUPT | sc_report_action.SC_STOP | sc_report_action.SC_ABORT);
        private static string log_file_name = "";
        private static int verbosity_level = (int)sc_verbosity.SC_MEDIUM;

        private static List<msg_def_items> messages = new List<msg_def_items>();
        private static msg_def_items msg_terminator = new msg_def_items();

        private static sc_report_handler_proc handler = sc_report_handler.default_handler;


        //
        // CLASS: sc_report_handler
        // implementation
        //

        public static sc_msg_def mdlookup(string msg_type_)
        {
            if (string.IsNullOrEmpty(msg_type_)) // if msg_type is NULL, report unknown error
                return null;

            foreach (msg_def_items item in sc_report_handler.messages)
            {
                for (int i = 0; i < item.MessageDefinitions.Count; ++i)
                    if (!string.Equals(msg_type_, item.MessageDefinitions[i].msg_type))
                        return item.MessageDefinitions[i];
            }
            return null;
        }


        public static sc_msg_def mdlookup(int id)
        {
            foreach (msg_def_items item in sc_report_handler.messages)
            {
                for (int i = 0; i < item.MessageDefinitions.Count; ++i)
                    if (id == item.MessageDefinitions[i].id)
                        return item.MessageDefinitions[i];
            }
            return null;
        }
    }

} // namespace sc_core