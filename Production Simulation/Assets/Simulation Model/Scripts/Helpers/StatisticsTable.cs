using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class StatisticsTable : MonoBehaviour
{
    private List<StatisticRow> statistics = new List<StatisticRow>();

    public void addStatistic(StatisticRow statistic)
    {
        statistics.Add(statistic);
    }
}