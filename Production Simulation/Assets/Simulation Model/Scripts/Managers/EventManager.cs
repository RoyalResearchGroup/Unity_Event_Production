using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// The event manager handles the execution of events. Each frame update, ONE event is processed from the list and the time advanced.
/// </summary>
public class EventManager : MonoBehaviour
{
    //Event list. Here, we can insert events
    [SerializeField]
    public SortedList m_events;

    //Time manager
    private TimeManager m_timeManager;


    private void Start()
    {
        m_timeManager = GameObject.FindWithTag("TimeManager").GetComponent<TimeManager>();
        m_events = GetComponent<SortedList>();
    }

    private void Update()
    {
        //Here, we process the first event in the list
        if(m_events.PeekEvent() != null)
        {
            //Pop the first event
            Event m_event = m_events.PopEvent();
            //Notify the module it was dispatched from (generally just a state change)
            m_event.m_module.EventCallback(m_event);

            //Update the time
            //First: Update the time of all events contained in the list
            m_events.SubtractTimeFromEvents(m_event.m_executionTime);
            //Second: Update the global time by adding the time
            m_timeManager.ProgressTime(m_event.m_executionTime);
            //Else, nothing to do here! The modules will update the simulation state automatically
        }

    }


    //Function to add an event to the manager from the modules
    public void EnqueueEvent(Event r_event)
    {
        m_events.AddEvent(r_event);
    }
}
