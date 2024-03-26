using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationObject : MonoBehaviour
{
    // A list of modules this one is connected to
    [SerializeField]
    public List<SimulationObject> connectedObjects;

    private List<SimulationObject> predecessors;
    private List<SimulationObject> successors;


    public void SetupLists(SimulationObject instance)
    {
        //Initialize successors based on connected modules
        successors = new List<SimulationObject>();
        successors.AddRange(connectedObjects);

        //Call all successors and set this instance as predecessor
        foreach (SimulationObject suc in successors)
        {
            suc.AddPredecessor(instance);
        }
    }


    public void AddPredecessor(SimulationObject predecessor)
    {
        predecessors.Add(predecessor);
    }


    public virtual SimulationObject InputCTRL()
    {
        return null;
    }

    //Push object out of model
    public virtual SimulationObject OutputCTRL()
    {
        return null;
    }

    //Can be called by previous or succeding models to trigger I/O controls
    public virtual void UpdateCTRL()
    { 

    }
}
