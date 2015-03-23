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
    public enum ReportAction
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

        public string get_msg()
        {
            return msg;
        }

        public sc_severity get_severity()
        {
            return severity;
        }

        public string get_file_name()
        {
            return file;
        }

        public int get_line_number()
        {
            return line;
        }

        public sc_time get_time()
        {
            return timestamp;
        }

        public int get_verbosity()
        {
            return m_verbosity_level;
        }

        public bool valid()
        {
            return process != null;
        }

        public virtual string what()
        {
            return m_what;
        }

        public virtual string get_process_name()
        {
            return (process != null)?process.name():string.Empty;
        }

        protected sc_severity severity;
        protected readonly sc_msg_def md;
        protected string msg;
        protected string file;
        protected int line;
        protected sc_time timestamp;
        protected sc_object process;
        protected int m_verbosity_level;
        protected string m_what;

        public sc_report()
        {
            severity = sc_severity.SC_INFO;
            md = null;
            msg = string.Empty;
            file = string.Empty;
            line = 0;
            timestamp = new sc_time();
            process = null;
            m_verbosity_level = (int)sc_verbosity.SC_MEDIUM;
            m_what = string.Empty;
        }
        public sc_report(sc_severity severity_, sc_msg_def md_, string msg_, int verbosity_level = 0, string file_ = "", int line_=0)
        {
            severity = severity_;
            md = md_;
            msg = msg_;
            file = file_;
            line = line_;
            timestamp = new sc_time();
            process = null;
            m_verbosity_level = verbosity_level;
            m_what = sc_report_compose_message(this);
        }
        public sc_report(sc_report other)
        {
            severity = other.severity;
            md = other.md;
            msg = other.msg;
            file = other.file;
            line = other.line;
            timestamp = new sc_time(other.timestamp);
            process = other.process;
            m_verbosity_level = other.m_verbosity_level;
            m_what = other.m_what;
        }

        public static string sc_report_compose_message(sc_report rep)
        {
            StringBuilder res = new StringBuilder();
            

            res.AppendFormat("{0} : ", System.Enum.GetName((rep.severity).GetType(), rep.severity));


            if (rep.get_id() >= 0) // backward compatibility with 2.0+
            {
                res.AppendFormat("id:{0}", rep.get_id());
            }
            res.AppendFormat("{0} ", rep.get_msg_type());

            string msg = rep.get_msg();
            if (string.IsNullOrEmpty(msg) == false)
            {
                res.AppendFormat(": {0} ", msg);
            }
            string what = rep.m_what;
            if (string.IsNullOrEmpty(what) == false)
            {
                res.AppendFormat(": {0} ", what);
            }
            if (rep.get_severity() > sc_severity.SC_INFO)
            {
                res.AppendFormat("\nIn file: {0} : {1}\nIn process:{2}", rep.get_file_name(), rep.get_line_number(), rep.get_process_name());
            }

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

            return md != null ? md.actions == (int)ReportAction.SC_DO_NOTHING : false; // only do-nothing set
        }
        public void suppress_id(int id_, bool suppress)
        {
            sc_msg_def md = sc_report_handler.mdlookup(id_);

            if (md != null)
                md.actions = suppress ? (uint)ReportAction.SC_DO_NOTHING : (uint)ReportAction.SC_UNSPECIFIED;
        }
        public void suppress_infos(bool suppress)
        {
            sc_report_handler.sev_actions[(int)sc_severity.SC_INFO] = suppress ? (uint)ReportAction.SC_DO_NOTHING : (uint)ReportAction.SC_LOG | (uint)ReportAction.SC_DISPLAY;
        }
        public void suppress_warnings(bool suppress)
        {
            sc_report_handler.sev_actions[(int)sc_severity.SC_WARNING] = suppress ? (uint)ReportAction.SC_DO_NOTHING : (uint)ReportAction.SC_LOG | (uint)ReportAction.SC_DISPLAY;
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