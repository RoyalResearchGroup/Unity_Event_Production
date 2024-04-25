using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewExperiment", menuName = "Experiments/SimpleExperiment")]
public class SimpleExperiment : Experiment
{
    public override bool EvaluateState(ExperimentManager man, List<Module> observations)
    {
        if (((Drain)observations[0]).absoluteDrain > 100)
        {
            return true;
        }
        return false;
    }
}
