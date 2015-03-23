//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

//TODO:uncomment section below (add to sc_value_base)
/*
private virtual void concat_set(sc_signed src, int low_i)
{
    sc_core.sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "operation failed", "concat_set(sc_signed) method not supported by this type");
}
private virtual void concat_set(sc_unsigned src, int low_i)
{
    sc_core.sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "operation failed", "concat_set(sc_unsigned) method not supported by this type");
}
*/

//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
namespace sc_dt
{

    public class sc_generic_base<T>
    {

    }


    public class sc_value_base
    {
        public virtual void concat_clear_data()
        {
            concat_clear_data(false);
        }

        protected virtual void concat_clear_data(bool to_ones)
        {
            sc_core.sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "operation failed", "concat_clear_data method not supported by this type");
        }

        protected virtual bool concat_get_ctrl(ref uint dst_p, int low_i)
        {
            sc_core.sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "operation failed", "concat_get_ctrl method not supported by this type");
            return false;
        }

        protected virtual bool concat_get_data(ref uint dst_p, int low_i)
        {
            sc_core.sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "operation failed", "concat_get_data method not supported by this type");
            return false;
        }

        protected virtual ulong concat_get_uint64()
        {
            sc_core.sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "operation failed", "concat_get_uint64 method not supported by this type");
            return 0;
        }
        protected virtual int concat_length()
        {
            return concat_length(false);
        }


        protected virtual int concat_length(bool xz_present_p)
        {
            sc_core.sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "operation failed", "concat_length method not supported by this type");
            return 0;
        }
        protected virtual void concat_set(long src, int low_i)
        {
            sc_core.sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "operation failed", "concat_set(int64) method not supported by this type");
        }

        protected virtual void concat_set(uint @long, int low_i)
        {
            sc_core.sc_report_handler.report(sc_core.sc_severity.SC_ERROR, "operation failed", "concat_set(uint64) method not supported by this type");
        }
    }
}