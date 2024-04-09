using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCharts.Runtime;

public class BufferPanel : PanelController
{
    [SerializeField]
    private BarChart usageChart;
    [SerializeField]
    private BarChart resourceChart;

    private void Awake()
    {
        //Bad solution, fix after release
        usageChart.chartName = Time.time + " benis3";
        resourceChart.chartName = Time.time + " benis4";
    }

    // Update with data
    public void NotifyEventBatch()
    {
        usageChart.ClearData();
        foreach (var entry in ((Buffer)simObject).gameObject.GetComponent<BufferStatistics>().bufferFill)
        {
            usageChart.AddXAxisData(""+entry.Key);
            usageChart.AddData(0, Mathf.Round(((float)entry.Value)*100.0f / t_manager.time));
        }

        resourceChart.ClearData();
        foreach (var entry in ((Buffer)simObject).gameObject.GetComponent<BufferStatistics>().aggregatedResources)
        {
            resourceChart.AddXAxisData(entry.Key.r_name);
            resourceChart.AddData(0, Mathf.Round(((float)entry.Value / ((Buffer)simObject).gameObject.GetComponent<BufferStatistics>().resourceSum) * 100));
        }
    }
}
