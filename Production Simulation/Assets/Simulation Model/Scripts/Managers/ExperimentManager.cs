using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
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
        UnityEngine.Random.InitState((int)UnityEngine.Random.Range(0f, 1000f));
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
        StartExperiment();
    }

    private void Update()
    {
        if (!running)
        {
            StartExperiment();
        }
    }

    public void NotifyEventBatch()
    {
        if (!e_manager.createStatistic) return;
        if(experiment.EvaluateState(this, observationSpace))
        {
            Debug.LogWarning("<color=green>Experiment succeeded!</color>");
            StopExperiment();
        }
    }

    public void ResetScene()
    {
        //BroadcastMessage("CallbackIllegalAction");
        BroadcastMessage("ResetModule");
    }
}
