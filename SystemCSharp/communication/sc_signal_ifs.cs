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
    //  CLASS : sc_signal_in_if<T>
    //
    //  The sc_signal<T> input interface class.
    // ----------------------------------------------------------------------------

    public interface sc_signal_in_if<T> : sc_interface
    {
        sc_event value_changed_event();

        T read();

        T get_data_ref();

        // was there a value changed event?
        bool Event();

        sc_reset is_reset();
    }


    // ----------------------------------------------------------------------------
    //  CLASS : sc_signal_in_if<bool>
    //
    //  Specialization of sc_signal_in_if<T> for type bool.
    // ----------------------------------------------------------------------------

    public interface sc_signal_in_if : sc_interface
    {

        // get the value changed event
        sc_event value_changed_event();

        // get the positive edge event
        sc_event posedge_event();

        // get the negative edge event
        sc_event negedge_event();


        // read the current value
        bool read();

        // get a reference to the current value (for tracing)
        bool get_data_ref();


        // was there a value changed event?
        bool Event();

        // was there a positive edge event?
        bool posedge();

        // was there a negative edge event?
        bool negedge();

        // designate this object as a reset signal
        sc_reset is_reset();
    }



    // ----------------------------------------------------------------------------
    //  CLASS : sc_signal_write_if<T>
    //
    //  The standard output interface class.
    // ----------------------------------------------------------------------------
    public interface sc_signal_write_if<T> : sc_interface
    {
        // write the new value
        void write(T value);

        sc_writer_policy get_writer_policy();
    }


    // ----------------------------------------------------------------------------
    //  CLASS : sc_signal_inout_if<T>
    //
    //  The sc_signal<T> input/output interface class.
    // ----------------------------------------------------------------------------

    public interface sc_signal_inout_if<T> : sc_signal_in_if<T>, sc_signal_write_if<T>
    {
    }


} // namespace sc_core