using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Station : Module
{
    [SerializeField]
    public List<float> resourceProcessingTimes = new List<float>();
    [SerializeField]
    public List<float> resourceSetupTimes = new List<float>();

    //callback override
    public override void EventCallback(Event r_event)
    {
        base.EventCallback(r_event);
        //Finished creating, out blocked
        SetSTATE(STATE.BLOCKED);
    }

    public override void LateUpdate()
    {
        base.LateUpdate();

        //If the buffer is not full
        if (resourceArray.Count < resourceArray.Limit && resourceArray.Count > 0 && GetSTATE() != STATE.OCCUPIED)
        {
            //Get the processing time:
            //Index:
            int id = resources.FindIndex(r => r == resourceArray.Peek().Resource);
            float t = resourceProcessingTimes[id];
            //Dispatch the Event to process the resource
            e_manager.EnqueueEvent(new Event(t, this, EVENTTYPE.PROCESS));
            Debug.Log(e_manager.m_events.PrintEvents());
            SetSTATE(STATE.OCCUPIED);
        }
    }
}
