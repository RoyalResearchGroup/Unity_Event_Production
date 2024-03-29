using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeuristicAgent : Agent
{
    [SerializeField] protected Strategy _strategy;

    protected override GameObject Decide(List<GameObject> options)
    {
        if (_strategy == null)
        {
            Debug.LogWarning("No strategy selected. Will do nothing...");
            return null;
        }

        return _strategy.act(options);
    }
}
