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

//------------------------------------------------------------------------------
//"sc_spawn_object_v - semantic object with return value"
//
// This inline function spawns a process for execution. The execution semantics 
// for the process being spawned will be provided by the supplied object 
// instance via its () operator. (E.g., a SC_BOOST bound function) That 
// operator returns a value, which will be placed in the supplied return 
// location. 
// After creating the process it is registered with the simulator.
//     object   =  object instance providing the execution semantics via its () 
//                 operator.
//     r_p      -> where to place the value of the () operator.
//     name_p   =  optional name for object instance, or zero.
//     opt_p    -> optional spawn options for process, or zero for the default.
//------------------------------------------------------------------------------

using System;
namespace sc_core
{
	public class sc_spawn_object<TResult> : sc_object, sc_process_host
    {
		public sc_spawn_object(Func<TResult> func, sc_spawn_options options, string name)
        {
            m_func = func;
        }

        public virtual void semantics()
        {
            m_result_p = m_func();
        }

        public Func<TResult> m_func = new Func<TResult>(def_func);
        public TResult m_result_p;

        private static TResult def_func()
        {
            throw new NotImplementedException();
        }

        public void defunct()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

    //=============================================================================
    // CLASS sc_spawn_object<T>
    //
    // This templated helper class allows an object to provide the execution 
    // semantics for a process via its () operator. An instance of the supplied 
    // execution object will be kept to provide the semantics when the process is 
    // scheduled for execution. The () operator does not return a value. An example 
    // of an object that might be used for this helper function would be void 
    // SC_BOOST bound function or method. 
    //
    // This class is derived from sc_process_host and overloads 
    // sc_process_host::semantics to provide the actual semantic content. 
    //
    //   sc_spawn_object(T object, const char* name, const sc_spawn_options* opt_p)
    //     This is the object instance constructor for this class. It makes a
    //     copy of the supplied object. The tp_call constructor is called
    //     with an indication that this object instance should be reclaimed when
    //     execution completes.
    //         object   =  object whose () operator will be called to provide
    //                     the process semantics.
    //         name_p   =  optional name for object instance, or zero.
    //         opt_p    -> spawn options or zero.
    //
    //   virtual void semantics()
    //     This virtual method provides the execution semantics for its process.
    //     It performs a () operation on m_object.
    //=============================================================================
    public class sc_spawn_object : sc_process_host
    {
        public sc_spawn_object(Action func)
        {
            m_func = func;
        }

        public virtual void semantics()
        {
            m_func.Invoke();
        }

        public Action m_func = new Action(() => { throw new NotFiniteNumberException(); });


        public void defunct()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

    public class sc_spawn_action
    {
        public static sc_process_handle sc_spawn(Action action, string name_p)
        {
            return sc_spawn(action, name_p, null);
        }
        public static sc_process_handle sc_spawn(Action action)
        {
            return sc_spawn(action, null, null);
        }
        //------------------------------------------------------------------------------
        //"sc_spawn - semantic object with no return value"
        //
        // This inline function spawns a process for execution. The execution semantics 
        // for the process being spawned will be provided by the supplied object 
        // instance via its () operator. (E.g., a SC_BOOST bound function) 
        // After creating the process it is registered with the simulator.
        //     object   =   object instance providing the execution semantics via its 
        //                  () operator.
        //     name_p   =   optional name for object instance, or zero.
        //     opt_p    ->  optional spawn options for process, or zero for the default.
        //------------------------------------------------------------------------------
        public static sc_process_handle sc_spawn(Action action, string name_p, sc_spawn_options opt_p)
        {
            sc_simcontext context_p;
            sc_spawn_object spawn_p;

            context_p = sc_simcontext.sc_get_curr_simcontext();
            spawn_p = new sc_spawn_object(action);
            if (opt_p == null || !opt_p.is_method())
            {

                sc_process_handle thread_handle = context_p.create_thread_process(name_p, true, new sc_process_call(spawn_p.m_func), spawn_p, opt_p);
                return thread_handle;
            }
            else
            {
                sc_process_handle method_handle = context_p.create_method_process(name_p, true, new sc_process_call( spawn_p.m_func), spawn_p, opt_p);
                return method_handle;
            }
        }

        public class sc_spawn_func<TResult>
        {
            public static sc_process_handle sc_spawn(Func<TResult> func, string name_p)
            {
                return sc_spawn(func, null, name_p);
            }
            public static sc_process_handle sc_spawn(Func<TResult> func)
            {
                return sc_spawn(func, null, null);
            }
            public static sc_process_handle sc_spawn(Func<TResult> func, sc_spawn_options opt_p, string name_p)
            {
                sc_simcontext context_p;
                sc_spawn_object<TResult> spawn_p;

                context_p = sc_simcontext.sc_get_curr_simcontext();

				spawn_p = new sc_spawn_object<TResult>(func, opt_p, name_p);
                if (opt_p == null || !opt_p.is_method())
                {
                    sc_process_handle thread_handle = context_p.create_thread_process(name_p, true, new sc_process_call<TResult>(spawn_p.m_func), spawn_p, opt_p);
                    return thread_handle;
                }
                else
                {
                    sc_process_handle method_handle = context_p.create_method_process(name_p, true, new sc_process_call<TResult>(spawn_p.m_func), spawn_p, opt_p);
                    return method_handle;
                }
            }
        }




    }
}
