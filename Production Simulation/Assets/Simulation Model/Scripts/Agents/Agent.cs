using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Base class for all agents. It is pretty stupid, so subagents should not call base.Decide().
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

    // Successors of the agent pass callerInFront = true, predecessors pass false.
    // This method determines the action space for the agent using a boolean value. (It could also derive the observation space from the boolean.)
    // it then calls Decide() to choose a GameObject from the action space.
    // if the chosen object is a module, that is returned.
    // In case the chosen object is another agent, it is also asked for an action. This recursively repeats until an agent selects a module.
    public Module DetermineAction(GameObject caller, bool callerInFront)
    {
        var options = callerInFront ? predecessors : successors;
        if(options == null || options.Count == 0)
        {
            return null;
        }
        GameObject decision = Decide(caller, options);
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
            return followUp.DetermineAction(gameObject, callerInFront);
        }

        return null;
    }

    // This method decides what action to take. For now, an action means moving a MU from a predecessor to a successor.
    // This method may be called either by one of the Agents successors or one of the agent's predecessors. Predecessors
    // prioritize calling the agent rather than other connected modules, while successors act contrarily.
    // This Method should return the module that the agent has chosen. The calling module can then validate that the
    // chosen module is in a valid state and can perform MoveToModule / MoveFromModule from then on.
    protected virtual GameObject Decide(GameObject caller, List<GameObject> options)
    {
        GameObject chosen = null;
        
        // This base agent is stupid, so it will always choose the last machine
        chosen = options.Last();
        
        // Here the NN agent would ask the network for an output. Integrating the training process into the simulation could be expensive.
        // The heuristic agent could work with strategies, the strategies are scriptable objects that one can assign to the heuristic agent.

        return chosen;
    }

    // These aren't used at all, maybe we should move these methods from the SimulationObject class
    // to the Module class.
    public override SimulationObject InputCTRL(List<Resource> r)
    {
        return null;
    }

    public override SimulationObject OutputCTRL(Resource r)
    {
        return null;
    }

    public override void UpdateCTRL(Module m = null)
    {
       
    }

    public override bool IsInputReady(Resource r)
    {
        return false;
    }

    public override bool IsOutputReady(List<Resource> r)
    {
        return false;
    }
}
