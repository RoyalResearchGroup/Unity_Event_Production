using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BlueprintManager))]
public class Station : Module
{
    private BlueprintManager b_manager;

    //Current accepting states(Mirror the blueprint managers version)
    public List<Resource> allowedResources = new List<Resource>();

    //Product buffer
    protected List<ResourceObject> products = new List<ResourceObject>();

    public override void Start()
    {
        base.Start();
        b_manager = GetComponent<BlueprintManager>();
    }

    //callback override
    public override void EventCallback(Event r_event)
    {
        //The base callback call is very important, especially in producing machines!
        base.EventCallback(r_event);
        //DEBUG:
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    public override void LateUpdate()
    {
        base.LateUpdate();
    }


    public override void DispatchEvent()
    {

        //DEBUG:
        GetComponent<SpriteRenderer>().color = Color.yellow;
    }


    public override void UpdateCTRL()
    {

    }

    public override void DetermineState()
    {
        throw new System.NotImplementedException();
    }

    public override void MoveToModule(Module module)
    {
        throw new System.NotImplementedException();
    }

    public override bool IsInputReady(Resource r)
    {
        throw new System.NotImplementedException();
    }

    public override bool IsOutputReady(List<Resource> r)
    {
        throw new System.NotImplementedException();
    }
}
