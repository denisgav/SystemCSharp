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


using System;
using System.Text;
namespace sc_core
{
    // ----------------------------------------------------------------------------
    //  CLASS : sc_in<T>
    //
    //  The sc_signal<T> input port class.
    // ----------------------------------------------------------------------------

    public class sc_in<T> : sc_port<sc_signal_in_if<T>>
    {
        // constructors

        public sc_in(int max_size_)
            : this(max_size_, sc_port_policy.SC_ONE_OR_MORE_BOUND)
        {
        }

		public sc_in(int max_size_, sc_port_policy policy)
            : base(max_size_, policy)
        {
        }

		public sc_in(string name_, int max_size_)
            : this(name_, max_size_, sc_port_policy.SC_ONE_OR_MORE_BOUND)
        {
        }

		public sc_in(string name_, int max_size_, sc_port_policy policy)
            : base(name_, max_size_, policy)
        {
        }
    }

    // ----------------------------------------------------------------------------
    //  CLASS : sc_inout<T>
    //
    //  The sc_signal<T> input port class.
    // ----------------------------------------------------------------------------

    public class sc_inout<T>
    : sc_port<sc_signal_inout_if<T>>
    {
        // constructors

		public sc_inout(int max_size_)
            : this(max_size_, sc_port_policy.SC_ONE_OR_MORE_BOUND)
        {
        }

		public sc_inout(int max_size_, sc_port_policy policy)
            : base(max_size_, policy)
        {
        }

		public sc_inout(string name_, int max_size_)
            : this(name_, max_size_, sc_port_policy.SC_ONE_OR_MORE_BOUND)
        {
        }

		public sc_inout(string name_, int max_size_, sc_port_policy policy)
            : base(name_, max_size_, policy)
        {
        }
    }

    // ----------------------------------------------------------------------------
    //  CLASS : sc_out<T>
    //
    //  The sc_signal<T> input port class.
    // ----------------------------------------------------------------------------

    public class sc_out<T>
    : sc_inout<T>
    {
        // constructors

		public sc_out(int max_size_)
            : this(max_size_, sc_port_policy.SC_ONE_OR_MORE_BOUND)
        {
        }

		public sc_out(int max_size_, sc_port_policy policy)
            : base(max_size_, policy)
        {
        }

		public sc_out(string name_, int max_size_)
            : this(name_, max_size_, sc_port_policy.SC_ONE_OR_MORE_BOUND)
        {
        }

		public sc_out(string name_, int max_size_, sc_port_policy policy)
            : base(name_, max_size_, policy)
        {
        }
    }
}