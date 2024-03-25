using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event
{
    //How long does it take to execute the Event?
    public float m_executionTime { get; set; }
    //Time at which the event was queued
    public float m_startingTime { get; set; }

    public Event(float executionTime, float startingTime)
    {
        m_executionTime = executionTime;
        m_startingTime = startingTime;

    }
}
