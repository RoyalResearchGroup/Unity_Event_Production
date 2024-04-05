using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Base class for heuristic strategies. But this implemented strategy is pretty stupid, so please don't use it.
[CreateAssetMenu]
public class Strategy : ScriptableObject
{
    // The list passed here is the action space. Maybe we should also provide the observation space to the strategies.
    public virtual GameObject act(GameObject caller, List<GameObject> options)
    {
        // this strategy is stupid, so it will always take the last option
        return options.Last();
    }
}
