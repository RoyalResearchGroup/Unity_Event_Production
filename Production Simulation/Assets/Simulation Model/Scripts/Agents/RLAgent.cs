using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

[RequireComponent(typeof(MLInterface))]
[RequireComponent(typeof(BufferSensorComponent))]
public class RLAgent : BaseAgent
{
    
    // The strategy that the agent uses
    [SerializeField] protected Strategy _strategy;
    BufferSensorComponent bf;
    MLInterface mlAgent;
    ActionSegment<int> discreteActions;
    bool actionsReceived = false;

    private void Start()
    {
        SetSTATE(STATE.AGENT);
        bf = GetComponent<BufferSensorComponent>();
        mlAgent = GetComponent<MLInterface>();
    }

    protected override GameObject Decide(GameObject caller, List<ModuleInformation> m_info)
    {
        /*if (_strategy == null)
        {
            // we call this civil disobedience
            Debug.LogWarning("No strategy selected. Will do nothing...");
            return null;
        }

        return _strategy.act(caller, m_info);*/

        List<float> inputs = new List<float>();
        foreach (ModuleInformation info in m_info)
        {
            //Take the first valid option
            switch (info.type)
            {
                /*case TYPE.SOURCE:
                    break;
                case TYPE.BUFFER:
                    
                    break;
                case TYPE.STATION:
                    
                    break;
                case TYPE.DRAIN:
                    
                    break;*/
                default:
                    float inp = System.Convert.ToSingle(info.valid && info.ready);
                    inputs.Add(inp);
                    break;
            }
        }
        bf.AppendObservation(inputs.ToArray());

        mlAgent.RequestAction();

        // Wait for the actions to be received using a coroutine
        StartCoroutine(WaitForActions());

        return null;
    }

    
    // Implement WaitForActions coroutine
    IEnumerator WaitForActions()
    {
        while (!actionsReceived)
        {
            yield return new WaitForSeconds(0.1f);
        }
        if (discreteActions.Length > 0)
        {
            var output = discreteActions[0];


            if (output < 0 || output >= m_info.Count)
            {
                // give penalty for invalid action
                Debug.LogWarning("Action out of range");
                mlAgent.EndEpisode();
            }
            else if (m_info[output].valid && m_info[output].ready)
            {
                // valid action received, return the corresponding module
                yield return m_info[output].module;
            }
            else
            {
                // give penalty for invalid action
                Debug.LogWarning("Invalid action selected");
                mlAgent.EndEpisode();
            }
        }
        else
        {
            Debug.LogWarning("No actions received");
        }

        // Reset the flag for the next decision
        actionsReceived = false;
    }
    

    public void CollectObservations(VectorSensor sensor)
    {

    }


    public void SetActions(ActionSegment<int> acts)
    {
        discreteActions = acts;
        actionsReceived = true;
    }
    
}
