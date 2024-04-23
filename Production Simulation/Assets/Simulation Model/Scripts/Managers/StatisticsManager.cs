using System.IO;
using UnityEngine;

public class StatisticsManager : MonoBehaviour
{
    private StatisticTable statistic_row = new StatisticTable();
    public void addStationStatistics(Module m, Vector4 machineUsage)
    {
        statistic_row.addStationStatistic(m,  machineUsage.w, machineUsage.y, machineUsage.x, machineUsage.z);
    }

    public void addDrainStatistics(Module m, float drainrate, float timePerProduct)
    {
        statistic_row.addDrainStatistics(m, drainrate, timePerProduct);
    }

    public void addBufferStatistics(Module m, float averageFill)
    {
        statistic_row.addBufferStatistics(m, averageFill);
    }
    public void extractStatistics()
    {
        BroadcastMessage("notifyStatisticsManager");
    }

    public void exportStatistics()
    { 
        string filePath = Path.Combine("Assets/results", "UwU_Ich_Will_Nicht_Mehr.csv");
        statistic_row.WriteToCSV(filePath);
    }
}