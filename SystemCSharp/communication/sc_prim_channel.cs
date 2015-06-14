using System.Collections.Generic;

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

//****************************************************************************
//
//  sc_prim_channel.cpp -- Abstract base class of all primitive channel
//                         classes.
//
//  Original Author: Martin Janssen, Synopsys, Inc., 2001-05-21
//
//  CHANGE LOG IS AT THE END OF THE FILE
// ****************************************************************************

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

//****************************************************************************
//
//  sc_prim_channel.h -- Abstract base class of all primitive channel classes.
//
//  Original Author: Martin Janssen, Synopsys, Inc., 2001-05-21
//
// CHANGE LOG AT THE END OF THE FILE
// ****************************************************************************



namespace sc_core
{

    // ----------------------------------------------------------------------------
    //  CLASS : sc_prim_channel
    //
    //  Abstract base class of all primitive channel classes.
    // ----------------------------------------------------------------------------

    public class sc_prim_channel : sc_object
    {
        public const sc_prim_channel list_end = null;
        public override string kind()
        {
            return "sc_prim_channel";
        }

        public bool update_requested()
        {
            return m_update_next_p != list_end;
        }

        // request the update method to be executed during the update phase

        // ----------------------------------------------------------------------------
        //  CLASS : sc_prim_channel
        //
        //  Abstract base class of all primitive channel classes.
        // ----------------------------------------------------------------------------

        // request the update method (to be executed during the update phase)

        public void request_update()
        {
            if (m_update_next_p == null)
            {
                m_registry.request_update(this);
            }
        }

        // request the update method to be executed during the update phase
        // from a process external to the simulator.

        // request the update method from external to the simulator (to be executed 
        // during the update phase)

        public void async_request_update()
        {
            m_registry.async_request_update(this);
        }


        // constructors

        // ----------------------------------------------------------------------------
        //  CLASS : sc_prim_channel
        //
        //  Abstract base class of all primitive channel classes.
        // ----------------------------------------------------------------------------

        // constructors

        protected sc_prim_channel()
            : base()
        {
            m_registry = simcontext().get_prim_channel_registry();
            m_update_next_p = null;
            m_registry.insert(this);
        }
        protected sc_prim_channel(string name_)
            : base(name_)
        {
            m_registry = simcontext().get_prim_channel_registry();
            m_update_next_p = null;
            m_registry.insert(this);
        }

        // destructor

        // destructor

        public override void Dispose()
        {
            m_registry.remove(this);
            base.Dispose();
        }

        // the update method (does nothing by default)

        // the update method (does nothing by default)

        protected virtual void update()
        {
        }

        // called by construction_done (does nothing by default)

        // called by construction_done (does nothing by default)

        protected virtual void before_end_of_elaboration()
        {
        }

        // called by elaboration_done (does nothing by default)

        // called by elaboration_done (does nothing by default)

        protected virtual void end_of_elaboration()
        {
        }

        // called by start_simulation (does nothing by default)

        // called by start_simulation (does nothing)

        protected virtual void start_of_simulation()
        {
        }

        // called by simulation_done (does nothing by default)

        // called by simulation_done (does nothing)

        protected virtual void end_of_simulation()
        {
        }


        // to avoid calling sc_get_curr_simcontext()

        // static sensitivity for SC_THREADs and SC_CTHREADs

        protected void wait()
        {
            sc_wait.wait(simcontext());
        }


        // dynamic sensitivity for SC_THREADs and SC_CTHREADs

        protected void wait(sc_event e)
        {
            sc_wait.wait(e, simcontext());
        }

        protected void wait(sc_event_or_list el)
        {
            sc_wait.wait(el, simcontext());
        }

        protected void wait(sc_event_and_list el)
        {
            sc_wait.wait(el, simcontext());
        }

        protected void wait(sc_time t)
        {
            sc_wait.wait(new sc_time(t), simcontext());
        }

        protected void wait(double v, sc_time_unit tu)
        {
            sc_wait.wait(new sc_time(v, tu, simcontext()), simcontext());
        }

        protected void wait(sc_time t, sc_event e)
        {
            sc_wait.wait(new sc_time(t), e, simcontext());
        }

