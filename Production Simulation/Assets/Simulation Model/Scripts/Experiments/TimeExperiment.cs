using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewExperiment", menuName = "Experiments/TimeExperiment")]
public class TimeExperiment : Experiment
{
    //Special check for Date (For now simple, can be extended)
    //CONCEPT
    public float expectedDate;
    private float fullfilledDate;
    public int expectedAmount;
    public bool achievedDate = false;
    private bool isEvaluated = false;
    private int currentAmount;
    private float guessedTime;

    public override bool EvaluateState(ExperimentManager man, List<Module> observations)
    {
        //achievedDate = false;
        foreach (Module m in observations)
        {
            if (m.gameObject.GetComponent<Drain>())
            {
                currentAmount = m.gameObject.GetComponent<Drain>().absoluteDrain;
                if(currentAmount >= expectedAmount && !isEvaluated)
                { 
                    if(man.GetComponent<TimeManager>().time <= expectedDate)
                    {
                        achievedDate = true;
                    }
                    isEvaluated = true;
                }

                if (isEvaluated)
                {
                    fullfilledDate = man.GetComponent<TimeManager>().time;
                    if (achievedDate)
                    {
                        Debug.Log("<color=green>Time limit achieved!</color>");
                    }
                    else
                    {
                        Debug.Log("<color=orange>Time limit failed!</color>");
                    }
                    //Calculate the expected time for the Amount requirement to be fullfilled
                    guessedTime = m.gameObject.GetComponent<DrainStatistics>().timePerProduct * expectedAmount;
                    return true;
                }
            }
        }
        
        return false;
    }
}
