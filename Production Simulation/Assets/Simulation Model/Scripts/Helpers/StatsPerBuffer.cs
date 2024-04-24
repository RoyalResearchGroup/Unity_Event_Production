using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StatsPerBuffer
{
    public Module module;
    private List<float> averageFill = new List<float>();

    public StatsPerBuffer(Module name, float _averageFill)
    {
        module = name;
        averageFill.Add(_averageFill);
    }

    public void Add(float _averageFill)
    {
        averageFill.Add(_averageFill);
    }

    public List<float> getValuesInPosition(int i)
    {
        return new List<float> { averageFill[i] };
    }

    public int getCount()
    {
        return averageFill.Count;
    }

    public List<float> getAverage()
    {
        return new List<float> { averageFill.Average()};
    }
}
