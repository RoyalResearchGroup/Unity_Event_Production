using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class StatisticRow : MonoBehaviour
{
    private List<string> moduleNames = new List<string>();
    private List<float> available = new List<float>();
    private List<float> setup = new List<float>();
    private List<float> blocked = new List<float>();
    private List<float> occupied = new List<float>();
    public void addStatistic(string _moduleNames, float _available, float _setup, float _blocked, float _occupied)
    {
        moduleNames.Add(_moduleNames);
        available.Add(_available);
        setup.Add(_setup);
        blocked.Add(_blocked);
        occupied.Add(_occupied);
    }

    public void WriteToCSV(string filePath)
    {
        StringBuilder csvContent = new StringBuilder();
        csvContent.AppendLine("ModuleName,Available,Setup,Blocked,Occupied");

        for (int i = 0; i < moduleNames.Count; i++)
        {
            csvContent.AppendLine($"{moduleNames[i]},{available[i]},{setup[i]},{blocked[i]},{occupied[i]}");
        }

        File.WriteAllText(filePath, csvContent.ToString());
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