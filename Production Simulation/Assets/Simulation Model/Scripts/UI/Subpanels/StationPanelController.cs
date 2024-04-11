using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using XCharts.Runtime;
using static UnityEngine.EventSystems.EventTrigger;

public class StationPanelController : PanelController
{
    [SerializeField]
    private PieChart usageChart;
    [SerializeField]
    private BarChart resourceChart;

    private void Awake()
    {
        //Bad solution, fix after release
        usageChart.chartName = Time.time + " benis";
        resourceChart.chartName = Time.time + " benis2";
    }

    // Update with data
    public void NotifyEventBatch()
    {
        usageChart.ClearData();
        Vector4 usageData = ((Station)simObject).gameObject.GetComponent<StationStatistics>().machineUsage;
        usageChart.AddData(0, Mathf.Round(usageData.x * 100), "Empty");
        usageChart.AddData(0, Mathf.Round(usageData.y * 100), "Setup");
        usageChart.AddData(0, Mathf.Round(usageData.z * 100), "Blocked");
        usageChart.AddData(0, Mathf.Round(usageData.w * 100), "Occupied");

        resourceChart.ClearData();
        foreach(var entry in ((Station)simObject).gameObject.GetComponent<StationStatistics>().aggregatedResources)
        {
            resourceChart.AddXAxisData(entry.Key.r_name);
            resourceChart.AddData(0, Mathf.Round(((float)entry.Value / ((Station)simObject).gameObject.GetComponent<StationStatistics>().resourceSum)*100));
        }
    }
}
