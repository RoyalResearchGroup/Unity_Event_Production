using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "NewStrategy", menuName = "Strategies/Strategy01")]
public class Strategy01 : Strategy
{
    public override GameObject act(GameObject caller, List<ModuleInformation> m_info, bool callerInFront)
    {
        Dictionary<string, GameObject> ready_options = new Dictionary<string, GameObject>();
        Dictionary<string, GameObject> all_options = new Dictionary<string, GameObject>();
        List<string> successorNames = new List<string> { "MachineYB 1", "MachineYB 2", "MachineY", "MachineB" };
        List<string> predecessorNames = new List<string> { "BufferY", "BufferB" };
        List<string> resourceNames = new List<string> { "YellowMU", "BlueMU" };

        foreach (ModuleInformation info in m_info)
        {
            if (info.valid)
            {
                all_options.Add(info.module.GetComponent<Module>().name, info.module);
                if (info.ready)
                {
                    ready_options.Add(info.module.GetComponent<Module>().name, info.module);
                }
            }
        }

        if (ready_options.Count == 0)
        {
            return null;

        }
        

        GameObject target = null;

        if (!callerInFront)
        {
            System.Random rand = new System.Random();

            switch (caller.GetComponent<Module>().GetModuleInformation().product.name)
            {
                case "YellowMU":
                    if (ready_options.ContainsKey(successorNames[0]) || ready_options.ContainsKey(successorNames[1]))
                    {
                        if (ready_options.ContainsKey(successorNames[0]) && ready_options.ContainsKey(successorNames[1]))
                        {
                            ready_options.TryGetValue(successorNames[rand.Next(2)], out target);
                        }
                        else if (ready_options.ContainsKey(successorNames[0]))
                        {
                            ready_options.TryGetValue(successorNames[0], out target);

                        }
                        else if (ready_options.ContainsKey(successorNames[1]))
                        {
                            ready_options.TryGetValue(successorNames[1], out target);
                        }
                    }
                    /*else if (ready_options.ContainsKey(successorNames[2]) && ready_options.ContainsKey(successorNames[3]))
                    {
                        all_options.TryGetValue(successorNames[2], out GameObject gameObject0);
                        all_options.TryGetValue(successorNames[3], out GameObject gameObject1);
                        if (!(gameObject0.GetComponent<Module>().GetModuleInformation().product.name == resourceNames[0]
                            || gameObject1.GetComponent<Module>().GetModuleInformation().product.name == resourceNames[0]
                            || gameObject0.GetComponent<Module>().GetModuleInformation().resourceBuffer.Any(resourceObject => resourceObject.Resource.name == resourceNames[0])
                            || gameObject1.GetComponent<Module>().GetModuleInformation().resourceBuffer.Any(resourceObject => resourceObject.Resource.name == resourceNames[0])))
                        {
                            ready_options.TryGetValue(successorNames[rand.Next(2) + 2], out target);
                            Debug.Log("Blue"
                                + " : " + (gameObject0.GetComponent<Module>().GetModuleInformation().product.name == resourceNames[0])
                                + " : " + (gameObject1.GetComponent<Module>().GetModuleInformation().product.name == resourceNames[0])
                                + " : " + (gameObject0.GetComponent<Module>().GetModuleInformation().resourceBuffer.Any(resourceObject => resourceObject.Resource.name == resourceNames[0]))
                                + " : " + (gameObject0.GetComponent<Module>().GetModuleInformation().resourceBuffer.Any(resourceObject => resourceObject.Resource.name == resourceNames[0])));
                        }
                    }*/
                    break;
                case "BlueMU":
                    if (ready_options.ContainsKey(successorNames[2]) || ready_options.ContainsKey(successorNames[3]))
                    {
                        if (ready_options.ContainsKey(successorNames[2]) && ready_options.ContainsKey(successorNames[3]))
                        {
                            ready_options.TryGetValue(successorNames[rand.Next(2) + 2], out target);

                        }
                        else if (ready_options.ContainsKey(successorNames[2]))
                        {
                            ready_options.TryGetValue(successorNames[2], out target);
                        }
                        else if (ready_options.ContainsKey(successorNames[3]))
                        {
                            ready_options.TryGetValue(successorNames[3], out target);
                        }
                    }/*
                    else if (ready_options.ContainsKey(successorNames[0]) && ready_options.ContainsKey(successorNames[1]))
                    {
                        all_options.TryGetValue(successorNames[0], out GameObject gameObject0);
                        all_options.TryGetValue(successorNames[1], out GameObject gameObject1);
                        if (!(gameObject0.GetComponent<Module>().GetModuleInformation().product.name == resourceNames[1] 
                            || gameObject1.GetComponent<Module>().GetModuleInformation().product.name == resourceNames[1]
                            || gameObject0.GetComponent<Module>().GetModuleInformation().resourceBuffer.Any(resourceObject => resourceObject.Resource.name == resourceNames[1])
                            || gameObject1.GetComponent<Module>().GetModuleInformation().resourceBuffer.Any(resourceObject => resourceObject.Resource.name == resourceNames[1])))
                        {
                            ready_options.TryGetValue(successorNames[rand.Next(2)], out target);
                            Debug.Log("Blue" 
                                + " : " + (gameObject0.GetComponent<Module>().GetModuleInformation().product.name == resourceNames[1])
                                + " : " + (gameObject1.GetComponent<Module>().GetModuleInformation().product.name == resourceNames[1])
                                + " : " + (gameObject0.GetComponent<Module>().GetModuleInformation().resourceBuffer.Any(resourceObject => resourceObject.Resource.name == resourceNames[1]))
                                + " : " + (gameObject0.GetComponent<Module>().GetModuleInformation().resourceBuffer.Any(resourceObject => resourceObject.Resource.name == resourceNames[1])));
                        }
                    }*/
                    break;
            }
        }
        else
        {
            string callerName = caller.GetComponent<Module>().name;
            if (callerName == successorNames[0] || callerName == successorNames[1])
            {
                if (ready_options.ContainsKey(predecessorNames[0]))
                {
                    ready_options.TryGetValue(predecessorNames[0], out target);
                }
            }
            else if (callerName == successorNames[2] || callerName == successorNames[3])
            {
                if (ready_options.ContainsKey(predecessorNames[1]))
                {
                    ready_options.TryGetValue(predecessorNames[1], out target);
                }
            }
        }
        return target;
    }
}