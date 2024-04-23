using System.IO;
using UnityEngine;

public class StatisticsManager : MonoBehaviour
{
    private StatisticRow statistic_row = new StatisticRow();
    public void addStationStatistics(Module m, Vector4 machineUsage)
    {
        statistic_row.addStatistic(m,  machineUsage.w, machineUsage.y, machineUsage.x, machineUsage.z);
    }
    public void extractStatistics()
    {
        BroadcastMessage("notifyStatisticsManager");
    }

    public void exportStatistics()
    { 
        string filePath = Path.Combine("Assets/results", "UwU_Ich_Will_Nicht_Mehr_.csv");
        statistic_row.WriteToCSV(filePath);
    }
}