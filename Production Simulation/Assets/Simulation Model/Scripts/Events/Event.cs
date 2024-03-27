using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EVENTTYPE
{
    PROCESS,
    CREATE
}

[System.Serializable]
public class Event
{
    // The duration it takes to execute the Event.
    public float m_executionTime { get; set; }
    // Time at which the event was queued.
    public Module m_module { get; set; }
    // The type of this event.
    public EVENTTYPE m_eventType { get; private set; }

    // Constructor for the event.
    public Event(float executionTime, Module module, EVENTTYPE eventType)
    {
        m_executionTime = executionTime;
        m_module = module;
        m_eventType = eventType;
    }
}
