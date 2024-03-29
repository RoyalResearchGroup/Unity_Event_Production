using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RLAgent : Agent
{
    protected override GameObject Decide(List<GameObject> options)
    {
        // Here, the neural network needs to make a choice

        return base.Decide(options);
    }
}
