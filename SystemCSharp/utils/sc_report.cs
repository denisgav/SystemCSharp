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


using System.Text;
using System;
namespace sc_core
{

    // ----------------------------------------------------------------------------
    //  ENUM : sc_severity
    //
    //  Enumeration of possible exception severity levels
    // ----------------------------------------------------------------------------

    public enum sc_severity
    {
        SC_INFO = 0, // informative only
        SC_WARNING, // indicates potentially incorrect condition
        SC_ERROR, // indicates a definite problem
        SC_FATAL, // indicates a problem from which we cannot recover
        SC_MAX_SEVERITY
    }


    // ----------------------------------------------------------------------------
    //  ENUM : sc_verbosity
    //
    //  Enumeration of message verbosity.
    // ----------------------------------------------------------------------------

    public enum sc_verbosity
    {
        SC_NONE = 0,
        SC_LOW = 100,
        SC_MEDIUM = 200,
        SC_HIGH = 300,
        SC_FULL = 400,
        SC_DEBUG = 500
    }

    // ----------------------------------------------------------------------------
    //  ENUM : 
    //
    //  Enumeration of actions on an exception (implementation specific)
    // ----------------------------------------------------------------------------
    public enum sc_report_action
    {
        SC_UNSPECIFIED = 0x0000, // look for lower-priority rule
        SC_DO_NOTHING = 0x0001, // take no action (ignore if other bits set)
        SC_THROW = 0x0002, // throw an exception
        SC_LOG = 0x0004, // add report to report log
        SC_DISPLAY = 0x0008, // display report to screen
        SC_CACHE_REPORT = 0x0010, // save report to cache
        SC_INTERRUPT = 0x0020, // call sc_interrupt_here(...)
        SC_STOP = 0x0040, // call sc_stop()
        SC_ABORT = 0x0080 // call abort()
    }

    public class sc_report : Exception
    {
        private string msg;
        public override string Message
        {
            get { return msg; }
        }

        private sc_severity severity;
        public virtual sc_severity Severity
        {
            get { return severity; }
            set { severity = value; }
        }

        private sc_msg_def md;
        public virtual sc_msg_def MsgDef
        {
            get { return md; }
            set { md = value; }
        }

        private string file;
        public virtual string File
        {
            get { return file; }
            set { file = value; }
        }

        private string member;
        public virtual string Member
        {
            get { return member; }
            set { member = value; }
        }

        private int line;
        public virtual int Line
        {
            get { return line; }
            set { line = value; }
        }

        private sc_time timeStamp;
        public virtual sc_time TimeStamp
        {
            get { return timeStamp; }
            set { timeStamp = value; }
        }

        private sc_object sender;
        public virtual sc_object Sender
        {
            get { return sender; }
            set { sender = value; }
        }

        public bool Valid
        {
            get
            {
                return sender != null;
            }
        }

        public virtual string SenderName
        {
            get
            {
                return (sender != null) ? sender.name() : string.Empty;
            }
        }

        private int mVerbosityLevel;
        public virtual int VerbosityLevel
        {
            get { return mVerbosityLevel; }
            set { mVerbosityLevel = value; }
        }

        private string mWhat;
        public virtual string What
        {
            get { return mWhat; }
            set { mWhat = value; }
        }

        public sc_report()
        {
            severity = sc_severity.SC_INFO;
            md = null;
            msg = string.Empty;
            file = string.Empty;
            member = string.Empty;
            line = 0;
            timeStamp = new sc_time();
            sender = null;
            mVerbosityLevel = (int)sc_verbosity.SC_MEDIUM;
            mWhat = string.Empty;
        }
        public sc_report(sc_severity severity_, sc_msg_def md_, string msg_, int verbosity_level = 0, string file_ = "", int line_ = 0, string member = "")
        {
            severity = severity_;
            md = md_;
            msg = msg_;
            file = file_;
            line = line_;
            this.member = member;
            timeStamp = new sc_time();
            sender = null;
            mVerbosityLevel = verbosity_level;
            mWhat = sc_report_compose_message(this);
        }
        public sc_report(sc_report other)
        {
            severity = other.severity;
            md = other.md;
            msg = other.msg;
            file = other.file;
            line = other.line;
            member = other.member;
            timeStamp = new sc_time(other.timeStamp);
            sender = other.sender;
            mVerbosityLevel = other.mVerbosityLevel;
            mWhat = other.mWhat;
        }

