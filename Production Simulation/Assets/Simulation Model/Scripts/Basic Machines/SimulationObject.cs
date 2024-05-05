using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum STATE
{
    AVAILABLE,
    OCCUPIED,
    BLOCKED,
    EMPTY,
    SETUP,
    AGENT
}


public enum TYPE
{
    BUFFER,
    STATION,
    SOURCE,
    DRAIN
}

/// <summary>
/// The base class for all Objects that are part of the simulation.
/// </summary>
public abstract class SimulationObject : MonoBehaviour
{
    // A list of modules this one is connected to
    [SerializeField]
    public List<SimulationObject> connectedObjects;

    protected List<GameObject> predecessors = new List<GameObject>();
    protected List<GameObject> successors = new List<GameObject>();

    //State management
    private STATE currentState = STATE.AVAILABLE;

    public GameObject lineRenderer;
    private GameObject lineRendererInstance;

    //UI
    [SerializeField]
    public GameObject UIPanel;

    public virtual void Start()
    {
        SetupLists();

        //Create the lines
        lineRenderer = Instantiate(lineRenderer,transform);
        lineRenderer.GetComponent<LineDrawer>().DrawLinesToSuccessors(successors);
    }

    //Setup the environment list (all adjacent Simulation objects)
    public void SetupLists()
    {
        foreach(SimulationObject obj in connectedObjects)
        {
            successors.Add(obj.gameObject);
        }


        //Call all successors and set this instance as predecessor
        foreach (GameObject suc in successors)
        {
            suc.GetComponent<SimulationObject>().AddPredecessor(gameObject);
        }
    }
    public void AddPredecessor(GameObject predecessor)
    {
        predecessors.Add(predecessor);
    }


    //State Machine: Every Simulation object has a state machine. In case of an agent, it has a constant state AGENT
    public STATE GetSTATE()
    {
        return currentState;
    }

    public void SetSTATE(STATE state)
    {
        currentState = state;
    }

    //These methods should be overwritten by the respectable derivate
    public abstract SimulationObject InputCTRL(List<Resource> r);
    public abstract SimulationObject OutputCTRL(Resource r);
    //Can be called by previous or succeding models to trigger I/O controls
    public abstract void UpdateCTRL(Module m = null);

    public abstract bool IsInputReady(Resource r);

    public abstract bool IsOutputReady(List<Resource> r);
    
    


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
