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
    private int numberOfRecords;

    public StatisticTable(int maxRecords)
    {
        numberOfRecords = maxRecords;
    }
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

    public void WriteToCSV(string filePath, TYPE type)
    {
        StringBuilder csvContent = new StringBuilder();
        List<Module> stationNames = new List<Module>();
        List<Module> drainNames = new List<Module>();
        List<Module> bufferNames = new List<Module>();
        switch (type)
        {
            case TYPE.STATION:
                stationNames = statsOfStations.Select(module => module.module).ToList();
                break;
            case TYPE.BUFFER:
                bufferNames = statsOfBuffer.Select(module => module.module).ToList();
                break;
            case TYPE.DRAIN:
                drainNames = statsOfDrains.Select(module => module.module).ToList();
                break;
        }
        
        StringBuilder row = new StringBuilder();

        /*foreach (Module module in stationNames)
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

        csvContent.AppendLine();*/

        //for (int i = 0; i < stationNames.Count(); i++)
        if (stationNames.Count() != 0)
        {
            csvContent.Append("Name;Available;Setup;Blocked;Occupied;");
        }

        //for (int i = 0; i < drainNames.Count(); i++)
        if (drainNames.Count() != 0)
        {
            csvContent.Append("Name;Drainrate;Time per Product;");
        }

        //for (int i = 0; i < bufferNames.Count(); i++)
        if (bufferNames.Count() != 0)
        {
            csvContent.Append("Name;Average Fill;");
        }
        csvContent.AppendLine();

        for (int i = 0; i < numberOfRecords; i++)
        {
            foreach (var module in statsOfStations)
            {
                row.Append(module.module.gameObject.name + ";");
                foreach (float value in module.getValuesInPosition(i))
                {
                    row.Append(value + ";");
                }
                csvContent.AppendLine(row.ToString());
                row = new StringBuilder();
            }
            foreach (var module in statsOfDrains)
            {
                row.Append(module.module.gameObject.name + ";");
                foreach (float value in module.getValuesInPosition(i))
                {
                    row.Append(value + ";");
                }
                csvContent.AppendLine(row.ToString());
                row = new StringBuilder();
            }
            foreach (var module in statsOfBuffer)
            {
                row.Append(module.module.gameObject.name + ";");
                foreach (float value in module.getValuesInPosition(i))
                {
                    row.Append(value + ";");
                }
                csvContent.AppendLine(row.ToString());
                row = new StringBuilder();
            }
            //csvContent.AppendLine(row.ToString());
            //row = new StringBuilder();
        }
/*
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
        csvContent.AppendLine(row.ToString());*/
        File.WriteAllText(filePath, csvContent.ToString());
    }
}