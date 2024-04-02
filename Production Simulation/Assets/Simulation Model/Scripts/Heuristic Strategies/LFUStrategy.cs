using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class LFUStrategy : Strategy
{
    public override GameObject act(GameObject caller, List<GameObject> options)
    {

        // implement LFU

        return base.act(caller, options);
    }
}
