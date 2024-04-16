using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewExperiment", menuName = "Experiments/TimeManager")]
public class TimeExperiment : Experiment
{
    //Special check for Date (For now simple, can be extended)
    //CONCEPT
    public float expectedDate;
    public float fullfilledDate;
    public float expectedAmount;
    public bool achievedDate = false;
    private bool metReq = false;

    public float guessedTime;

    public override bool EvaluateState(ExperimentManager man, List<Module> observations)
    {
        return false;
        /*achievedDate = true;
        foreach (Module m in observations)
        {
            if (m.gameObject.GetComponent<DrainStatistics>())
            {
                if(m.gameObject.GetComponent<Drain>().absoluteDrain >= expectedAmount && !metReq)
                {
                    if(man.GetComponent<TimeManager>().time <= expectedDate)
                    {
                        achievedDate = false;
                        break;
                    }
                    metReq = true;
                }
            }
        }
        if(achievedDate)
        {
            fullfilledDate = man.GetComponent<TimeManager>().time;
        }
        //Calculate the expected time for the Amount requirement to be fullfilled
        guessedTime = m.gameObject.GetComponent<DrainStatistics>(). * expectedAmount;*/
    }
}
