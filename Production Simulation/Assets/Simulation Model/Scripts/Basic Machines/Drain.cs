using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drain : Module
{
    //Number of drained (exiting the system) items
    public int absoluteDrain;
    public List<Resource> allowedResources;


    public override void DetermineState()
    {
        SetSTATE(STATE.AVAILABLE);
    }

    public override bool IsInputReady(Resource r)
    {
        if(allowedResources.Contains(r)) return true;
        return false;   
    }

    public override bool IsOutputReady(List<Resource> r)
    {
        //Does never output anything
        return false;
    }

    public override void Start()
    {
        base.Start();
        resourceBuffer = new LimitedQueue<ResourceObject> (10000);
        DetermineState();
    }

    public override void NotifyEventBatch()
    {
        //Simply clear the resource buffer
        if(resourceBuffer.Count > 0 )
        {
            absoluteDrain += resourceBuffer.Count;
            resourceBuffer.Clear();
        }
        base.NotifyEventBatch();
    }

    public override void MoveToModule(Module module)
    {
        //There will be no follow ip modules, so nothing to do here
    }

    public override void UpdateCTRL(Module m)
    {
        //Check if there is an aviable input machine that could provide a new resource
        while(true)
        {
            Module mod_in;

            mod_in = (Module)InputCTRL(allowedResources);

            //Same case for the input
            if (mod_in == null)
            {
                DetermineState();
                return;
            }
            else
            {
                //Otherwise, initiate the transaction
                mod_in.UpdateCTRL(GetComponent<Module>());
            }
        }
    }

    public override ModuleInformation GetModuleInformation()
    {
        return new ModuleInformation(TYPE.DRAIN,GetSTATE(), null, allowedResources, null, null, null, resourceBuffer);
    }

    public override List<Resource> GetAcceptedResources()
    {
        return allowedResources;
    }

    public override Resource GetOutputResource()
    {
        return null;
    }

    public override void ResetModule()
    {
        absoluteDrain = 0;  
    }

    public override bool ResourceSetupBlueprint(Resource resource)
    {
        return true;    
    }

    public override Resource GetProduct()
    {
        return null;
    }
}
