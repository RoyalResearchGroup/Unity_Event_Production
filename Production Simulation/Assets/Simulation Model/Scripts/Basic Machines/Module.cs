using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;


public enum STATE
{
    AVAILABLE,
    OCCUPIED,
    BLOCKED
}

public class Module : SimulationObject
{
    //A list of resources that this machine takes in and its capacity
    public List<Resource> resources;
    public uint capacity = 1;
    //The instances of these resource objects
    private ResourceObject[] resourceArray;

    //State management
    private STATE currentState = STATE.AVAILABLE;


    //Event system: The modules schedule an event with the event manager
    private EventManager e_manager;



    public void Start()
    {
        e_manager = GameObject.FindWithTag("EventManager").GetComponent<EventManager>();
        resourceArray = new ResourceObject[capacity];
        Debug.Log("Name: " + gameObject.name);
        Debug.Log("Name: " + gameObject.GetComponent<SimulationObject>());
        SetupLists(gameObject.GetComponent<SimulationObject>());
    }




    //__________________________________________________________________________

    //Update here: Event scheduling
    private void Update()
    {

    }


    //Update here: Move the movable units or get the action from an agent
    private void LateUpdate()
    {
        //Call to update our models state (the network might have changed)
        //UpdateCTRL();
    }



    //Can be called by previous or succeding models to trigger I/O controls
    public override void UpdateCTRL()
    {
        //We want to update the network in this step. This means that if theres currently a blockage of the system, we have to notify adjacent models.
        //We do this by using in- and output controls.

        //For example, our machine just finished working. We first use the out_ctrl to find a suitable machine (can be an agent or just random guessing) to forward the Resource to.
        //If there is none, our machine goes into the BLOCKED state, running input_ctrl is not necessary now

        //If the machine is free now, its state switches to AVAILABLE and all predecessing machines are notified. again, this might be handled by an agent or just random guessing

        //IMPORTANT: When input_ctrl is called, this decides what to do based on the inputs and calls output_ctrl on the chosen machine followed by input_ctrl to check for its own 

        if (OutputCTRL() == null)
        {
            SetSTATE(STATE.BLOCKED);
            return;
        }

        SetSTATE(STATE.AVAILABLE);
        //Move the object to the returned free machine



        InputCTRL();
    }



    //A scheduled event was performed, do the action (callback by the event system)
    public void EventCallback()
    {

    }


    //State Machine:
    public STATE GetSTATE()
    {
        return currentState;
    }

    public void SetSTATE(STATE state)
    {
        currentState = state;
    }



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
