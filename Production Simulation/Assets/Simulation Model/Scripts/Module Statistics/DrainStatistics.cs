using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Drain))]
public class DrainStatistics : Statistics
{
    public float drainRate;

    private void Update()
    {
        drainRate = GetComponent<Drain>().absoluteDrain / (t_manager.time+1);
    }
}
