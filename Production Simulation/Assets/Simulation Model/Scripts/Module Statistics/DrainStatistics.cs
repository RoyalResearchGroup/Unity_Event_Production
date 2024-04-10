using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCharts.Runtime;

[RequireComponent(typeof(Drain))]
public class DrainStatistics : Statistics
{
    public float drainRate;
    public float timePerProduct;

    //Special check for Date (For now simple, can be extended)
    //CONCEPT
    public float expectedDate;
    public float fullfilledDate;
    public float expectedAmount;
    public bool achievedDate = false;
    private bool metReq = false;

    public float guessedTime;

    public void NotifyEventBatch()
    {
        if (!useStatistics) return;
        drainRate = GetComponent<Drain>().absoluteDrain / (t_manager.time+1);
        timePerProduct = 1 / drainRate;

        //We fullfilled the required amount of produts
        if (GetComponent<Drain>().absoluteDrain >= expectedAmount && !metReq)
        {
            //Did we fullfill the time?
            if(t_manager.time <= expectedDate) {
                achievedDate = true;
                fullfilledDate = t_manager.time;
            }
            else
            {
                achievedDate = false;
                fullfilledDate = t_manager.time;
            }
            metReq = true;
        }

        //Calculate the expected time for the Amount requirement to be fullfilled
        guessedTime = timePerProduct * expectedAmount;
    }
}
