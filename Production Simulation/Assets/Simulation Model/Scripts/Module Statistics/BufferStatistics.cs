using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BufferStatistics : Statistics
{
    public float averageFill;
    private float absoluteFill;

    public Dictionary<Resource, int> aggregatedResources = new Dictionary<Resource, int>();
    public int resourceSum = 0;

    public Dictionary<int, float> bufferFill = new Dictionary<int, float>();


    public void NotifyEventBatch()
    {
        if (!useStatistics) return;

        if (bufferFill.ContainsKey(GetComponent<Buffer>().GetRBufferFillDEBUG()))
        {
            bufferFill[GetComponent<Buffer>().GetRBufferFillDEBUG()] += t_manager.deltaTime;
        }
        else
        {
            //Debug.Log(simObject.gameObject.GetComponent<Buffer>().GetRBufferFillDEBUG());
            bufferFill.Add(GetComponent<Buffer>().GetRBufferFillDEBUG(), t_manager.deltaTime);
        }


        var currentInputBuffer = GetComponent<Module>().GetResourceBuffer();
        // Aggregate resources from the current input buffer for comparison.
        var res = currentInputBuffer
            .GroupBy(ro => ro.Resource)
            .ToDictionary(group => group.Key, group => group.Count());

        foreach (var kvp in res)
        {
            if (aggregatedResources.ContainsKey(kvp.Key))
                aggregatedResources[kvp.Key] += kvp.Value;
            else aggregatedResources.Add(kvp.Key, kvp.Value);

            resourceSum += kvp.Value;
        }

        absoluteFill += GetComponent<Buffer>().GetRBufferFillDEBUG() * t_manager.deltaTime;
        averageFill = absoluteFill / t_manager.time;
    }

    public override void ResetModule()
    {
        averageFill = 0;
        absoluteFill = 0;
        resourceSum = 0;
        aggregatedResources.Clear();
        bufferFill.Clear();
    }

    public override void notifyStatisticsManager()
    {
        
    }
}
