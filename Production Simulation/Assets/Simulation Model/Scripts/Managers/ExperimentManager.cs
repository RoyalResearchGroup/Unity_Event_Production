using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class ExperimentManager : MonoBehaviour
{
    public int iterations;
    private int iterationCount;
    [Tooltip("Amount of experiments using the same random seed per iteration.")]
    public int epochs; //Epochs per iteration are the amount of experiments using the same random seed per iteration (useful for ML agents) -> increasing this will multiply the amount of iterations
    public Experiment experimentTemplate;

    private Experiment experiment;

    public List<Module> observationSpace;

    private int seed;
    private bool running = false;
    private EventManager e_manager;

    //UI
    public void StartExperiment()
    {
        Random.InitState((int)Random.Range(0f, 1000f));
        running = true;
        experiment = Instantiate(experimentTemplate);
        GetComponent<EventManager>().StartExperiment();
    }

    public void StopExperiment()
    {
        running = false;
        iterationCount++;
        if (iterationCount >= iterations)
        {
            Debug.Log("<color=green>Experiment succeeded!</color>");
        }
        GetComponent<EventManager>().StopExperiment();
        ResetScene();
    }
    //________________________________________________

    private void Start()
    {
        e_manager = GetComponent<EventManager>();
        StartExperiment();
    }

    private void Update()
    {
        if (!running && iterationCount < iterations)
        {
            StartExperiment();
        }
    }

    public void NotifyEventBatch()
    {
        if (!e_manager.createStatistic) return;
        if(experiment.EvaluateState(this, observationSpace))
        {
            Debug.LogWarning("Iteration completed!");
            StopExperiment();
        }
    }

    public void ResetScene()
    {
        //BroadcastMessage("CallbackIllegalAction");
        BroadcastMessage("ResetModule");
    }
}
