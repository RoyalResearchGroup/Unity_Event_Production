using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using System;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System.Runtime.CompilerServices;

[RequireComponent(typeof(MLInterface))]
public class RLAgent : BaseAgent
{
    
    // The strategy that the agent uses
    [SerializeField] protected Strategy _strategy;
    MLInterface mlAgent;
    EventManager e_manager;
    GameObject caller;
    bool callerInFront;

    ActionSegment<int> discreteActions;
    bool actionsReceived = false;

    public override void Start()
    {
        base.Start();
        mlAgent = GetComponent<MLInterface>();
        e_manager = GetComponentInParent<EventManager>();
    }

    public void NotifyEventBatch()
    {
        Academy.Instance.EnvironmentStep();
    }

    public override Module DetermineAction(GameObject caller, bool callerInFront)
    {
        this.caller = caller;
        this.callerInFront = callerInFront;
        return base.DetermineAction(caller, callerInFront);
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

        //Debug.Log("Observations: " + inputs.Count);
        //bf.AppendObservation(inputs.ToArray());
        e_manager.Pause(true);
        Academy.Instance.EnvironmentStep();
        // Wait for the actions to be received using a coroutine
        StartCoroutine(WaitForActions());
        //WaitForActions();

        return null;
    }

    
    // Implement WaitForActions coroutine
    IEnumerator WaitForActions()
    {
        mlAgent.RequestDecision();

        while (!actionsReceived)
        {
            yield return new WaitForEndOfFrame();
        }

        if (discreteActions.Length > 0)
        {
            /*string outputString = "Continuous Actions: ";
            foreach (var action in discreteActions)
            {
                outputString += action + " ";
            }
            Debug.Log("Action: " + outputString);*/
            
            var output = discreteActions[0];
            //Debug.Log("Action: " + output);

            if (output == m_info.Count)
            {
                // no module selected (do nothing)
                Debug.Log("Do nothing...");
                e_manager.Pause(false);
            }
            else if (output < 0 || output >= m_info.Count)
            {
                // give penalty for invalid action
                Debug.LogWarning("Action out of range");
                // end the experiment in the experiment manager
                mlAgent.AddReward(-1f);
                mlAgent.EndEpisode(); 
                GetComponentInParent<ExperimentManager>().StopExperiment();
            }
            else if (m_info[output].valid && m_info[output].ready)
            {
                // valid action received, return the corresponding module
                //return m_info[output].module;
                Module callerM = caller.GetComponent<Module>();
                Module decisionM = m_info[output].module.GetComponent<Module>();
                Debug.Log("Valid action selected");
                mlAgent.AddReward(0.5f);
                if (callerInFront)
                {
                    // maybe the caller could not be ready to get input (if it is dogshit)
                    decisionM.UpdateCTRL(callerM);
                    //decisionM.MoveToModule(callerM);
                }
                else
                {
                    callerM.UpdateCTRL(decisionM);
                    //callerM.MoveToModule(decisionM);
                }
                e_manager.Pause(false);
            }
            else
            {
                // give penalty for invalid action
                Debug.LogWarning("Invalid action selected");
                // add penalty
                mlAgent.AddReward(-1f);
                mlAgent.EndEpisode();
                GetComponentInParent<ExperimentManager>().StopExperiment();
            }
        }
        else
        {
            Debug.LogWarning("No actions received");
            e_manager.Pause(false);
        }

        // Reset the flag for the next decision
        actionsReceived = false;
        //e_manager.Pause(false);
    }
    

    public void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(callerInFront);
        foreach (ModuleInformation info in m_info)
        {
            //Take the first valid option
            switch (info.type)
            {
                case TYPE.SOURCE:
                    sensor.AddObservation(info.product);
                    sensor.AddObservation(info.valid && info.ready);
                    break;
                case TYPE.BUFFER:
                    sensor.AddObservation(info.product);
                    sensor.AddObservation((int) info.state);
                    sensor.AddObservation(codeAllowedResources(info.input));
                    break;
                case TYPE.STATION:
                    sensor.AddObservation(info.product);
                    sensor.AddObservation((int) info.state);
                    sensor.AddObservation(info.processingTimes);
                    sensor.AddObservation(codeAllowedResources(info.input));
                    break;
                case TYPE.DRAIN:
                    sensor.AddObservation((int) info.state);
                    sensor.AddObservation(codeAllowedResources(info.input));
                    break;
            }
            sensor.AddObservation(info.valid && info.ready);
        }
    }


    public float codeAllowedResources(List<Resource> list)
    {
        if (list.Capacity == 0)
        {
            return 0;
        }
        else if (list.Capacity == 2)
        {
            return 1;
        }
        else
        {
            foreach (var allowedRes in list)
            {
                switch (allowedRes.name)
                {
                    case "yellowMU":
                        return 0.33f;
                    case "blueMU":
                        return 0.67f;
                }
            }
        }
        return 0;
    }
    
    //function accept for converting a string into a float hash
    public float GetFloatHash(string input)
    {
        int hashCode = input.GetHashCode();
        float hashFloat = Convert.ToSingle(hashCode);
        return hashFloat;
    }

    public void ApplyDeadlockPenalty()
    {
        mlAgent.AddReward(-5f);
        mlAgent.EndEpisode();
    }


    public void SetActions(ActionSegment<int> acts)
    {
        discreteActions = acts;
        actionsReceived = true;
    }

}