        public static string sc_report_compose_message(sc_report rep)
        {
            StringBuilder res = new StringBuilder();


            res.AppendFormat("{0} : ", System.Enum.GetName((rep.severity).GetType(), rep.severity));


            if (rep.get_id() >= 0) // backward compatibility with 2.0+
            {
                res.AppendFormat("id:{0}", rep.get_id());
            }
            res.AppendFormat("{0}: {1}: {2}:", rep.File, rep.Line, rep.Member);

            res.AppendFormat("{0} ", rep.get_msg_type());

            string msg = rep.Message;
            if (string.IsNullOrEmpty(msg) == false)
            {
                res.AppendFormat(": {0} ", msg);
            }
            if (rep.Severity > sc_severity.SC_INFO)
            {
                res.AppendFormat("\nIn process:{0}", rep.SenderName);
            }
            res.AppendLine();

            return res.ToString();
        }

        public string get_msg_type()
        {
            return md.msg_type;
        }

        public void register_id(int id, string msg)
        {
            if (id < 0)
            {
                sc_core.sc_report_handler.report(sc_core.sc_severity.SC_ERROR, sc_report_handler.SC_ID_REGISTER_ID_FAILED_, "invalid report id");
            }
            if (string.IsNullOrEmpty(msg))
            {
                sc_core.sc_report_handler.report(sc_core.sc_severity.SC_ERROR, sc_report_handler.SC_ID_REGISTER_ID_FAILED_, "invalid report message");
            }
            sc_msg_def md = sc_report_handler.mdlookup(id);

            if (md == null)
                md = sc_report_handler.add_msg_type(msg);

            if (md == null)
            {
                sc_core.sc_report_handler.report(sc_core.sc_severity.SC_ERROR, sc_report_handler.SC_ID_REGISTER_ID_FAILED_, "report_map insertion error");
            }

            if (md.id != -1)
            {
                if (string.Compare(msg, md.msg_type) != 0)
                {
                    sc_core.sc_report_handler.report(sc_core.sc_severity.SC_ERROR, sc_report_handler.SC_ID_REGISTER_ID_FAILED_, "report id already exists");
                }
                return;
            }
            md.id = id;
        }

        public string get_message(int id)
        {
            sc_msg_def md = sc_report_handler.mdlookup(id);

            return md != null ? md.msg_type : "unknown_id";
        }
        public bool is_suppressed(int id)
        {
            sc_msg_def md = sc_report_handler.mdlookup(id);

            return md != null ? md.actions == (int)sc_report_action.SC_DO_NOTHING : false; // only do-nothing set
        }
        public void suppress_id(int id_, bool suppress)
        {
            sc_msg_def md = sc_report_handler.mdlookup(id_);

            if (md != null)
                md.actions = suppress ? (uint)sc_report_action.SC_DO_NOTHING : (uint)sc_report_action.SC_UNSPECIFIED;
        }
        public void suppress_infos(bool suppress)
        {
            sc_report_handler.sev_actions[(int)sc_severity.SC_INFO] = suppress ? (uint)sc_report_action.SC_DO_NOTHING : (uint)sc_report_action.SC_LOG | (uint)sc_report_action.SC_DISPLAY;
        }
        public void suppress_warnings(bool suppress)
        {
            sc_report_handler.sev_actions[(int)sc_severity.SC_WARNING] = suppress ? (uint)sc_report_action.SC_DO_NOTHING : (uint)sc_report_action.SC_LOG | (uint)sc_report_action.SC_DISPLAY;
        }
        public void make_warnings_errors(bool flag)
        {
            warnings_are_errors = flag;
        }
        public int get_id()
        {
            return md.id;
        }

        public static bool warnings_are_errors = false;
    }
}