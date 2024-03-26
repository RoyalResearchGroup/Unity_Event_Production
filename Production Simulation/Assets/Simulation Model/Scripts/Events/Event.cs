using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EventType
{
    ENTER,
    EXIT
}


public class Event
{
    // The duration it takes to execute the Event.
    public float m_executionTime { get; set; }
    // Time at which the event was queued.
    public float m_startingTime { get; set; }
    // The module this event applies to.
    public Module m_module { get; set; }
    // The type of this event.
    public EventType m_eventType { get; private set; }

    // Constructor for the event.
    public Event(float executionTime, float startingTime, Module module, EventType eventType)
    {
        m_executionTime = executionTime;
        m_startingTime = startingTime;
        m_module = module;
        m_eventType = eventType;
    }
}
