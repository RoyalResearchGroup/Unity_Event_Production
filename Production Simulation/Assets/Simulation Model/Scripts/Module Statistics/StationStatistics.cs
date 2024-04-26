using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Station))]
public class StationStatistics : Statistics
{

    //Vector displaying percentages of usage for machines. Currently: SC!!!!
    //- EMPTY/AVAILABLE (= Waiting)
    //- SETUP
    //- BLOCKED
    //- OCCUPIED
    [HideInInspector]
    private Vector4 absoluteUsage = Vector4.zero;
    public Vector4 machineUsage = Vector4.zero;

    //Resources
    public Dictionary<Resource, int> aggregatedResources = new Dictionary<Resource, int>();
    public int resourceSum = 0;




    public void NotifyEventBatch()
    {
        if (!useStatistics) return;
        var currentInputBuffer = GetComponent<Module>().GetResourceBuffer();
        // Aggregate resources from the current input buffer for comparison.
        var res = currentInputBuffer
            //.Where(ro => ro.Resource != null)
            .GroupBy(ro => ro.Resource)
            .ToDictionary(group => group.Key, group => group.Count());


        foreach ( var kvp in res)
        {
            if(aggregatedResources.ContainsKey(kvp.Key))
                aggregatedResources[kvp.Key] += kvp.Value;
            else aggregatedResources.Add(kvp.Key, kvp.Value);

            resourceSum += kvp.Value;
        }

        //Get the current status and the delta time
        float dt = t_manager.deltaTime;
        float abs_time = t_manager.time;
        STATE state = GetComponent<Station>().GetSTATE();

        //Get the setup ration
        float sr = GetComponent<Station>().setupRatio;

        switch (state)
        {
            case STATE.AVAILABLE:
                absoluteUsage.x += dt;
                break;
            case STATE.BLOCKED:
                absoluteUsage.z += dt;
                break;
            case STATE.OCCUPIED:
                absoluteUsage.w += dt - dt * sr;
                absoluteUsage.y += dt * sr;
                break;
        }

        machineUsage = new Vector4(absoluteUsage.x/abs_time, absoluteUsage.y / abs_time, absoluteUsage.z / abs_time, absoluteUsage.w / abs_time);
    }

    public override void ResetModule()
    {
        absoluteUsage = Vector4.zero;
        machineUsage = Vector4.zero;
        aggregatedResources.Clear();
        resourceSum = 0;
    }
}
