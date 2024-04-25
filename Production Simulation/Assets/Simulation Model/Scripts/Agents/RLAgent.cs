using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using System;
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
        e_manager.Pause(true);

        // Wait for the actions to be received using a coroutine
        WaitForActions();

        return null;
    }

    
    // Implement WaitForActions coroutine
    private void WaitForActions()
    {
        mlAgent.RequestDecision();
        Academy.Instance.EnvironmentStep();


        if (discreteActions.Length > 0)
        {   
            var output = discreteActions[0];
            if(!inject)
            {
                currentAction = actionBuffer.Count - 1;
                actionBuffer[currentAction].SetOutput(output);
            }

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
                    mlAgent.AddReward(1.0f);
                    Debug.Log("None perfected!");
                }
                else
                {
                    mlAgent.AddReward(0.3f);

                    if (c == d)
                    {
                        Debug.LogWarning("<color=yellow>None false</color>");
                        mlAgent.AddReward(-1.0f);
                        GetComponentInParent<ExperimentManager>().StopExperiment();
                        //mlAgent.EndEpisode();
                        ResetBuffer();
                    }
                }
                mlAgent.EndEpisode();
                //mlAgent.AddReward(0.0f);
                e_manager.Pause(false);
            }
            else if (m_info[output].valid && m_info[output].ready)
            {
                // valid action received, return the corresponding module
                Module callerM = caller.GetComponent<Module>();
                Module decisionM = m_info[output].module.GetComponent<Module>();
                Debug.Log("Valid action selected");
                mlAgent.AddReward(0.1f);


                //SPECIAL REWARDS FOR SPECIAL NEEDS
                //______________________________________________________________
                //Reward the agent for choosing diverse machines


                //Reward agent for choosing machines that are setup for the resource in question


                //Reward agent for choosing a machine that is fast

                if (callerInFront)
                {
                    //Setup
                    if (caller.GetComponent<Module>().ResourceSetupBlueprint(m_info[output].product))
                    {
                        mlAgent.AddReward(0.2f);
                        //Debug.LogWarning("Bonus");
                    }
                    else
                    {
                        mlAgent.AddReward(-0.3f);
                    }
                }
                else
                {
                    if (m_info[output].module.GetComponent<Module>().ResourceSetupBlueprint(caller.GetComponent<Module>().GetProduct()))
                    {
                        mlAgent.AddReward(0.2f);
                        //Debug.LogWarning("Bonus");
                    }
                    else
                    {
                        mlAgent.AddReward(-0.3f);
                    }
                }


                //______________________________________________________________

                if (callerInFront)
                {
                    // maybe the caller could not be ready to get input (if it is dogshit)
                    decisionM.UpdateCTRL(callerM);
                }
                else
                {
                    callerM.UpdateCTRL(decisionM);
                }
                mlAgent.EndEpisode();
                e_manager.Pause(false);
            }
            else
            {
                // give penalty for invalid action
                Debug.LogWarning("<color=blue>Invalid action selected.</color>");


                // add penalty
                mlAgent.AddReward(-1.0f);
                mlAgent.EndEpisode();
                ResetBuffer();
                GetComponentInParent<ExperimentManager>().StopExperiment();
            }
        }
        else
        {
            Debug.LogWarning("No actions received");
            e_manager.Pause(false);
        }
    }
    

    public void CollectObservations(VectorSensor sensor)
    {
        //INJECTOR: Enables us to add fake inputs overriding the environments inputs
        if (inject)
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

            float resourceObservation = 0f;

            if(!info.ready)
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


            //Used for the CA
            if(info.valid && info.ready) inputs.Add(1);
            else inputs.Add(0);
            inputs.Add(resourceObservation);

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

        //Add a new Action to the buffer
        actionBuffer.Add(new ActionStorage(inputs));
    }



    public void ApplyDeadlockPenalty()
    {
        Debug.LogWarning("<color=red>Deadlock detected!</color>");

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

        if (currentAction < 0)
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
                if (output == 0)
                {
                    i--;
                    continue;
                }

                //Without penalizing 0, the penalties can be more aggressive
                if(actionBuffer[currentAction].GetOutput() == output)
                {
                    Debug.Log("Injection learned!");
                    mlAgent.AddReward(reward);
                }
                else
                {
                    mlAgent.AddReward(-0.01f);
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
