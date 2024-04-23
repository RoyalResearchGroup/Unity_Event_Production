using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class StatisticTable
{ 
    private List<StatsPerStation> statsOfStations = new List<StatsPerStation>();
    private List<StatsPerDrain> statsOfDrains = new List<StatsPerDrain>();
    private List<StatsPerBuffer> statsOfBuffer = new List<StatsPerBuffer>();
    public void addStationStatistic(Module _module, float _available, float _setup, float _blocked, float _occupied)
    {
        if (!statsOfStations.Any(module => module.module == _module))
        {
            statsOfStations.Add(new StatsPerStation(_module, _available, _setup, _blocked, _occupied));
        }
        else
        {
            statsOfStations[statsOfStations.FindIndex(module => module.module == _module)].Add(_available, _setup, _blocked, _occupied);
        }
    }

    public void addDrainStatistics(Module _module, float _drainrate, float _timePerProduct)
    {
        if (!statsOfDrains.Any(module => module.module == _module))
        {
            statsOfDrains.Add(new StatsPerDrain(_module, _drainrate, _timePerProduct));
        }
        else
        {
            statsOfDrains[statsOfDrains.FindIndex(module => module.module == _module)].Add(_drainrate, _timePerProduct);
        }
    }

    public void addBufferStatistics(Module _module, float _averageFill)
    {
        {
            if (!statsOfBuffer.Any(module => module.module == _module))
            {
                statsOfBuffer.Add(new StatsPerBuffer(_module, _averageFill));
            }
            else
            {
                statsOfBuffer[statsOfBuffer.FindIndex(module => module.module == _module)].Add(_averageFill);
            }
        }
    }

    public void WriteToCSV(string filePath)
    {
        StringBuilder csvContent = new StringBuilder();
        List<Module> stationNames = statsOfStations.Select(module => module.module).ToList();
        List<Module> drainNames = statsOfDrains.Select(module => module.module).ToList();
        List<Module> bufferNames = statsOfBuffer.Select(module => module.module).ToList();
        StringBuilder row = new StringBuilder();

        foreach (Module module in stationNames)
        {
            csvContent.Append(module + ";;;;");
        }

        foreach (Module module in drainNames)
        {
            csvContent.Append(module + ";;");
        }
        
        foreach (Module module in bufferNames)
        {
            csvContent.Append(module + ";");
        }

        csvContent.AppendLine();

        for (int i = 0; i < stationNames.Count(); i++)
        {
            csvContent.Append("Available;Setup;Blocked;Occupied;");
        }

        for (int i = 0; i < drainNames.Count(); i++)
        {
            csvContent.Append("Drainrate; Time per Product;");
        }

        for (int i = 0; i < bufferNames.Count(); i++)
        {
            csvContent.Append("Average Fill;");
        }
        csvContent.AppendLine();

        for (int i = 0; i < statsOfStations[0].getCount(); i++)
        {
            foreach (var module in statsOfStations)
            {
                foreach (float value in module.getValuesInPosition(i))
                {
                    row.Append(value + ";");
                }
            }
            foreach (var module in statsOfDrains)
            {
                foreach (float value in module.getValuesInPosition(i))
                {
                    row.Append(value + ";");
                }
            }
            foreach (var module in statsOfBuffer)
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
        foreach (var module in statsOfStations)
        {
            foreach (float value in module.getAverage())
            {
                row.Append(value + ";");
            }
        }

        foreach (var module in statsOfDrains)
        {
            foreach (float value in module.getAverage())
            {
                row.Append(value + ";");
            }
        }
        
        foreach (var module in statsOfBuffer)
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