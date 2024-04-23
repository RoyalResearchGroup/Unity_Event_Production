using UnityEngine;

public class StatisticsManager : MonoBehaviour
{
    private StatisticsTable statisticsTable;
    private StatisticRow statistic_row;
    public StatisticsManager ()
    {

    }
    public void addStationStatistics(Module m, Vector4 machineUsage)
    {
        statistic_row.addStatistic(m.name,  machineUsage.w, machineUsage.y, machineUsage.x, machineUsage.z);
    }
    public void extractStatisticsOfExperimentEnd()
    {
        BroadcastMessage("notifyStatisticsManager");
        statisticsTable.addStatistic(statistic_row);
        statistic_row = new StatisticRow();
    }

    public void extractStatisticsOfExperimentStep()
    {
        BroadcastMessage("notifyStatisticsManager");
        statisticsTable.addStatistic(statistic_row);
        statistic_row = new StatisticRow();
    }
}