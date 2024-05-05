using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using System;
using System.Linq;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System.Runtime.CompilerServices;


[System.Serializable]
public class ActionStorage
{
    private List<float> input;
    private int output;

    public ActionStorage(List<float> input)
    {
        this.input = input;
    }

    public void SetOutput(int _output)
    {
        output = _output;
    }
    public int GetOutput()
    {
        return output;
    }

    public List<float> GetInput()
    {
        return input;
    }
}


[RequireComponent(typeof(MLInterface))]
public class RLAgent : BaseAgent
{
    public Drain obs;

    //Training settings
    [Header("Training Settings")]
    public bool train = true;
    public bool optimizeSetup = true;
    public bool optimizeTime = true;
    public bool optimizeOccupation = true;
    
    public bool observeResources = true;
    public bool observeSetup = true;

    [Header("Action Space Settings")]
    public bool allowOptionalAction = true;

    [Header("Strategy")]
    // The strategy that the agent uses
    [SerializeField] protected Strategy _strategy;
    MLInterface mlAgent;
    EventManager e_manager;
    GameObject caller;
    bool callerInFront;

    ActionSegment<int> discreteActions;

    /// <summary>
    /// This is brutally stupid but the only workarround.
    /// The RL agent will keep track of a specified number of last actions. if a deadlock occurs, the python backend 
    /// will recieve "fake" steps to train the penalty of these.
    /// </summary>
    [SerializeField] private int actionstackSize = 10;
    public int episodeLength = 10;
    private int currentEpisode = 0;

    //Need to store the actions by storing the simulation state -> We can request the decisions again, if they match, we penalize them, if not, no reward/penalty
    private List<ActionStorage> actionBuffer;
    private bool inject = false;
    private int currentAction = -1;


    public override void Start()
    {
        base.Start();
        mlAgent = GetComponent<MLInterface>();
        e_manager = GetComponentInParent<EventManager>();

        actionBuffer = new List<ActionStorage>();
    }

    public override Module DetermineAction(GameObject caller, bool callerInFront)
    {
        this.caller = caller;
        this.callerInFront = callerInFront;
        return base.DetermineAction(caller, callerInFront);
    }

    protected override GameObject Decide(GameObject caller, List<ModuleInformation> m_info, bool callerInFront)
    {
        mlAgent.RequestDecision();
        Academy.Instance.EnvironmentStep();

        if (discreteActions.Length > 0)
        {
            var output = discreteActions[0];
            if (!inject && train)
            {
                currentAction = actionBuffer.Count - 1;
                actionBuffer[currentAction].SetOutput(output);
            }

            if (output == m_info.Count)
            {
                // no module selected (do nothing)
                if (train)
                    Debug.Log("Do nothing...");

                //Get the number of currently working machines
                int c = 0;
                int d = 0;
                foreach (ModuleInformation m in m_info)
                {
                    if (m.valid && m.ready)
                    {
                        c++;
                    }
                    if (m.valid) d++;
                }
                if (c == 0)
                {
                    mlAgent.AddReward(1.0f);
                    if(train)
                        Debug.Log("None perfected!");
                }
                else
                {
                    if(allowOptionalAction)
                        mlAgent.AddReward(0.2f);
                        

                    if (c == d || !allowOptionalAction)
                    {
                        if (train)
                            Debug.LogWarning("<color=yellow>None false</color>");
                        mlAgent.AddReward(-2.0f);
                        GetComponentInParent<ExperimentManager>().StopExperiment();
                        GetComponentInParent<AgentManager>().EndEpisode();
                        ResetBuffer();
                    }
                }
                //GetComponentInParent<AgentManager>().EndEpisode();
                //mlAgent.EndEpisode();
            }
            else if (m_info[output].valid && m_info[output].ready)
            {
                // valid action received, return the corresponding module
                Module callerM = caller.GetComponent<Module>();
                Module decisionM = m_info[output].module.GetComponent<Module>();
                if (train)
                    Debug.Log("Valid action selected");
                mlAgent.AddReward(0.3f);


                //SPECIAL REWARDS FOR SPECIAL NEEDS
                //______________________________________________________________
                //Reward the agent for choosing diverse machines


                //Reward agent for choosing machines that are setup for the resource in question


                //Reward agent for choosing a machine that is fast

                if (optimizeSetup)
                {
                    if (callerInFront)
                    {
                        //Setup
                        if (caller.GetComponent<Module>().ResourceSetupBlueprint(m_info[output].product))
                        {
                            mlAgent.AddReward(1.0f);
                            //Debug.LogWarning("Bonus");
                        }
                        else
                        {
                            mlAgent.AddReward(-1.0f);
                        }
                    }
                    else
                    {
                        if (m_info[output].module.GetComponent<Module>().ResourceSetupBlueprint(caller.GetComponent<Module>().GetProduct()))
                        {
                            mlAgent.AddReward(1.0f);
                            //Debug.LogWarning("Bonus");
                        }
                        else
                        {
                            mlAgent.AddReward(-1.0f);
                        }
                    }
                }

                //mlAgent.EndEpisode();
                //GetComponentInParent<AgentManager>().EndEpisode();
                return m_info[output].module;
            }
            else
            {
                // give penalty for invalid action
                if (train)
                {
                    if(caller.GetComponent<Module>().GetProduct()!=null)
                        Debug.LogWarning("<color=blue>Invalid action selected. </color>" + caller.name + " " + caller.GetComponent<Module>().GetProduct().name);
                    else
                        Debug.LogWarning("<color=blue>Invalid action selected. </color>" + caller.name + " null");
                }


                // add penalty
                mlAgent.AddReward(-2.0f);
                //mlAgent.EndEpisode();
                ResetBuffer();
                GetComponentInParent<ExperimentManager>().StopExperiment();
                GetComponentInParent<AgentManager>().EndEpisode();
            }
        }
        else
        {
            if (train)
                Debug.LogWarning("No actions received");
        }
        //GetComponentInParent<AgentManager>().EndEpisode();
        /*if(currentEpisode == episodeLength)
        {
            currentEpisode = 0;
            mlAgent.EndEpisode();
        }
        else
        {
            currentEpisode++;
        }*/
        return null;
    }

