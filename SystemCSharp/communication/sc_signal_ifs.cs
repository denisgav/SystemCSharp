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

    public abstract class sc_signal_in_if_param<T> : sc_interface
    {

        public abstract sc_event value_changed_event();


        public abstract T read();

        public abstract T get_data_ref();


        // was there a value changed event?
        public abstract bool Event();


        // constructor

        protected sc_signal_in_if_param()
        {
        }


        public virtual void register_port(sc_port_base port_, string if_typename_)
        {
            throw new System.NotImplementedException();
        }

        public virtual sc_event default_event()
        {
            throw new System.NotImplementedException();
        }
    }


    // ----------------------------------------------------------------------------
    //  CLASS : sc_signal_in_if<bool>
    //
    //  Specialization of sc_signal_in_if<T> for type bool.
    // ----------------------------------------------------------------------------

    public abstract class sc_signal_in_if : sc_interface
    {

        // get the value changed event
        public abstract sc_event value_changed_event();

        // get the positive edge event
        public abstract sc_event posedge_event();

        // get the negative edge event
        public abstract sc_event negedge_event();


        // read the current value
        public abstract bool read();

        // get a reference to the current value (for tracing)
        public abstract bool get_data_ref();


        // was there a value changed event?
        public abstract bool Event();

        // was there a positive edge event?
        public abstract bool posedge();

        // was there a negative edge event?
        public abstract bool negedge();


        // constructor

        protected sc_signal_in_if()
        {
        }


        // designate this object as a reset signal
        public virtual sc_reset is_reset()
        {
            return null;
        }

        public abstract void register_port(sc_port_base port_, string if_typename_);

        public abstract sc_event default_event();
    }



    // ----------------------------------------------------------------------------
    //  CLASS : sc_signal_write_if<T>
    //
    //  The standard output interface class.
    // ----------------------------------------------------------------------------
    public abstract class sc_signal_write_if<T> : sc_interface
    {
        public sc_signal_write_if()
        {
        }
        // write the new value
        public abstract void write(T NamelessParameter);

        public virtual sc_writer_policy get_writer_policy()
        {
            return sc_writer_policy.SC_UNCHECKED_WRITERS;
        }

        public abstract void register_port(sc_port_base port_, string if_typename_);

        public abstract sc_event default_event();
    }


    // ----------------------------------------------------------------------------
    //  CLASS : sc_signal_inout_if<T>
    //
    //  The sc_signal<T> input/output interface class.
    // ----------------------------------------------------------------------------
    /*
    public class sc_signal_inout_if<T> : sc_signal_in_if_param<T>, sc_signal_write_if<T>
    {


        // constructor

        protected sc_signal_inout_if()
        {
        }



    }
    */

} // namespace sc_core

