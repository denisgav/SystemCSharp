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

using System.Threading;

namespace sc_core
{
	// ----------------------------------------------------------------------------
	//  CLASS : sc_host_mutex
	//
	//   The sc_host_mutex class, wrapping an OS mutex on the simulation host
	// ----------------------------------------------------------------------------
	public class sc_host_mutex : sc_mutex_if
	{
		// constructors and destructor
		public sc_host_mutex ()
		{
			lock_object = new object ();
		}

		public virtual void Dispose ()
		{
			Monitor.Exit (lock_object);
		}

		// interface methods
		// blocks until mutex could be locked
		public virtual int @lock ()
		{
			Monitor.Enter (lock_object);
			return 0;
		}
		// returns -1 if mutex could not be locked
		public virtual int trylock ()
		{
			return Monitor.TryEnter (lock_object)?1:0;
		}
		// should return -1 if mutex was not locked by caller,
		// but is not yet able to check this
		public virtual int unlock ()
		{
			Monitor.Exit (lock_object);
			return  1;
		}

		private object lock_object;
	}
}
// namespace sc_core
