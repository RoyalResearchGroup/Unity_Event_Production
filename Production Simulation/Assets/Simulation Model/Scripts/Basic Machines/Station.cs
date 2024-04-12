using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BlueprintManager))]
public class Station : Module
{
    private BlueprintManager b_manager;

    //Current accepting states(Mirror the blueprint managers version)
    private List<Resource> allowedResources = new List<Resource>();
    private Blueprint currentBlueprint;
    private Blueprint setupBlueprint = null; //No setup at start
    //Product buffer
    protected ResourceObject product;


    //DEBUG/STATS: Save the ratio of SETUP time to occupation time if setup is needed
    //[HideInInspector]
    public float setupRatio { get; set; } = 0f;


    public override void Start()
    {
        base.Start();
        b_manager = GetComponent<BlueprintManager>();

        //Get the first set of required Resources and init the blueprint manager -> extract all necessary information from the set blueprints
        b_manager.InitializeBlueprintSettings();

        //Current largest capacity
        resourceBuffer = new LimitedQueue<ResourceObject>(b_manager.GetCurrentCapacity());

        b_manager.UpdateAllowedResourcesAndBlueprints(resourceBuffer);
        allowedResources = b_manager.GetAllowedResources();

        product = new ResourceObject(null);
    }

    //callback override
    public override void EventCallback(Event r_event)
    {
        //The base callback call is very important, especially in producing machines!
        base.EventCallback(r_event);

        //Create the new product
        product = new ResourceObject(currentBlueprint.product);

        //Wipe all changes made
        resourceBuffer.Clear();
        b_manager.UpdateAllowedResourcesAndBlueprints(resourceBuffer);
        allowedResources = b_manager.GetAllowedResources();
        resourceBuffer.Limit = b_manager.GetCurrentCapacity();
        DetermineState();

        //DEBUG:
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    public override void NotifyEventBatch()
    {
        base.NotifyEventBatch();
    }


    public override void DispatchEvent()
    {
        base.DispatchEvent();

        //Production time calculation: if the blueprint is not ready (different setup), add the setup time
        float time = currentBlueprint.processingTime;
        setupRatio = 0f;
        if (currentBlueprint != setupBlueprint)
        {
            time += currentBlueprint.setupTime;
            setupBlueprint = currentBlueprint;
            setupRatio = currentBlueprint.setupTime / time;
        }
        //Enqueue the event
        e_manager.EnqueueEvent(new Event(time, this, EVENTTYPE.PROCESS));
        //DEBUG:
        GetComponent<SpriteRenderer>().color = Color.yellow;
    }


    public override void UpdateCTRL(Module m)
    {
        bool action = true;
        while (resourceBuffer.Count < resourceBuffer.Limit && action)
        {
            //Update the blueprint manager in case a blueprint is finished
            b_manager.UpdateAllowedResourcesAndBlueprints(resourceBuffer);
            allowedResources = b_manager.GetAllowedResources();
            resourceBuffer.Limit = b_manager.GetCurrentCapacity();
            bool action_in = true;
            bool action_out = true;
            //The source can only output resources, so no input model needed
            Module mod_out;

            //The current resource type in the buffer
            Resource res = product.Resource;

            //Get a candidate for output
            mod_out = (Module)OutputCTRL(res);

            if (m != null)
            {
                mod_out = m;
            }

            //If there no candidate, the out action failed
            if (mod_out == null)
            {
                DetermineState();
                action_out = false;
            }
            else
            {
                //Otherwise, we can move the resource
                MoveToModule(mod_out);
                mod_out.UpdateCTRL(null);
            }

            if(product.Resource != null)
            {
                return;
            }

            //Check if there is an aviable input machine that could provide a new resource
            Module mod_in;

            mod_in = (Module)InputCTRL(allowedResources);

            //Same case for the input
            if (mod_in == null)
            {
                DetermineState();
                action_in = false;
            }
            else
            {
                //Otherwise, initiate the transaction
                mod_in.UpdateCTRL(GetComponent<Module>());
            }

            //If neither one of the actions was successful, break the loop
            action = action_in && action_out;
            m = null;
        }

        //UpdateCTRL is a bit different here then in other modules. Every time this is called, we need to check if the machine is ready to produce something.
        Blueprint ready_bp = b_manager.FindFirstBlueprintMeetingRequirements(resourceBuffer);
        if (ready_bp != null)
        {
            currentBlueprint = ready_bp;
            //If this is the case, the event can be dispatched and the method returned.
            DispatchEvent();
            DetermineState();
        }

    }

    public override void DetermineState()
    {
        //If there is currently an event dispatched, the state is occupied
        if (d_event)
        {
            SetSTATE(STATE.OCCUPIED);
        }
        else
        {
            if (resourceBuffer.Count > 0 && product.Resource == null)
            {
                //If there are no products in the product buffer, the machine is available, otherwise blocked
                if (resourceBuffer.Count < resourceBuffer.Limit)
                    SetSTATE(STATE.AVAILABLE);
                else
                    SetSTATE(STATE.BLOCKED);
            }
            else if(product.Resource != null)   //If theres products, the machine is automatically blocked, no input is accepted
            {
                SetSTATE(STATE.BLOCKED);
            }
            else { SetSTATE(STATE.AVAILABLE);}

            //DEBUG:
            setupRatio = 0f;
        }
    }

    public override void MoveToModule(Module module)
    {
        ResourceObject res = product;
        product = new ResourceObject(null);
        module.AddResource(res);
        DetermineState();
        module.DetermineState();
    }

    public override bool IsInputReady(Resource r)
    {
        //This module is input ready if r matches at least one allowed resource and the machine is currently not producing anything and theres space in the resource buffer
        if(allowedResources.Contains(r) && !d_event && resourceBuffer.Count < resourceBuffer.Limit)
        {
            return true;
        }
        else return false;
    }

    public override bool IsOutputReady(List<Resource> r)
    {
        //This module is output ready if there a product that matches a resource in the list and it is not producing anything atm (null if no product -> no match)

        if(r.Contains(product.Resource) && !d_event)
        {
            return true;
        }
        return false;
    }

    public override ModuleInformation GetModuleInformation()
    {
        Resource peek = null;
        if(product.Resource != null)
        {
            peek = product.Resource;
        }

        return new ModuleInformation(TYPE.STATION,GetSTATE(), peek, allowedResources, setupBlueprint, b_manager.GetProcessingTimes(), b_manager.blueprints);
    }

    public override List<Resource> GetAcceptedResources()
    {
        return allowedResources;
    }

    public override Resource GetOutputResource()
    {
        return product.Resource;
    }

    public override void ResetModule()
    {
        base.ResetModule();
        Debug.Log("RESET");
        resourceBuffer.Clear();
        product.Resource = null;
        b_manager.UpdateAllowedResourcesAndBlueprints(resourceBuffer);
        allowedResources = b_manager.GetAllowedResources();
        currentBlueprint = null;
        setupBlueprint = null;
        GetComponent<SpriteRenderer>().color = Color.white;
        DetermineState();
    }
}
