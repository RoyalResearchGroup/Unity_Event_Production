using System.Collections;
using System.Collections.Generic;
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

    private void Update()
    {
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
}
