using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Experiment : ScriptableObject
{
    //Check if the experiment is successful
    public abstract bool EvaluateState(ExperimentManager man, List<Module> observations);
}
