using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using System;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System.Runtime.CompilerServices;



class ActionStorage
{
    

}


[RequireComponent(typeof(MLInterface))]
public class RLAgent : BaseAgent
{
    public Drain obs;

    private int maxDrain = 0;
    
    // The strategy that the agent uses
    [SerializeField] protected Strategy _strategy;
    MLInterface mlAgent;
    EventManager e_manager;
    GameObject caller;
    bool callerInFront;

    ActionSegment<int> discreteActions;
    bool actionsReceived = false;

    /// <summary>
    /// This is brutally stupid but the only workarround.
    /// The RL agent will keep track of a specified number of last actions. if a deadlock occurs, the python backend 
    /// will recieve "fake" steps to train the penalty of these.
    /// </summary>
    [SerializeField] private int actionstackSize = 10;

    //Need to store the actions by storing the simulation state -> We can request the decisions again, if they match, we penalize them, if not, no reward/penalty
    private List<ActionStorage> actionBuffer;
    private bool inject = false;


    public override void Start()
    {
        base.Start();
        mlAgent = GetComponent<MLInterface>();
        e_manager = GetComponentInParent<EventManager>();

        actionBuffer = new List<ActionStorage>();
    }

    /*public void NotifyEventBatch()
    {
        Academy.Instance.EnvironmentStep();
    }*/

    public override Module DetermineAction(GameObject caller, bool callerInFront)
    {
        this.caller = caller;
        this.callerInFront = callerInFront;
        return base.DetermineAction(caller, callerInFront);
    }

    protected override GameObject Decide(GameObject caller, List<ModuleInformation> m_info, bool callerInFront)
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
        //Academy.Instance.EnvironmentStep();
        // Wait for the actions to be received using a coroutine
        StartCoroutine(WaitForActions());
        //WaitForActions();

        return null;
    }

    
    // Implement WaitForActions coroutine
    IEnumerator WaitForActions()
    {
        mlAgent.RequestDecision();
        Academy.Instance.EnvironmentStep();

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

            //mlAgent.AddReward(0.1f);

            if (output == m_info.Count)
            {
                // no module selected (do nothing)
                Debug.Log("Do nothing...");

                //Get the number of currently working machines
                int c = 0;
                int d = 0;
                foreach(ModuleInformation m in m_info)
                {
                    if(m.valid && m.ready)
                    {
                        c++;
                    }
                    if (m.valid) d++;
                }
                if(c == 0)
                {
                    mlAgent.AddReward(0.4f); //+ decStep*0.2f);
                    Debug.Log("None perfected!");
                    //mlAgent.EndEpisode();
                }
                else
                {
                    mlAgent.AddReward(0.05f);
                    //mlAgent.AddReward(obs.absoluteDrain * 0.5f);

                    if (c == d)
                    {
                        mlAgent.AddReward(-0.5f);
                        //mlAgent.AddReward(obs.absoluteDrain * 0.3f);
                        GetComponentInParent<ExperimentManager>().StopExperiment();
                        mlAgent.EndEpisode();
                    }
                    //mlAgent.EndEpisode();
                }
                //mlAgent.AddReward(0.0f);
                e_manager.Pause(false);
            }
            else if (m_info[output].valid && m_info[output].ready)
            {
                // valid action received, return the corresponding module
                //return m_info[output].module;
                Module callerM = caller.GetComponent<Module>();
                Module decisionM = m_info[output].module.GetComponent<Module>();
                Debug.Log("Valid action selected");
                mlAgent.AddReward(0.1f); // + decStep * 0.2f);
                //mlAgent.EndEpisode();

                //Academy step here - Update might call the agent again!
                //Academy.Instance.EnvironmentStep();

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

                //e_manager.CheckDeadlock();

                e_manager.Pause(false);
            }
            else
            {
                // give penalty for invalid action
                Debug.LogWarning("Invalid action selected.");

                /*string str = "Observations: ";
                foreach (var module in m_info)
                {
                    str += (module.valid && module.ready) + " ";                    
                }
                // give penalty for invalid action
                Debug.LogWarning(str);*/


                // add penalty
                mlAgent.AddReward(-0.5f);
                //mlAgent.AddReward(obs.absoluteDrain * 0.2f);
                //mlAgent.AddReward(obs.absoluteDrain * 0.5f);
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
        //Academy.Instance.EnvironmentStep();
    }
    

    public void CollectObservations(VectorSensor sensor)
    {
        //INJECTOR: Enables us to add fake inputs overriding the environments inputs
        if (inject)
        {

            return;
        }


        List<float> inputs = new List<float>();


        //string str = "OBS: ";
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
                    //float inp = System.Convert.ToSingle(info.valid && info.ready);
                    sensor.AddObservation(info.valid && info.ready);
                    if(info.valid && info.ready) inputs.Add(1);
                    else inputs.Add(0);
                    //str += info.valid && info.ready;
                    break;
            }
        }
        sensor.AddObservation(callerInFront);
        //Debug.Log(str);
    }

    public void ApplyDeadlockPenalty()
    {
        //Not too high, we dont want a random decision to be penalized
        mlAgent.AddReward(-0.5f);
        //mlAgent.AddReward((obs.absoluteDrain-maxDrain) * 0.75f);
        Debug.LogWarning("Deadlock detected!");
        if(maxDrain < obs.absoluteDrain )
        {
            //mlAgent.AddReward(obs.absoluteDrain * 0.5f);
            maxDrain = obs.absoluteDrain;
        }
        mlAgent.EndEpisode();


    }


    public void SetActions(ActionSegment<int> acts)
    {
        discreteActions = acts;
        actionsReceived = true;
    }

}
