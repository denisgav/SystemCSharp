//****************************************************************************
//
//  The following code is derived, directly or indirectly, from the SystemC
//  source code Copyright (c) 1996-2014 by all Contributors.
//  All Rights reserved.
//
//  The contents of this file are subject to the restrictions and limitations
//  set forth in the SystemC Open Source License (the "License");
//  You may not use this file except in compliance with such restrictions and
//  limitations. You may obtain instructions on how to receive a copy of the
//  License at http://www.accellera.org/. Software distributed by Contributors
//  under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF
//  ANY KIND, either express or implied. See the License for the specific
//  language governing rights and limitations under the License.
//
// ****************************************************************************


namespace sc_core
{

    // ----------------------------------------------------------------------------
    //  CLASS : sc_buffer<T>
    //
    //  The sc_buffer<T> primitive channel class.
    // ----------------------------------------------------------------------------

    public class sc_buffer<T> : sc_signal<T>
    {

        // typedefs

        public sc_buffer(sc_writer_policy writer_policy_)
            : base(writer_policy_)
        {
        }

        public sc_buffer(string name_, sc_writer_policy writer_policy_)
            : base(name_, writer_policy_)
        {
        }

        public sc_buffer(string name_, T initial_value_, sc_writer_policy writer_policy_)
            : base(name_, initial_value_, writer_policy_)
        {
        }

        // interface methods


        // other methods

        public override sc_signal<T> CopyFrom(T a)
        {
            write(a);
            return this;
        }

        public override sc_signal<T> CopyFrom(sc_signal_in_if_param<T> a)
        {
            write(a.read());
            return this;
        }


        public override string kind()
        {
            return "sc_buffer";
        }

        public override void write(T value_)
        {
            if (!writer_policy_check.check_write(this, true))
                return;

            this.m_new_val = value_;
            this.request_update();
        }

        public override void update()
        {
            writer_policy_check.update();
            do_update();
        }


    }
}