using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeuristicAgent : Agent
{
    [SerializeField] private Strategy _strategy;

    protected override GameObject Decide(List<GameObject> options)
    {
        return base.Decide(options);
    }
}
