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

    public class sc_name_gen
    {
        public sc_name_gen()
        {
            m_unique_name = string.Empty;
        }

        public string gen_unique_name(string basename_)
        {
            return gen_unique_name(basename_, false);
        }

        private int counter = 0;

        public string gen_unique_name(string basename_, bool preserve_first)
        {
            if (string.IsNullOrEmpty(basename_))
            {
                sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "cannot generate unique name from null string", "");
            }

            m_unique_name = string.Format("{0}_{1}", basename_, counter);
            counter++;
            return m_unique_name;
        }
        private string m_unique_name = string.Empty;

    }

} // namespace sc_core
