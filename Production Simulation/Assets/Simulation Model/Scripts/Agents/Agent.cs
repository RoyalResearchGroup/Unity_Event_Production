using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Agent : SimulationObject
{
    // Start is called before the first frame update
    void Start()
    {
        // IMPORTANT: Otherwise modules will not be able to see that this is an agent
        SetSTATE(STATE.AGENT);
        SetupLists();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Module DetermineAction(bool callerInFront)
    {
        var options = callerInFront ? predecessors : successors;
        if(options == null || options.Count == 0)
        {
            return null;
        }
        GameObject decision = Decide(options);
        if (!decision)
            return null;

        Module chosen;
        if (decision.TryGetComponent<Module>(out chosen))
        {
            return chosen;
        }

        Agent followUp;
        if (decision.TryGetComponent<Agent>(out followUp))
        {
            return followUp.DetermineAction(callerInFront);
        }

        return null;
    }

    // This method decides what action to take. For now, an action means moving a MU from a predecessor to a successor.
    // This method may be called either by one of the Agents successors or one of the agent's predecessors. Predecessors
    // prioritize calling the agent rather than other connected modules, while successors act contrarily.
    // This Method should return the module that the agent has chosen. The calling module can then validate that the
    // chosen module is in a valid state and can perform MoveToModule / MoveFromModule from then on.
    // Successors of the agent pass callerInFront = true, predecessor pass false.
    protected virtual GameObject Decide(List<GameObject> options)
    {
        GameObject chosen = null;
        
        // This base agent is stupid, so it will always choose the last machine
        chosen = options.Last();
        
        // Here the NN agent would ask the network for an output. Integrating the training process into the simulation could be expensive.
        // The heuristic agent could work with strategies, the strategies are scriptable objects that one can assign to the heuristic agent.

        return chosen;
    }

    public override SimulationObject InputCTRL()
    {
        throw new System.NotImplementedException();
    }

    public override SimulationObject OutputCTRL(Resource r)
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateCTRL()
    {
        throw new System.NotImplementedException();
    }
}
