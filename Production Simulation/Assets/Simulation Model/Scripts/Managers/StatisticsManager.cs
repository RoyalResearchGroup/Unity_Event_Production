using UnityEngine;

public class StatisticsManager : MonoBehaviour
{
    private StatisticsTable statisticsTable;
    private Statistic statistic;
    public StatisticsManager ()
    {

    }
    public void addStationStatistics(Module m, Vector4 machineUsage)
    {
        statistic = new Statistic(m.name,  machineUsage.w, machineUsage.y, machineUsage.x, machineUsage.z);
    }
    public void extractModuleStatistics()
    {
        BroadcastMessage("notifyStatisticsManager");
        statisticsTable.addStatistic(statistic);
        statistic = new Statistic();
    }
}