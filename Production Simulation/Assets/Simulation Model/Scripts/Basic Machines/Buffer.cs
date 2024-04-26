using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Buffer : Module
{
    //Important: the capacity is global. This means a capacity of 1 means that only one resource can be present at once. when the out buffer is full, theres no space in the in_buffer
    [SerializeField]
    public int capacity = 1;

    //Current accepting states
    public List<Resource> allowedResources = new List<Resource>();


    //Debug/Stats:
    private int absoluteFill = 0;


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
        if(r == null) return false;
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


    public override void UpdateCTRL(Module m)
    {
        bool action = true;
        while (resourceBuffer.Count > 0 && action)
        {
            bool action_in = true;
            bool action_out = true;
            //The source can only output resources, so no input model needed
            Module mod_out;

            //The current resource type in the buffer
            Resource res_peek = resourceBuffer.Peek().Resource;

            //Get a candidate for output
            mod_out = m != null ? m : (Module)OutputCTRL(res_peek);

            //If there no candidate, the out action failed
            if (mod_out == null)
            {
                DetermineState();
                action_out  = false;
            }
            else
            {
                //Otherwise, we can move the resource
                MoveToModule(mod_out);
                mod_out.UpdateCTRL();
            }


            //Check if there is an aviable input machine that could provide a new resource
            Module mod_in;

            mod_in = (Module)InputCTRL(allowedResources);

            //Same case for the input
            if (mod_in == null) {
                DetermineState();
                action_in = false;
            }
            else
            {
                mod_in.UpdateCTRL(gameObject.GetComponent<Module>());
            }

            //If neither one of the actions was successful, break the loop
            action = action_in && action_out;
            m = null;
        }
    }


    /// <summary>
    /// Debug Section:
    /// </summary>
    /// <returns>

    public int GetRBufferFillDEBUG()
    {
        return resourceBuffer.Count;
    }

    public override ModuleInformation GetModuleInformation()
    {
        Resource peek = null;
        if(resourceBuffer.Count > 0)
        {
            peek = resourceBuffer.Peek().Resource;
        }
        return new ModuleInformation(TYPE.BUFFER,GetSTATE(), peek, allowedResources, null, null, null, null);
    }

    public override List<Resource> GetAcceptedResources()
    {
        return allowedResources;
    }

    public override Resource GetOutputResource()
    {
        return resourceBuffer.Peek().Resource;
    }

    public override void ResetModule()
    {
        base.ResetModule();
        resourceBuffer.Clear();
        absoluteFill = 0;
        DetermineState ();
    }

    public override bool ResourceSetupBlueprint(Resource resource)
    {
        return true;
    }

    public override Resource GetProduct()
    {
        if(resourceBuffer.Count > 0)
            return resourceBuffer.Peek().Resource;
        else return null;
    }
}
