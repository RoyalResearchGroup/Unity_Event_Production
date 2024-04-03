using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueprintManager : MonoBehaviour
{

    //The blueprint this machine is producing
    [SerializeField]
    public List<Blueprint> blueprint = new List<Blueprint>();

    //A list of resources that this machine takes in and its capacity (set by blueprint)
    private List<float> resourceProcessingTimes = new List<float>();
    private List<float> resourceSetupTimes = new List<float>();
    private List<int> capacities = new List<int>();

    //Current accepting states
    private List<Resource> allowedResources = new List<Resource>();
    private int currentCapacity;
    private List<Blueprint> currentBlueprints = new List<Blueprint>();

    private void Start()
    {
        
    }


    public float GetProcessingTime(Blueprint blueprint)
    {
        return 0;
    }

    public List<Resource> GetAllowedResource()
    {
        return allowedResources;
    }

}
