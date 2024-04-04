using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BufferStatistics : Statistics
{
    public float averageFill;
    private float absoluteFill;

    private void Update()
    {
        absoluteFill += GetComponent<Buffer>().GetRBufferFillDEBUG() * t_manager.deltaTime;
        averageFill = absoluteFill / t_manager.time;
    }


}
