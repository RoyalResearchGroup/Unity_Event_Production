using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

[CreateAssetMenu(fileName = "NewStrategy", menuName = "Strategies/RLAgentStrategy")]
public class RLStrategy : Strategy
{

    private RLAgent rlAgent;

    private void Start()
    {
        //rlAgent = GetComponent<RLAgent>();
    }
    public override GameObject act(GameObject caller, List<ModuleInformation> m_info)
    {
        //The Strategy should return a viable machine. 
        //This simple example strategy will show how the m_info list can be used to find a fitting target.

        GameObject target = null;

        //Iterate through the options:
        foreach (ModuleInformation info in m_info)
        {
            //Take the first valid option
            if (info.valid && info.ready)
            {
                target = info.module;
                break;
            }
        }


        //caller.GetComponent<MLAgent>().CollectObservations(sensor);



        return target;
    }
}
