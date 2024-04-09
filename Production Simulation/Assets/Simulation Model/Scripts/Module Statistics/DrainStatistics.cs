using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Drain))]
public class DrainStatistics : Statistics
{
    public float drainRate;

    public void NotifyEventBatch()
    {
        if (!useStatistics) return;
        drainRate = GetComponent<Drain>().absoluteDrain / (t_manager.time+1);
    }
}
