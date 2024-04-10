using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class MLAgent : Agent
{
    RLAgent rlAgent;
    // Start is called before the first frame update
    void Start()
    {
        rlAgent = GetComponent<RLAgent>();
    }

    public override void OnEpisodeBegin()
    {
        // TODO Here the environment needs to be reset by the experiment manager
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor 
    }
}
