using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Assembler : Module
{
    //The assembler is a bit different to the other machines as it has to take in different types of resources
    //To accomplish this easily, the assembler modifies the intern resource list of the modules
    //When collected all necessary resource, the event is dispatched. Earlier calls for dispatches are ignored
    [SerializeField]
    public float assembleTime;
    [SerializeField]
    public Blueprint blueprint;

    public override void EventCallback(Event r_event)
    {
        base.EventCallback(r_event);
        //Finished creating, out blocked
        SetSTATE(STATE.AVAILABLE);

        //DEBUG:
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    public override void LateUpdate()
    {
        base.LateUpdate();
    }


    public override void DispatchEvent()
    {
        //The module has to be filled with enough valid resources
        //We have to check the contents of the buffer and remove those resources from the variable buffer that are already present in the necessary size
        bool ready = false;




        //Dispatch the Event to process the resource
        e_manager.EnqueueEvent(new Event(assembleTime, this, EVENTTYPE.PROCESS));

        //DEBUG:
        GetComponent<SpriteRenderer>().color = Color.yellow;
    }

}
