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