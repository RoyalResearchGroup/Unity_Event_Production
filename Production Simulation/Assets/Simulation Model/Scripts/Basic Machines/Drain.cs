using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drain : Module
{
    //Number of drained (exiting the system) items
    public int absoluteDrain;



    public override void LateUpdate()
    {
        SetSTATE(STATE.AVAILABLE);
        //Simply clear the resource buffer
        if(resourceArray.Count > 0 )
        {
            absoluteDrain += 1;
            resourceArray.Clear();
        }
    }
}
