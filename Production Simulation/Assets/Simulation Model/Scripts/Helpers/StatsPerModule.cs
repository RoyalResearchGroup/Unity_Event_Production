using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StatsPerModule
{
    public Module module { get; set; }
    private List<float> available = new List<float>();
    private List<float> setup = new List<float>();
    private List<float> blocked = new List<float>();
    private List<float> occupied = new List<float>();

    public StatsPerModule(Module name, float _available, float _setup, float _blocked, float _occupied)
    {
        module = name;
        available.Add(_available);
        setup.Add(_setup);
        blocked.Add(_blocked);
        occupied.Add(_occupied);
    }

    public void Add(float _available, float _setup, float _blocked, float _occupied) {
        available.Add(_available);
        setup.Add(_setup);
        blocked.Add(_blocked);
        occupied.Add(_occupied);
    }

    public List<float> getValuesInPosition(int i)
    {   
        return new List<float> { available[i], setup[i], blocked[i], occupied[i] };
    }

    public int getCount()
    {
        return available.Count;
    }

    public List<float> getAverage()
    {
        return new List<float> { available.Average(), setup.Average(), blocked.Average(), occupied.Average()};
    }
}
