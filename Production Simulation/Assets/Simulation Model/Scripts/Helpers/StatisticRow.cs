using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class StatisticRow
{ 
    private List<StatsPerModule> statsOfModules = new List<StatsPerModule>();
    public void addStatistic(Module _module, float _available, float _setup, float _blocked, float _occupied)
    {
        if (!statsOfModules.Any(module => module.module == _module))
        {
            statsOfModules.Add(new StatsPerModule(_module, _available, _setup, _blocked, _occupied));
        }
        else
        {
            statsOfModules[statsOfModules.FindIndex(module => module.module == _module)].Add(_available, _setup, _blocked, _occupied);
        }
    }

    public void WriteToCSV(string filePath)
    {
        StringBuilder csvContent = new StringBuilder();
        List<Module> moduleNames = statsOfModules.Select(module => module.module).ToList();
        StringBuilder row = new StringBuilder();

        foreach (Module module in moduleNames)
        {
            csvContent.Append(module + ";;;;");
        }
        csvContent.AppendLine();

        for (int i = 0; i < moduleNames.Count(); i++)
        {
            csvContent.Append("Available;Setup;Blocked;Occupied;");
        }
        csvContent.AppendLine();

        for (int i = 0; i < statsOfModules[0].getCount(); i++)
        {
            foreach (var module in statsOfModules)
            {
                foreach (float value in module.getValuesInPosition(i))
                {
                    row.Append(value + ";");
                }
            }
            csvContent.AppendLine(row.ToString());
            row = new StringBuilder();
        }

        csvContent.AppendLine("Average: ");
        row = new StringBuilder();
        foreach (var module in statsOfModules)
        {
            foreach (float value in module.getAverage())
            {
                row.Append(value + ";");
            }
        }
        csvContent.AppendLine(row.ToString());
        File.WriteAllText(filePath, csvContent.ToString());
    }
}