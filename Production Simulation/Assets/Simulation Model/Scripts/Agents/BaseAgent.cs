using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

// Base class for all agents. It is pretty stupid, so subagents should not call base.Decide().
public class BaseAgent : SimulationObject
{

    protected List<ModuleInformation> m_info = new List<ModuleInformation>();
    private GameObject d_caller;


    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        // IMPORTANT: Otherwise modules will not be able to see that this is an agent
        SetSTATE(STATE.AGENT);
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
    public virtual Module DetermineAction(GameObject caller, bool callerInFront)
    {
        d_caller = caller;
        GetObservationInformation(callerInFront, caller);

        //var options = callerInFront ? predecessors : successors;
        if(m_info == null || m_info.Count == 0)
        {
            return null;
        }
        GameObject decision = Decide(caller, m_info, callerInFront);
        if (!decision)
            return null;

        Module chosen;
        if (decision.TryGetComponent<Module>(out chosen))
        {
            return chosen;
        }

        BaseAgent followUp;
        if (decision.TryGetComponent<BaseAgent>(out followUp))
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
    protected virtual GameObject Decide(GameObject caller, List<ModuleInformation> m_info, bool callerInFront)
    {
        GameObject chosen = null;
        
        // This base agent is stupid, so it will always choose the last machine
        chosen = m_info.Last().module;
        
        // Here the NN agent would ask the network for an output. Integrating the training process into the simulation could be expensive.
        // The heuristic agent could work with strategies, the strategies are scriptable objects that one can assign to the heuristic agent.

        return chosen;
    }

    //Iterate through the environment to get information required by the agents. These can access this information through the Agent top level class.
    private void GetObservationInformation(bool callerInFront, GameObject caller)
    {
        m_info.Clear();

        foreach(GameObject pred in predecessors)
        {
            ModuleInformation temp_info = pred.GetComponent<Module>().GetModuleInformation();
            temp_info.module = pred;
            if (callerInFront)
            {
                temp_info.valid = true;
            }
            else
            {
                temp_info.valid = false;
            }

            temp_info.ready = pred.GetComponent<Module>().IsOutputReady(caller.GetComponent<Module>().GetAcceptedResources());
            m_info.Add(temp_info);
        }
        foreach (GameObject suc in successors)
        {
            ModuleInformation temp_info = suc.GetComponent<Module>().GetModuleInformation();
            temp_info.module = suc;
            if (callerInFront)
            {
                temp_info.valid = false;
            }
            else 
            {
                temp_info.valid = true; 
            }
            temp_info.ready = suc.GetComponent<Module>().IsInputReady(caller.GetComponent<Module>().GetOutputResource());
            m_info.Add(temp_info);
        }
    }

    public virtual void CallbackIllegalAction()
    {
        //DUBIDU
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
