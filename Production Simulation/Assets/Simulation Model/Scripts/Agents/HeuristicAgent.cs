using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Agent that follows heuristic strategies. If the strategy is stupid, this agent is also stupid.
public class HeuristicAgent : BaseAgent
{
    // The strategy that the agent uses
    [SerializeField] protected Strategy _strategy;

    protected override GameObject Decide(GameObject caller, List<ModuleInformation> m_info, bool callerInFront)
    {
        if (_strategy == null)
        {
            // we call this civil disobedience
            Debug.LogWarning("No strategy selected. Will do nothing...");
            return null;
        }

        return _strategy.act(caller, m_info, callerInFront);
    }
}
