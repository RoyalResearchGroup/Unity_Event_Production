using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class Strategy : ScriptableObject
{
    public virtual GameObject act(List<GameObject> options)
    {
        // this strategy is stupid, so it will always take the last option
        return options.Last();
    }
}
