using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewExperiment", menuName = "Experiments/RateExperiment")]
public class RateExperiment : Experiment
{
    //The rate needed to finish the experiment.
    public float targetRate = 1f;
    public override bool EvaluateState(ExperimentManager man, List<Module> observations)
    {
        //avg rate
        /*float rate = 0;
        int drainCount = 0;
        foreach (Module m in observations)
        {
            if (m.gameObject.GetComponent<DrainStatistics>())
            {
                rate += m.gameObject.GetComponent<DrainStatistics>().drainRate;
                drainCount++;
            }
        }
        if(drainCount > 0)
        {
            rate /= drainCount;
        }

        if(rate >= targetRate)
        {
            return true;
        }*/
        return false;
    }
}
