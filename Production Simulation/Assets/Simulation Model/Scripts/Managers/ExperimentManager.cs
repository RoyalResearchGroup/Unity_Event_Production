using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExperimentManager : MonoBehaviour
{
    public int iterations;
    [Tooltip("Amount of experiments using the same random seed per iteration.")]
    public int epochs; //Epochs per iteration are the amount of experiments using the same random seed per iteration (useful for ML agents) -> increasing this will multiply the amount of iterations

    public Experiment experiment;

    public List<Module> observationSpace;

    private int seed;
    private bool running = false;
    private EventManager e_manager;

    //UI
    public void StartExperiment()
    {
        running = true;
        GetComponent<EventManager>().StartExperiment();
    }

    public void StopExperiment()
    {
        running = false;
        GetComponent<EventManager>().StopExperiment();
        ResetScene();
    }
    //________________________________________________

    private void Start()
    {
        e_manager = GetComponent<EventManager>();
    }

    public void NotifyEventBatch()
    {
        if (!e_manager.createStatistic) return;
        if(experiment.EvaluateState(this, observationSpace))
        {
            StopExperiment();
        }
    }

    public void ResetScene()
    {
        //BroadcastMessage("CallbackIllegalAction");
        BroadcastMessage("ResetModule");
    }
}
