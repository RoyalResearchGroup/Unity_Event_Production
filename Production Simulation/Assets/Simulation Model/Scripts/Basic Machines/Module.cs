using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;
using UnityEngine.EventSystems;


/// <summary>
/// The Base class for all modules: Drain, Station, etc.
/// </summary>
public abstract class Module : SimulationObject
{
    protected LimitedQueue<ResourceObject> resourceBuffer;

    //Event system: The modules schedule an event with the event manager
    [HideInInspector]
    protected EventManager e_manager;
    [HideInInspector]
    protected ExperimentManager x_manager;
    protected bool e_callback = false;

    //Has an event been dispatched and is pending?
    protected bool d_event = false;

    public override void Start()
    {
        base.Start();
        e_manager = GetComponentInParent<EventManager>();
    }



    //__________________________________________________________________________
    //Update here: Update and event handling
    /// <summary>
    /// IMPORTANT NOTE: We do this in the NotifyEventBatch as the EventCallback is called in the update method. This means we can check possibly queued events in our local event queue for their state.
    /// </summary>
    public virtual void NotifyEventBatch()
    {
        //Check event queue. If the current state is managed in an event, we dont execute this function.
        //The occupied state means that the module is currently paused by the time specified in the dispatched event.

        //Also, only execute if there has been an event callback this frame

        if (e_callback)
        {
            UpdateCTRL(null);
        }
        //Rest the check
        e_callback = false;
    }



    //Override the I/O functions
    //In the case of a "module", the output function works as a checker for aviable modules succeeding this module!
    public override SimulationObject OutputCTRL(Resource r)
    {
        SimulationObject object_out = null;
        foreach(GameObject module in successors)
        {
            //We might prioritize Agents over simple connections (for now not relevant), take the first one aviable
            if(module.GetComponent<SimulationObject>().GetSTATE() == STATE.AGENT)
            {
                Module target = module.GetComponent<BaseAgent>().DetermineAction(gameObject, false);
                //Debug.Log(target);
                
                if (target && target.IsInputReady(r))
                {
                    //Debug.Log("Legal: " + target + " Caller: " + gameObject.name);
                    object_out = target.GetComponent<SimulationObject>();
                    // check if target is a valid target
                    if (Random.value < 1 / (successors.Count))
                    {
                        break;
                    }
                }
                else
                {
                    if(!target)
                    {
                        //No action taken, valid
                        //Debug.Log("No action");
                    }
                    else
                    {
                        Debug.Log("Illegal output: " + target + " Caller: " + gameObject.name);
                        //Callback for Agent Illegal Action
                        //x_manager.ResetScene();
                        reportInacceptibleAgent();
                    }
                }
            }
            //Did we find a fitting module? It needs to be available and support the given resource
            if (module.GetComponent<SimulationObject>().IsInputReady(r))
            {
                object_out = module.GetComponent<SimulationObject>();
                //Assign with a random possibility (choose all modules with equal possibility)
                if (Random.value < 1 / (successors.Count))
                {
                    break;
                }
            }
        }
        return object_out;
    }

    //In a module, the Input function handles the input of resources. If it was possible to clear the module (at least one position is aviable now), we can search for available input modules that might provide a new resource
    public override SimulationObject InputCTRL(List<Resource> r)
    {
        SimulationObject object_in = null;
        foreach (GameObject module in predecessors)
        {
            //Did we find a fitting module? It must have a fitting resource ready.
            if (module.GetComponent<SimulationObject>().IsOutputReady(r))
            {
                object_in = module.GetComponent<SimulationObject>();
                //Assign with a random possibility (choose all modules with equal possibility)
                if (Random.value < 1 / (predecessors.Count))
                {
                    break;
                }
            }
            //We might prioritize Agents over simple connections (for now not relevant), take the first one aviable
            else if(module.GetComponent<SimulationObject>().GetSTATE() == STATE.AGENT)
            {
                Module target = module.GetComponent<BaseAgent>().DetermineAction(gameObject, true);
                //Debug.Log(target);

                if (target && target.IsOutputReady(r))
                {
                    object_in = target.GetComponent<SimulationObject>();
                    //Debug.Log("Legal: " + target + " Caller: " + gameObject.name);
                    // check if target is a valid target
                    if (Random.value < 1 / (successors.Count))
                    {
                        break;
                    }
                }
                else
                {
                    if (!target)
                    {
                        //No action taken, valid
                        //Debug.Log("No action");
                    }
                    else
                    {
                        Debug.Log("Illegal input: " + target + " Caller: " + gameObject.name);
                        reportInacceptibleAgent();
                    }
                }
            }

        }
        return object_in;
    }

    private void reportInacceptibleAgent()
    {
        //Nothing here yet.
    }

    //A scheduled event was performed, do the action (callback by the event system).
    public virtual void EventCallback(Event r_event)
    {
        e_callback = true;
        d_event = false;
    }

    //Update the state based on the resourceArray state and event queue
    public abstract void DetermineState();

    //Function to move a resource from this module to another one.
    public abstract void MoveToModule(Module module);

    //Dispatch an event
    public virtual void DispatchEvent()
    {
        d_event = true;
    }

    //Add a resource to the buffer
    public void AddResource(ResourceObject o)
    {
        resourceBuffer.Enqueue(o);
    }

    //Get the resource buffer
    public LimitedQueue<ResourceObject> GetResourceBuffer()
    {
        return resourceBuffer;
    }

    //Get all currently accepted resources and output resource (e.g. product), implemented by every module.
    public abstract List<Resource> GetAcceptedResources();
    public abstract Resource GetOutputResource();

    //Get current state information:
    public abstract ModuleInformation GetModuleInformation();

    public virtual void ResetModule()
    {
        d_event= false;
        e_callback= false;
    }

    public abstract bool ResourceSetupBlueprint(Resource resource);

    public abstract Resource GetProduct();

    /// <summary>
    ///DEBUG SECTION
    /// </summary>

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
