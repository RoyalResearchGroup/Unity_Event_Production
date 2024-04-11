using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class MLInterface : Agent
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
        rlAgent.CollectObservations(sensor);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // TODO Here the agent needs to act on the environment
        rlAgent.SetActions(actions.DiscreteActions);
    }



}
