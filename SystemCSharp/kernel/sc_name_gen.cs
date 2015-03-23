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
