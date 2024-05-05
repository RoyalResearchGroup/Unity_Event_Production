using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "NewStrategy", menuName = "Strategies/Strategy02")]
public class Strategy02 : Strategy
{
    [SerializeField]
    public List<Resource> usedResources = new List<Resource>();
    void OnEnable()
    {
        try
        {
            // Set amount of resources which the agent has to handle!
            if (usedResources.Count() != 2)
            {
                throw new ArgumentException("Chipi Chipi Chapa Chapa");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("EXCEPTION: Add all to handle resources to strategy!!! -> Assets/Simulation Model/Scriptable Objects/Strategies");
            //wrongAmountOfResources = false;
        }
    }

    public override GameObject act(GameObject caller, List<ModuleInformation> m_info, bool callerInFront)
    {
        Dictionary<string, GameObject> ready_options = new Dictionary<string, GameObject>();
        Dictionary<string, GameObject> all_options = new Dictionary<string, GameObject>();
        List<string> successorNames = new List<string> { "Station 1", "Station 2", "Station 3", "Station 4" };
        List<string> predecessorNames = new List<string> { "Buffer 1", "Buffer 2" };

        GameObject target = null;
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
                    else if (ready_options.ContainsKey(successorNames[2]) && ready_options.ContainsKey(successorNames[3]))
                    {
                        all_options.TryGetValue(successorNames[2], out GameObject gameObject0);
                        all_options.TryGetValue(successorNames[3], out GameObject gameObject1);
                        if (!(gameObject0.GetComponent<Module>().GetModuleInformation().product == usedResources[0]
                            || gameObject1.GetComponent<Module>().GetModuleInformation().product == usedResources[0]
                            || gameObject0.GetComponent<Module>().GetModuleInformation().resourceBuffer.Any(resourceObject => resourceObject.Resource == usedResources[0])
                            || gameObject1.GetComponent<Module>().GetModuleInformation().resourceBuffer.Any(resourceObject => resourceObject.Resource == usedResources[0])))
                        {
                            ready_options.TryGetValue(successorNames[rand.Next(2) + 2], out target);
                        }
                    }
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
                    }
                    else if (ready_options.ContainsKey(successorNames[0]) && ready_options.ContainsKey(successorNames[1]))
                    {
                        all_options.TryGetValue(successorNames[0], out GameObject gameObject0);
                        all_options.TryGetValue(successorNames[1], out GameObject gameObject1);
                        if (!(gameObject0.GetComponent<Module>().GetModuleInformation().product == usedResources[1]
                            || gameObject1.GetComponent<Module>().GetModuleInformation().product == usedResources[1]
                            || gameObject0.GetComponent<Module>().GetModuleInformation().resourceBuffer.Any(resourceObject => resourceObject.Resource == usedResources[1])
                            || gameObject1.GetComponent<Module>().GetModuleInformation().resourceBuffer.Any(resourceObject => resourceObject.Resource == usedResources[1])))
                        {
                            ready_options.TryGetValue(successorNames[rand.Next(2)], out target);
                        }
                    }
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