using System;
using System.IO;
using UnityEngine;

[RequireComponent(typeof(ExperimentManager))]
public class StatisticsManager : MonoBehaviour
{
    private ExperimentManager x_manager;
    private StatisticTable stationTable;
    private StatisticTable bufferTable;
    private StatisticTable drainTable;

    private void Start()
    {
        x_manager = GetComponent<ExperimentManager>();
        stationTable = new StatisticTable(x_manager.iterations);
        bufferTable = new StatisticTable(x_manager.iterations);
        drainTable = new StatisticTable(x_manager.iterations);
    }

    public void addStationStatistics(Module m, Vector4 machineUsage)
    {
        stationTable.addStationStatistic(m,  machineUsage.w, machineUsage.y, machineUsage.x, machineUsage.z);
    }

    public void addDrainStatistics(Module m, float drainrate, float timePerProduct)
    {
        drainTable.addDrainStatistics(m, drainrate, timePerProduct);
    }

    public void addBufferStatistics(Module m, float averageFill)
    {
        bufferTable.addBufferStatistics(m, averageFill);
    }
    public void extractStatistics()
    {
        BroadcastMessage("notifyStatisticsManager");
    }

    public void exportStatistics()
    { 
        string filePath = Path.Combine("Assets/results", "stationUsage.csv");
        stationTable.WriteToCSV(filePath, TYPE.STATION);
        filePath = Path.Combine("Assets/results", "bufferUsage.csv");
        bufferTable.WriteToCSV(filePath, TYPE.BUFFER);
        filePath = Path.Combine("Assets/results", "drainUsage.csv");
        drainTable.WriteToCSV(filePath, TYPE.DRAIN);
    }
}