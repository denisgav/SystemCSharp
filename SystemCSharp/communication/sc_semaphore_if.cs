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
    //  CLASS : sc_semaphore_if
    //
    //  The sc_semaphore_if interface class.
    // ----------------------------------------------------------------------------

    public interface sc_semaphore_if : sc_interface
    {

        // the classical operations: wait(), trywait(), and post()

        // lock (take) the semaphore, block if not available
        int wait();

        // lock (take) the semaphore, return -1 if not available
        int trywait();

        // unlock (give) the semaphore
        int post();

        // get the value of the semphore
        int get_value();
    }

}
