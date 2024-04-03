using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buffer : Module
{
    //Important: the capacity is global. This means a capacity of 1 means that only one resource can be present at once. when the out buffer is full, theres no space in the in_buffer
    [SerializeField]
    public int capacity = 1;

    //Current accepting states
    public List<Resource> allowedResources = new List<Resource>();

    public override void DetermineState()
    {
        if (resourceBuffer.Count > 0)
        {
            if (resourceBuffer.Count < resourceBuffer.Limit)
                SetSTATE(STATE.AVAILABLE);
            else
                SetSTATE(STATE.BLOCKED);
        }
        else
        {
            SetSTATE(STATE.EMPTY);
        }
    }


    public override bool IsInputReady(Resource r)
    {
        //Input always possible when space
        if (resourceBuffer.Count < resourceBuffer.Limit && allowedResources.Contains(r)) return true;
        return false;
    }

    public override bool IsOutputReady(List<Resource> r)
    {
        if (resourceBuffer.Count > 0 && r.Contains(resourceBuffer.Peek().Resource))
        {
            return true;
        }
        return false;
    }

    public override void MoveToModule(Module module)
    {
        ResourceObject res = resourceBuffer.Dequeue();
        module.AddResource(res);
        //Update the states on both this and the target module
        DetermineState();
        module.DetermineState();
    }

    public override void Start()
    {
        base.Start();
        resourceBuffer = new LimitedQueue<ResourceObject>(capacity);
    }


    public override void UpdateCTRL()
    {
        while (resourceBuffer.Count > 0)
        {
            //The source can only output resources, so no input model needed
            Module mod_out;

            //The current resource type in the buffer
            Resource res_peek = resourceBuffer.Peek().Resource;

            //Get a candidate for output
            mod_out = (Module)OutputCTRL(res_peek);
            //There are no candidates, so break the loop and return.
            if (mod_out == null) return;

            //Otherwise, we can move the resource
            MoveToModule(mod_out);
            mod_out.UpdateCTRL();


            //Check if there is an aviable input machine that could provide a new resource
            Module mod_in;

            mod_in = (Module)InputCTRL(allowedResources);
            if (mod_in != null) {
                mod_in.UpdateCTRL();
            }
        }
    }
}
