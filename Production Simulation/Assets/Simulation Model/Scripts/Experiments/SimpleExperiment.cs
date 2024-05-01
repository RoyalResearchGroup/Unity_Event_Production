using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewExperiment", menuName = "Experiments/SimpleExperiment")]
public class SimpleExperiment : Experiment
{
    public int maxTarget = 20;

    public override bool EvaluateState(ExperimentManager man, List<Module> observations)
    {
        if (((Drain)observations[0]).absoluteDrain > maxTarget)
        {
            return true;
        }
        return false;
    }
}
