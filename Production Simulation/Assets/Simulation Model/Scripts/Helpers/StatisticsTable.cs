using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class StatisticsTable : MonoBehaviour
{
    private List<Statistic> statistics = new List<Statistic>();

    public void addStatistic(Statistic statistic)
    {
        statistics.Add(statistic);
    }
}