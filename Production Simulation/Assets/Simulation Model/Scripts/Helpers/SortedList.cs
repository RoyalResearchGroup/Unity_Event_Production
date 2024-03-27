using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// List to keep in the events. An event is inserted by iterating through the list from top to bottom and insert it at the right position
/// /// </summary>
[System.Serializable]
public class SortedList : MonoBehaviour
{
    private List<Event> events = new List<Event>();

    // Method to add an event in the sorted order based on execution time
    public void AddEvent(Event newEvent)
    {
        if (events.Count == 0)
        {
            events.Add(newEvent);
        }
        else
        {
            bool added = false;
            for (int i = 0; i < events.Count; i++)
            {
                if (newEvent.m_executionTime < events[i].m_executionTime)
                {
                    events.Insert(i, newEvent);
                    added = true;
                    break;
                }
            }

            if (!added)
            {
                events.Add(newEvent);
            }
        }
    }

    // Method to retrieve and remove the first event in the list
    public Event PopEvent()
    {
        if (events.Count > 0)
        {
            Event firstEvent = events[0];
            events.RemoveAt(0);
            return firstEvent;
        }
        return null;
    }

    // Peek at the first event without removing it
    public Event PeekEvent()
    {
        return events.Count > 0 ? events[0] : null;
    }

    //Adjust time
    public void SubtractTimeFromEvents(float timeValue)
    {
        foreach (var eventEntry in events)
        {
            eventEntry.m_executionTime = Mathf.Max(eventEntry.m_executionTime - timeValue, 0.0f);
        }
    }

    //DEB
    public string PrintEvents()
    {
        string s_out = "";
        foreach (var eventObj in events)
        {
            s_out += $"Event: {eventObj.m_eventType}, ExecutionTime: {eventObj.m_executionTime}";
        }
        return s_out;
    }
}
