using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Tilemaps;
using UnityEngine;


public class Module : SimulationObject
{
    //A list of resources that this machine takes in and its capacity
    [SerializeField]
    public List<Resource> resources = new List<Resource>();
    [SerializeField]
    public int capacity = 1;

    [SerializeField]
    public bool disableCTRL = false;

    //The instances of these resource objects
    protected LimitedQueue<ResourceObject> resourceArray;

    //Is this module used for production? If not, it is used as a buffer/drain/etc.
    [SerializeField]
    public bool producing = true;


    //Event system: The modules schedule an event with the event manager
    [HideInInspector]
    protected EventManager e_manager;
    private bool e_callback = false;


    public void Start()
    {
        e_manager = GameObject.FindWithTag("EventManager").GetComponent<EventManager>();
        resourceArray = new LimitedQueue<ResourceObject>(capacity);
        SetupLists();
    }



    //__________________________________________________________________________
    //Update here: Update and event handling
    /// <summary>
    /// IMPORTANT NOTE: We do this in the LateUpdate as the EventCallback is called in the update method. This means we can check possibly queued events in our local event queue for their state.
    /// </summary>
    public virtual void LateUpdate()
    {
        //Check event queue. If the current state is managed in an event, we dont execute this function.
        //The occupied state means that the module is currently paused by the time specified in the dispatched event.

        //Also, only execute if there has been an event callback this frame

        if (e_callback && resourceArray.Count>0)
        {
            UpdateCTRL();
        }
        //Rest the check
        e_callback = false;
    }



    //Can be called by previous or succeding modules to trigger I/O controls
    public override void UpdateCTRL()
    {
        if (disableCTRL)
        {
            return;
        }
        //Nothing to do if there are no resources present
        //We want to update the network in this step. This means that if theres currently a blockage of the system, we have to notify adjacent modules.
        //We do this by using in- and output controls.

        //For example, our machine just finished working. We first use the out_ctrl to find a suitable machine (can be an agent or just random guessing) to forward the Resource to.
        //If there is none, our machine goes into the BLOCKED state, running input_ctrl is not necessary now

        //If the machine is free now, its state switches to AVAILABLE and all predecessing machines are notified. again, this might be handled by an agent or just random guessing

        //IMPORTANT: When input_ctrl is called, this decides what to do based on the inputs and calls output_ctrl on the chosen machine followed by input_ctrl to check for its own 

        //Do this as long as theres available modules on the path and we have a resource in this modules buffer

        while(resourceArray.Count > 0)
        {
            SetSTATE(STATE.BLOCKED);
            SimulationObject sim_out;
            SimulationObject sim_in;

            //To check if a machine is ready for our current resource, we have to compare it to the list of suported resources. For now, we identify similar resources by its type.
            Resource resource_peek = resourceArray.Peek().Resource;

            if ((sim_out = OutputCTRL(resource_peek)) == null)
            {
                return;
            }
            //Debug.Log("_________________________________________");
            //Debug.Log("Going into: " + sim_out.gameObject.name);

            //If we found a machine acepting our resource, we move it to this machine.
            //Use different approach based on agent
            if (sim_out.GetSTATE() != STATE.AGENT)
                MoveToModule((Module)sim_out);
            else
            {
                //Empty for now...
            }

            //The last thing here is to dispatch the update on the out ctrl model:
            //This ensures that all machines connected from there are up to date
            sim_out.UpdateCTRL();


            if ((sim_in = InputCTRL()) == null)
            {
                return;
            }

            //Debug.Log("Drawing from: " + sim_in.gameObject.name);
            //Debug.Log("_________________________________________");

            //If we found a machine acepting our resource, we move it to this machine.
            //Use different approach based on agent
            if (sim_in.GetSTATE() != STATE.AGENT)
                MoveFromModule((Module)sim_in);
            else
            {
                //Empty for now...
            }

            //If we found a suitable machine, we just have to call the UpdateCTRL on it.
            sim_in.UpdateCTRL();
        }
    }

