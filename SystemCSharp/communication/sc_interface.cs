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
    //  CLASS : sc_interface
    //
    //  Abstract base class of all interface classes.
    //  BEWARE: Direct inheritance from this class must be done virtual.
    // ----------------------------------------------------------------------------

    public interface sc_interface
    {

        // register a port with this interface (does nothing by default)

        // ----------------------------------------------------------------------------
        //  CLASS : sc_interface
        //
        //  Abstract base class of all interface classes.
        //  BEWARE: Direct inheritance from this class must be done virtual.
        // ----------------------------------------------------------------------------

        // register a port with this interface (does nothing by default)

        void register_port(sc_port_base port_, string if_typename_);

        // get the default event

        sc_event default_event();

    }

}