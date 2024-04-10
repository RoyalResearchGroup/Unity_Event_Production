using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

[RequireComponent(typeof(MLAgent))]
public class RLAgent : BaseAgent
{
    
    // The strategy that the agent uses
    [SerializeField] protected Strategy _strategy;

    protected override GameObject Decide(GameObject caller, List<ModuleInformation> m_info)
    {
        if (_strategy == null)
        {
            // we call this civil disobedience
            Debug.LogWarning("No strategy selected. Will do nothing...");
            return null;
        }

        return _strategy.act(caller, m_info);
    }
    
}
