using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

[RequireComponent(typeof(ExperimentManager))]
public class StatisticsManager : MonoBehaviour
{
    [Tooltip("Directory where the results should be safed (root is 'Production Simulation'")]
    public string directory;
    private ExperimentManager x_manager;
    private StatisticTable stationTable;
    private StatisticTable bufferTable;
    private StatisticTable drainTable;
    private List<bool> experimentTable;

    private void Start()
    {
        x_manager = GetComponent<ExperimentManager>();
        stationTable = new StatisticTable(x_manager.iterations);
        bufferTable = new StatisticTable(x_manager.iterations);
        drainTable = new StatisticTable(x_manager.iterations);
        experimentTable = new List<bool>(x_manager.iterations);
    }

    public void addStationStatistics(Module m, Vector4 machineUsage)
    {
        stationTable.addStationStatistic(m,  machineUsage.x, machineUsage.y, machineUsage.z, machineUsage.w);
    }

    public void addDrainStatistics(Module m, float drainrate, float timePerProduct)
    {
        drainTable.addDrainStatistics(m, drainrate, timePerProduct);
    }

    public void addBufferStatistics(Module m, float averageFill)
    {
        bufferTable.addBufferStatistics(m, averageFill);
    }
    
    public void extractStatistics(bool experimentSuccessful)
    {
        BroadcastMessage("notifyStatisticsManager");
        experimentTable.Add(experimentSuccessful);
    }

    public void exportStatistics()
    {
        string dir = String.Format("./Assets/results/{0}/{1}", directory, gameObject.layer);
        Directory.CreateDirectory(dir);
        
        string filePath = Path.Combine(dir, "stationUsage.csv");
        stationTable.WriteToCSV(filePath, TYPE.STATION);
        filePath = Path.Combine(dir, "bufferUsage.csv");
        bufferTable.WriteToCSV(filePath, TYPE.BUFFER);
        filePath = Path.Combine(dir, "drainUsage.csv");
        drainTable.WriteToCSV(filePath, TYPE.DRAIN);
        filePath = Path.Combine(dir, "experiments.csv");
        ExportExperimentResults(filePath);
    }

    private void ExportExperimentResults(string filePath)
    {
        StringBuilder csvContent = new StringBuilder();

        if (experimentTable.Count != 0)
        {
            csvContent.AppendLine("Success;");

            foreach (var experiment in experimentTable)
            {
                csvContent.AppendLine("" + experiment + ";");
            }
            
            File.WriteAllText(filePath, csvContent.ToString());
        }
    }
}