        protected void wait(double v, sc_time_unit tu, sc_event e)
        {
            sc_wait.wait(new sc_time(v, tu, simcontext()), e, simcontext());
        }

        protected void wait(sc_time t, sc_event_or_list el)
        {
            sc_wait.wait(new sc_time(t), el, simcontext());
        }

        protected void wait(double v, sc_time_unit tu, sc_event_or_list el)
        {
            sc_wait.wait(new sc_time(v, tu, simcontext()), el, simcontext());
        }

        protected void wait(sc_time t, sc_event_and_list el)
        {
            sc_wait.wait(new sc_time(t), el, simcontext());
        }

        protected void wait(double v, sc_time_unit tu, sc_event_and_list el)
        {
            sc_wait.wait(new sc_time(v, tu, simcontext()), el, simcontext());
        }

        protected void wait(int n)
        {
            sc_wait.wait(n, sc_time_unit.SC_FS, simcontext());
        }


        // static sensitivity for SC_METHODs

        protected void next_trigger()
        {
            sc_wait.next_trigger(simcontext());
        }


        // dynamic sensitivity for SC_METHODs

        protected void next_trigger(sc_event e)
        {
            sc_wait.next_trigger(e, simcontext());
        }

        protected void next_trigger(sc_event_or_list el)
        {
            sc_wait.next_trigger(el, simcontext());
        }

        protected void next_trigger(sc_event_and_list el)
        {
            sc_wait.next_trigger(el, simcontext());
        }

        protected void next_trigger(sc_time t)
        {
            sc_wait.next_trigger(new sc_time(t), simcontext());
        }

        protected void next_trigger(double v, sc_time_unit tu)
        {
            sc_wait.next_trigger(new sc_time(v, tu, simcontext()), simcontext());
        }

        protected void next_trigger(sc_time t, sc_event e)
        {
            sc_wait.next_trigger(new sc_time(t), e, simcontext());
        }

        protected void next_trigger(double v, sc_time_unit tu, sc_event e)
        {
            sc_wait.next_trigger(new sc_time(v, tu, simcontext()), e, simcontext());
        }

        protected void next_trigger(sc_time t, sc_event_or_list el)
        {
            sc_wait.next_trigger(new sc_time(t), el, simcontext());
        }

        protected void next_trigger(double v, sc_time_unit tu, sc_event_or_list el)
        {
            sc_wait.next_trigger(new sc_time(v, tu, simcontext()), el, simcontext());
        }

        protected void next_trigger(sc_time t, sc_event_and_list el)
        {
            sc_wait.next_trigger(new sc_time(t), el, simcontext());
        }

        protected void next_trigger(double v, sc_time_unit tu, sc_event_and_list el)
        {
            sc_wait.next_trigger(new sc_time(v, tu, simcontext()), el, simcontext());
        }


        // for SC_METHODs and SC_THREADs and SC_CTHREADs

        public bool timed_out()
        {
            return sc_wait.timed_out(simcontext());
        }




        // called during the update phase of a delta cycle (if requested)

        // called during the update phase of a delta cycle (if requested)

        public void perform_update()
        {
            update();
            m_update_next_p = null;
        }

        // called when construction is done

        // called when construction is done

        public void construction_done()
        {
            sc_object.hierarchy_scope scope = new sc_object.hierarchy_scope(get_parent_object());
            before_end_of_elaboration();
        }

        // called when elaboration is done

        // called when elaboration is done

        public void elaboration_done()
        {
            sc_object.hierarchy_scope scope = new sc_object.hierarchy_scope(get_parent_object());
            end_of_elaboration();
        }

        // called before simulation starts

        // called before simulation begins

        public void start_simulation()
        {
            sc_object.hierarchy_scope scope = new sc_object.hierarchy_scope(get_parent_object());
            start_of_simulation();
        }

        // called after simulation ends

        // called after simulation ends

        public void simulation_done()
        {
            sc_object.hierarchy_scope scope = new sc_object.hierarchy_scope(get_parent_object());
            end_of_simulation();
        }

        public sc_prim_channel_registry m_registry; // Update list manager.
        public sc_prim_channel m_update_next_p; // Next entry in update list.
    }


    


} // namespace sc_core
