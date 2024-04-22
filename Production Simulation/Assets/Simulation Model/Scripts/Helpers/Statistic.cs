using System.Collections.Generic;
using UnityEngine;

public class Statistic : MonoBehaviour
{
    private List<string> moduleNames = new List<string>();
    private List<float> available = new List<float>();
    private List<float> setup = new List<float>();
    private List<float> blocked = new List<float>();
    private List<float> occupied = new List<float>();

    public Statistic(string _moduleNames, float _available, float _setup, float _blocked, float _occupied)
    {
        moduleNames.Add(_moduleNames);
        available.Add(_available);
        setup.Add(_setup);
        blocked.Add(_blocked);
        occupied.Add(_occupied);
    }

    public Statistic() { }
    public void addStatistic(string _moduleNames, float _available, float _setup, float _blocked, float _occupied)
    {
        moduleNames.Add(_moduleNames);
        available.Add(_available);
        setup.Add(_setup);
        blocked.Add(_blocked);
        occupied.Add(_occupied);
    }

    public List<string> GetModuleNames()
    {
        return moduleNames;
    }

    public List<float> GetAvailable()
    {
        return available;
    }

    public List<float> GetSetup()
    {
        return setup;
    }

    public List<float> GetBlocked()
    {
        return blocked;
    }

    public List<float> GetOccupied()
    {
        return occupied;
    }
}