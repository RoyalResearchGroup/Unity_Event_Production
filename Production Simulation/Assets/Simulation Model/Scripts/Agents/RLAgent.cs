using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RLAgent : BaseAgent
{
    protected override GameObject Decide(GameObject caller, List<ModuleInformation> m_info)
    {
        // Here, the neural network needs to make a choice

        return base.Decide(caller, m_info);
    }
}
