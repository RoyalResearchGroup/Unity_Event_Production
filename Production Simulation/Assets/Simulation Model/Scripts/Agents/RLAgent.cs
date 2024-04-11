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

    private void LateUpdate()
    {
        
    }

    public new Module DetermineAction(GameObject caller, bool callerInFront)
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
            var output = discreteActions[0];


            if (output < 0 || output >= m_info.Count)
            {
                // give penalty for invalid action
                Debug.LogWarning("Action out of range");
                mlAgent.EndEpisode();
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
                Debug.Log("Valid action selected: " + decisionM.name);
                if (callerInFront)
                {
                    // maybe the caller could not be ready to get input (if it is dogshit)
                    decisionM.UpdateCTRL(callerM);
                }
                else
                {
                    callerM.UpdateCTRL(decisionM);
                }
                mlAgent.AddReward(0.5f);
            }
            else
            {
                // give penalty for invalid action
                Debug.LogWarning("Invalid action selected: " + output);
                // add penalty
                mlAgent.AddReward(-0.1f);
                mlAgent.EndEpisode();
                GetComponentInParent<ExperimentManager>().StopExperiment();

            }
        }
        else
        {
            Debug.LogWarning("No actions received");
        }

        // Reset the flag for the next decision
        actionsReceived = false;
        e_manager.Pause(false);
    }
    

    public void CollectObservations(VectorSensor sensor)
    {
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
                    //float inp = System.Convert.ToSingle(info.valid && info.ready);
                    sensor.AddObservation(info.valid && info.ready);
                    break;
            }
        }
    }


    public void SetActions(ActionSegment<int> acts)
    {
        discreteActions = acts;
        actionsReceived = true;
    }

    public class CoroutineAwaiter : INotifyCompletion
    {
        private readonly MonoBehaviour _host;
        private readonly IEnumerator _coroutine;
        private Action _continuation;

        public CoroutineAwaiter(MonoBehaviour host, IEnumerator coroutine)
        {
            _host = host;
            _coroutine = coroutine;
        }

        public CoroutineAwaiter GetAwaiter()
        {
            return this;
        }

        public bool IsCompleted
        {
            get { return false; }
        }

        public void OnCompleted(Action continuation)
        {
            _continuation = continuation;
            _host.StartCoroutine(InvokeCoroutine());
        }

        private IEnumerator InvokeCoroutine()
        {
            yield return _host.StartCoroutine(_coroutine);
            _continuation?.Invoke();
        }

        public void GetResult()
        {
            // No result to return for coroutine
        }
    }

}