    //Override the I/O functions
    //In the case of a "module", the output function works as a checker for aviable modules succeeding this module!
    public override SimulationObject OutputCTRL(Resource r)
    {
        SimulationObject object_out = null;
        foreach(GameObject module in successors)
        {
            //Did we find a fitting module? It needs to be available and support the given resource
            if (module.GetComponent<SimulationObject>().GetSTATE() == STATE.AVAILABLE && module.GetComponent<Module>().resources.Contains(r))
            {
                object_out = module.GetComponent<SimulationObject>();
                //Assign with a random possibility (choose all modules with equal possibility)
                if (Random.value < 1 / (successors.Count))
                {
                    break;
                }
            }
            //We might prioritize Agents over simple connections (for now not relevant), take the first one aviable
            else if(module.GetComponent<SimulationObject>().GetSTATE() == STATE.AGENT)
            {
                //if(module.GetComponent<Agent>().DetermineAction()!=ACTION.NOTHING)
                //return ...
            }

        }
        return object_out;
    }

    //In a module, the Input function handles the input of resources. If it was possible to clear the module (at least one position is aviable now), we can search for available input modules that might provide a new resource
    public override SimulationObject InputCTRL()
    {
        SimulationObject object_in = null;
        foreach (GameObject module in predecessors)
        {
            //Did we find a fitting module? It must have a fitting resource ready.
            if (module.GetComponent<SimulationObject>().GetSTATE() == STATE.BLOCKED && resources.Contains(module.GetComponent<Module>().resourceArray.Peek().Resource))
            {
                object_in = module.GetComponent<SimulationObject>();
                //Assign with a random possibility (choose all modules with equal possibility)
                if (Random.value < 1 / (predecessors.Count))
                {
                    break;
                }
            }
            //We might prioritize Agents over simple connections (for now not relevant), take the first one aviable
            else if (module.GetComponent<SimulationObject>().GetSTATE() == STATE.AGENT)
            {
                //if(module.GetComponent<Agent>().DetermineAction()!=ACTION.NOTHING)
                //return ...
            }

        }
        return object_in;
    }



    //A scheduled event was performed, do the action (callback by the event system).
    public virtual void EventCallback(Event r_event)
    {
        e_callback = true;
    }

    //Update the state based on the resourceArray state and event queue
    public void DetermineState()
    {
        //Distinct between two types: producing and not producing. Not producing modules do not need to dispatch an event when a resource enters the machine.

        if(producing)
        {
            SetSTATE(STATE.OCCUPIED);
            //In this case, we also have to dispatch the event! This has to be overwritten by the module instances themselves!
            DispatchEvent();
        }
        else
        {
            if(resourceArray.Count == resourceArray.Limit)
                SetSTATE(STATE.BLOCKED);
            else
                SetSTATE(STATE.AVAILABLE);
        }
    }

    //Function to move a resource from this module to another one.
    public void MoveToModule(Module module)
    {
        //Remove the resource from this machine and assign it to the target machine
        ResourceObject res = resourceArray.Dequeue();
        module.resourceArray.Enqueue(res);

        //The other module might be a buffer or take in several resources, so we keep that in mind here.
        module.DetermineState();
        DetermineState();
    }

    //Function to move a resource from this module to another one.
    public void MoveFromModule(Module module)
    {
        //Remove the resource from this machine and assign it to the target machine
        ResourceObject res = module.resourceArray.Dequeue();
        resourceArray.Enqueue(res);

        //The other module might be a buffer or take in several resources, so we keep that in mind here.
        module.DetermineState();
        DetermineState();
    }


    //Dispatch an event
    public virtual void DispatchEvent(){}



    // Visualize connections in editor mode
    void OnDrawGizmos()
    {
        if (connectedObjects != null)
        {
            foreach (SimulationObject module in connectedObjects)
            {
                if (module != null)
                {
                    Gizmos.color = Color.black;
                    Gizmos.DrawLine(transform.position, module.transform.position);
                }
            }
        }
    }
}
