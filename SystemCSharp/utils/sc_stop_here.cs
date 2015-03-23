namespace sc_core
{
    public static class sc_stop_here
    {

        internal static string info_id = "info_id";
        internal static string warning_id = "warning_id";
        internal static string error_id = "error_id";
        internal static string fatal_id = "fatal_id";

        // ----------------------------------------------------------------------------
        //  FUNCTION : sc_interrupt_here
        //
        //  Debugging aid for warning, error, and fatal reports.
        //  This function *cannot* be inlined.
        // ----------------------------------------------------------------------------


        // ----------------------------------------------------------------------------
        //  FUNCTION : sc_interrupt_here
        //
        //  Debugging aid for interrupt warning, error, and fatal reports.
        // ----------------------------------------------------------------------------

        public static void interrupt_here(string id, sc_severity severity)
        {
            // you can set a breakpoint at some of the lines below, either to
            // interrupt with any severity, or to interrupt with a specific severity

            switch (severity)
            {
                case sc_severity.SC_INFO:
                    info_id = id;
                    break;
                case sc_severity.SC_WARNING:
                    warning_id = id;
                    break;
                case sc_severity.SC_ERROR:
                    error_id = id;
                    break;
                default:
                case sc_severity.SC_FATAL:
                    fatal_id = id;
                    break;
            }
        }

        // ----------------------------------------------------------------------------
        //  FUNCTION : sc_stop_here
        //
        //  Debugging aid for warning, error, and fatal reports.
        //  This function *cannot* be inlined.
        // ----------------------------------------------------------------------------



        // ----------------------------------------------------------------------------
        //  FUNCTION : sc_stop_here
        //
        //  Debugging aid for warning, error, and fatal reports.
        // ----------------------------------------------------------------------------

        public static void stop_here(string id, sc_severity severity)
        {
            // you can set a breakpoint at some of the lines below, either to
            // stop with any severity, or to stop with a specific severity

            switch (severity)
            {
                case sc_severity.SC_INFO:
                    info_id = id;
                    break;
                case sc_severity.SC_WARNING:
                    warning_id = id;
                    break;
                case sc_severity.SC_ERROR:
                    error_id = id;
                    break;
                default:
                case sc_severity.SC_FATAL:
                    fatal_id = id;
                    break;
            }
        }


    }
}