    public void NotifyEndEpisode()
    {
        mlAgent.EndEpisode();
    }

    public void CollectObservations(VectorSensor sensor)
    {
        //INJECTOR: Enables us to add fake inputs overriding the environments inputs
        if (inject && train)
        {
            //The input now is the input saved in the action buffer
            foreach (var i in actionBuffer[currentAction].GetInput()) 
            {
                sensor.AddObservation(i);
            }
            return;
        }

        //REGULAR: Fill the observations as well as create a new actionStorage object in the action buffer
        List<float> inputs = new List<float>();

        foreach (ModuleInformation info in m_info)
        {
            sensor.AddObservation(info.valid && info.ready);
            //Used for the CA
            if (info.valid && info.ready) inputs.Add(1);
            else inputs.Add(0);

            float resourceObservation = 0f;
            if (observeResources)
            {
                if (!info.ready)
                {           
                    if(info.setup!=null)
                    {
                        if (info.setup.product.r_name == "BlueMU")
                        {
                            resourceObservation = 1f;
                        }
                        else
                        {
                            resourceObservation = -1f;
                        }
                    }
                }
                sensor.AddObservation(resourceObservation);
                inputs.Add(resourceObservation);
            }

            if (observeSetup)
            {
                if (callerInFront)
                {
                    bool obsv = caller.GetComponent<Module>().ResourceSetupBlueprint(info.product);
                    sensor.AddObservation(obsv);
                    if (obsv) inputs.Add(1);
                    else inputs.Add(0);
                }
                else
                {
                    bool obsv = info.module.GetComponent<Module>().ResourceSetupBlueprint(caller.GetComponent<Module>().GetProduct());
                    sensor.AddObservation(obsv);
                    if (obsv) inputs.Add(1);
                    else inputs.Add(0);
                }
            }
        }

        //Add a new Action to the buffer
        if(train)
            actionBuffer.Add(new ActionStorage(inputs));
    }

    public void ApplyFinishReward()
    {
        float rew = obs.absoluteDrain/GetComponentInParent<TimeManager>().time * 100.0f;
        mlAgent.AddReward(rew);
        mlAgent.EndEpisode();
    }
    
    public void UseStrategy(in ActionBuffers actionOut)
    {
        GameObject decision = _strategy.act(caller, m_info, callerInFront);
        var actions = actionOut.DiscreteActions;
        if (predecessors.Contains(decision))
        {
            actions[0] = predecessors.IndexOf(decision);
        }
        else if (successors.Contains(decision))
        {
            actions[0] = successors.IndexOf(decision) + predecessors.Count;
        }
        else if (decision == null)
        {
            actions[0] = successors.Count + predecessors.Count;
        }
        //Debug.Log("Action: " + actions[0]);

        //actionsReceived = true;
    }

    public void ApplyDeadlockPenalty()
    {
        if (train)
            Debug.LogWarning("<color=red>Deadlock detected!</color>");
        mlAgent.AddReward(-100.0f);
        mlAgent.EndEpisode();

        //We have to perform backtracking injection
        //Inject(actionstackSize, -1.0f);
        //After, reset the action buffer as the episode concludes
        ResetBuffer();
    }


    public void SetActions(ActionSegment<int> acts)
    {
        discreteActions = acts;
    }


    //Backtracking Injection
    public void Inject(int size, float reward)
    {
        //Debug.Log("<color=green> INJECTION!</color>");

        if (currentAction < 0 || !train)
        {
            return;
        }

        inject = true;
        //Perform the injection for the last <size> decisions
        for(int i = 0; i < size; i++)
        {
            mlAgent.RequestDecision();
            Academy.Instance.EnvironmentStep();
            var output = discreteActions[0];

            //Only do if the entry exists
            try
            {
                //0 cant be a problem for a deadlock (REPLACE LATER WITH IGNORE LIST
                /*if (output == 0)
                {
                    i--;
                    continue;
                }*/

                //Without penalizing 0, the penalties can be more aggressive
                if(actionBuffer[currentAction].GetOutput() == output)
                {
                    Debug.Log("Injection learned!");
                    mlAgent.AddReward(reward);
                }
            }
            catch (Exception e)
            {
                //Catchy catchy Marcel is edgy
                break;
            }
        }
        //End the injection episode
        inject = false;
        mlAgent.EndEpisode();
    }

    public void ResetBuffer()
    {
        actionBuffer.Clear();
        currentAction = -1;
    }

}
