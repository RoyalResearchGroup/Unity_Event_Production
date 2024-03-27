using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class Source : Module
{
    public float creationRate;
    public Resource creationType;

    public override void EventCallback(Event r_event)
    {
        //Finished creating, out blocked
        SetSTATE(STATE.BLOCKED);
        //Create the resource object
        ResourceObject obj = new ResourceObject(creationType);
        resourceArray.Enqueue(obj);
    }

    public override void LateUpdate()
    {
        base.LateUpdate();

        //If the buffer is not full
        if(resourceArray.Count < resourceArray.Limit && GetSTATE()!=STATE.OCCUPIED) {
            //Dispatch the Event to spawn a resource
            e_manager.EnqueueEvent(new Event(1 / creationRate, this, EVENTTYPE.CREATE));
            SetSTATE(STATE.OCCUPIED);   
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
}


