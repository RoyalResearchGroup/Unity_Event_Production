using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drain : Module
{
    //Number of drained (exiting the system) items
    public int absoluteDrain;

    public override void DetermineState()
    {
        SetSTATE(STATE.AVAILABLE);
    }

    public override bool IsInputReady(Resource r)
    {
        //Input is always possible
        return true;
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

    public override void LateUpdate()
    {
        //Simply clear the resource buffer
        if(resourceBuffer.Count > 0 )
        {
            absoluteDrain += resourceBuffer.Count;
            resourceBuffer.Clear();
        }
    }

    public override void MoveToModule(Module module)
    {
        //There will be no follow ip modules, so nothing to do here
    }

    public override void UpdateCTRL()
    {
        //No need to update anything
    }
}
