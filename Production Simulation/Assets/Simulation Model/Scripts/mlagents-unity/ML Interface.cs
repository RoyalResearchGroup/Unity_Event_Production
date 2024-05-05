using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class MLInterface : Agent
{
    RLAgent rlAgent;

    private void Awake()
    {
        Academy.Instance.AutomaticSteppingEnabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        rlAgent = GetComponent<RLAgent>();
    }

    public override void OnEpisodeBegin()
    {
       //Wub Wub
    }



    public override void CollectObservations(VectorSensor sensor)
    {
        rlAgent.CollectObservations(sensor);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        rlAgent.SetActions(actions.DiscreteActions);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        rlAgent.UseStrategy(actionsOut);
    }
}
