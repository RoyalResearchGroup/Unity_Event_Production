using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class Source : Module
{
    public float creationRate;
    public Resource creationType;

    public override void DetermineState()
    {
        //The states here are simple.
        if (d_event)
        {
            SetSTATE(STATE.OCCUPIED);
        }
        else
        {
            if (resourceBuffer.Count > 0)
            {

                if(resourceBuffer.Count < resourceBuffer.Limit)
                    SetSTATE(STATE.AVAILABLE);
                else
                    SetSTATE(STATE.BLOCKED);
            }
            else
            {
                SetSTATE(STATE.EMPTY);
            }
        }
    }

    public override void DispatchEvent()
    {
        base.DispatchEvent();
        e_manager.EnqueueEvent(new Event(1 / creationRate, this, EVENTTYPE.CREATE));
    }

    public override void EventCallback(Event r_event)
    {
        base.EventCallback(r_event);

        //Create the resource object
        ResourceObject obj = new ResourceObject(creationType);
        AddResource(obj);
    }



    public override void Start()
    {
        base.Start();
        //init resource buffer with one slot
        resourceBuffer = new LimitedQueue<ResourceObject>(1);
    }


    public override void LateUpdate()
    {
        base.LateUpdate();

        //If the buffer is not full
        if(resourceBuffer.Count < resourceBuffer.Limit && GetSTATE()!=STATE.OCCUPIED) {
            //Dispatch the Event to spawn a resource
            DispatchEvent();
            DetermineState();
        }   
    }




    public override void MoveToModule(Module module)
    {
        ResourceObject res = resourceBuffer.Dequeue();
        module.AddResource(res);
        //Update the states on both this and the target module
        DetermineState();
        module.DetermineState();
    }

    public override void UpdateCTRL(Module m)
    {
        //As long as we can find output objects and there are resources present, distribute them
        while(resourceBuffer.Count > 0)
        {
            //The source can only output resources, so no input model needed
            Module mod_out;

            //The current resource type in the buffer
            Resource res_peek = resourceBuffer.Peek().Resource;

            //Get a candidate for output
            mod_out = (Module) OutputCTRL(res_peek);

            if (m != null)
            {
                mod_out = m;
            }

            //There are no candidates, so break the loop and return.
            if (mod_out == null) 
            {
                //Determine state before leaving (likely blocked)
                DetermineState();
                return;
            }

            //Otherwise, we can move the resource
            MoveToModule(mod_out);
            mod_out.UpdateCTRL();

            m = null;
        }
        //Dispatch a new Event creation
        if(resourceBuffer.Count < resourceBuffer.Limit)
        {
            DispatchEvent();
            DetermineState();
        }
    }

    //Override the Gizmo color:
    // Visualize connections in editor mode
    void OnDrawGizmos()
    {
        if (connectedObjects != null)
        {
            foreach (Module module in connectedObjects)
            {
                if (module != null)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(transform.position, module.transform.position);
                }
            }
        }
    }

    public override bool IsInputReady(Resource r)
    {
        //Never takes anything in
        return false;
    }

    public override bool IsOutputReady(List<Resource> r)
    {
        //Output readiness is based on the current state. If there are items in the buffer, they can be drawn
        if(resourceBuffer.Count>0 && r.Contains(resourceBuffer.Peek().Resource))
        {
            return true;
        }
        return false;
    }
}


