using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCharts.Runtime;

/// <summary>
/// Statistic extension component for the Drain
/// </summary>
[RequireComponent(typeof(Drain))]
public class DrainStatistics : Statistics
{
    public float drainRate;
    public float timePerProduct;

    public void NotifyEventBatch()
    {
        if (!useStatistics) return;
        drainRate = GetComponent<Drain>().absoluteDrain / (t_manager.time+0.000000001f);
        timePerProduct = 1 / drainRate;
    }

    public override void ResetModule()
    {
        drainRate = 0;
        timePerProduct = 0;   
    }

    public override void notifyStatisticsManager()
    {
        StatisticsManager statisticsManager = GetComponentInParent<StatisticsManager>();
        statisticsManager.addDrainStatistics(GetComponent<Module>(), drainRate, timePerProduct);
    }
}
