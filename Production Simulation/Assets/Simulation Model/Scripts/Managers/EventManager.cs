using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// The event manager handles the execution of events. Each frame update, ONE event is processed from the list and the time advanced.
/// </summary>
[RequireComponent(typeof(TimeManager))]
public class EventManager : MonoBehaviour
{
    //Event list. Here, we can insert events
    [SerializeField]
    public SortedList m_events;

    //Time manager
    private TimeManager m_timeManager;

    //Performance optimization: If theres performance overhead, process events in batches
    [SerializeField]
    private int batchSize = 1;
    [SerializeField]
    public float simSpeed = 1;

    private int stepCounter = 0;

    [SerializeField]
    public bool createStatistic = true;

    private bool experimentRunning = false;


    private void Start()
    {
        m_timeManager = GetComponent<TimeManager>();
        m_events = GetComponent<SortedList>();
    }

    private void Update()
    {
        if (!experimentRunning) return;


        stepCounter++;
        if (stepCounter > 1/simSpeed)
        {
            stepCounter = 0;
        }
        else
        {
            return;
        }
        int counter = 0;
        while(counter < batchSize)
        {
            //Here, we process the first event in the list
            if (m_events.PeekEvent() != null)
            {
                //DEBUG
                //Debug.Log(m_events.PrintEvents());
                //Pop the first event
                Event m_event = m_events.PopEvent();
                //Notify the module it was dispatched from (generally just a state change)
                m_event.m_module.EventCallback(m_event);

                //Update the time
                //First: Update the time of all events contained in the list
                m_events.SubtractTimeFromEvents(m_event.m_executionTime);
                //Second: Update the global time by adding the time
                m_timeManager.ProgressTime(m_event.m_executionTime);


                //Some modules have to be notified that the event was processed (eg source, drain)
                BroadcastMessage("NotifyEventBatch");
                counter++;
            }
            else
            {
                break;
            }
        }

    }

    public void StartExperiment()
    {
        experimentRunning = true;
    }

    public void StopExperiment()
    {
        m_events.Clear();
        stepCounter = 0;
        experimentRunning = false;
    }
    //Function to add an event to the manager from the modules
    public void EnqueueEvent(Event r_event)
    {
        m_events.AddEvent(r_event);
    }
}
