using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "NewStrategy", menuName = "Strategies/SimpleStrategy")]
public class SimpleStrategy : Strategy
{
    public override GameObject act(GameObject caller, List<ModuleInformation> m_info)
    {
        //The Strategy should return a viable machine. 
        //This simple example strategy will show how the m_info list can be used to find a fitting target.

        GameObject target = null;

        //Iterate through the options:
        foreach(ModuleInformation info in m_info)
        {
            //Take the first valid option
            if(info.valid && info.ready)
            {
                target = info.module;
                break;
            }
        }
        return target;
    }
}
