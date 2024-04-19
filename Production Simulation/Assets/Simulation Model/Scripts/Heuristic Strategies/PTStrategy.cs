using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.VisualScripting.NativeUtility;
using static UnityEngine.GraphicsBuffer;

[CreateAssetMenu(fileName = "NewStrategy", menuName = "Strategies/PTStrategy")]
public class PTStrategy : Strategy
{
    // All valid and ready machines
    private List<GameObject> ready_options = new List<GameObject>();
    // All valid machines
    private List<GameObject> all_options = new List<GameObject>();


    public override GameObject act(GameObject caller, List<ModuleInformation> m_info, bool callerInFront)
    {
        //The Strategy should return a viable machine. 
        //This simple example strategy will show how the m_info list can be used to find a fitting target.

        GameObject target = null;
        // ready_options -> machine must be valid and ready
        ready_options = new List<GameObject>();
        // all_options -> machine must be valid
        all_options = new List<GameObject>();

        foreach (ModuleInformation info in m_info)
        {
            if (info.valid)
            {
                if (info.ready)
                {
                    ready_options.Add(info.module);
                }
                all_options.Add(info.module);
            }
        }

        // 1. Strategy
        if (ready_options.Count == 0)
        {
            return null;
        }

        // Is module that called the method a predessor or successor

        // ------------------------------------------------
        if (!callerInFront) // --> If caller is predecessor
        // ------------------------------------------------
        {
            // Can "many" successor handle the resource/product that should moved? 
            bool rareResource = false;
            // The resource or product that has to be moved
            Resource resource = caller.GetComponent<Module>().GetOutputResource();

            List<GameObject> selectedModules = new List<GameObject>();

            selectedModules = sortReadyOptionsWithProcessingTimePre(resource, ready_options);

            if (selectedModules.Count > 0)
            {
                target = selectedModules[0];
            }
        }
        // -------------------------------
        else // --> If caller is successor
        // -------------------------------
        {
            List<GameObject> selectedModules = new List<GameObject>();

            selectedModules = sortReadyOptionsWithProcessingTimeSuc(caller.GetComponent<Module>().GetModuleInformation(), ready_options);

            if (selectedModules.Count > 0)
            {
                target = selectedModules[0];
            }
        }
        return target;
    }

    // -------------------
    // Predecessor methods  
    // -------------------
    private List<GameObject> sortReadyOptionsWithProcessingTimePre(Resource resource, List<GameObject> ready_options)
    {
        // Sort all machines descendendly by the lowest processingtime for a blueprint that
        // contains the resource thats has to be transported  
        List<Tuple<GameObject, float>> moduleScores = new List<Tuple<GameObject, float>>();
        foreach (GameObject module in ready_options)
        {
            ModuleInformation m_info = module.GetComponent<Module>().GetModuleInformation();
            // Create List with all Blueprints and their processing time
            foreach (Blueprint temp_item in m_info.blueprints)
            {
                // If the resource that should be moved is used for the Blueprint
                if (temp_item.resources.Any(re => re.resource == resource))
                {
                    // Check if the gameobject has already an entry in the list
                    if (moduleScores.Any(t => t.Item1 == module))
                    {
                        //Find existing tupel with the same gameobject
                        var existingTupel = moduleScores.FirstOrDefault(t => t.Item1 == module);
                        // Check if the processing time of the current blueprint that can use the resource that has to be moved is
                        // smaller than the stored processing time for the same module
                        if (existingTupel.Item2 > temp_item.processingTime)
                        {
                            moduleScores.Remove(existingTupel);
                            moduleScores.Add(new Tuple<GameObject, float>(module, temp_item.processingTime));
                        }
                    }
                    else
                    {
                        moduleScores.Add(new Tuple<GameObject, float>(module, temp_item.processingTime));
                    }
                }
            }
        }
        return moduleScores.OrderByDescending(x => x.Item2).Select(x => x.Item1).ToList();
    }

    // ----------------
    // Successor method
    // ----------------
    private List<GameObject> sortReadyOptionsWithProcessingTimeSuc(ModuleInformation caller_info, List<GameObject> tempReadyOptions)
    {
        // Sort all machines descendendly by the lowest processingtime for a blueprint that
        // contains the resource thats has to be transported  
        List<Tuple<GameObject, float>> moduleScores = new List<Tuple<GameObject, float>>();
        foreach (GameObject module in tempReadyOptions)
        {
            ModuleInformation m_info = module.GetComponent<Module>().GetModuleInformation();
            Resource outputProduct = m_info.product;
            // Create List with all Blueprints and their processing time
            foreach (Blueprint temp_item in caller_info.blueprints)
            {
                // If the resource that should be moved is used for the Blueprint
                if (temp_item.resources.Any(re => re.resource == outputProduct))
                {
                    // Check if the gameobject has already an entry in the list
                    if (moduleScores.Any(t => t.Item1 == module))
                    {
                        //Find existing tupel with the same gameobject
                        var existingTupel = moduleScores.FirstOrDefault(t => t.Item1 == module);
                        // Check if the processing time of the current blueprint that can use the resource that has to be moved is
                        // smaller than the stored processing time for the same module
                        if (existingTupel.Item2 > temp_item.processingTime)
                        {
                            moduleScores.Remove(existingTupel);
                            moduleScores.Add(new Tuple<GameObject, float>(module, temp_item.processingTime));
                        }
                    }
                    else
                    {
                        moduleScores.Add(new Tuple<GameObject, float>(module, temp_item.processingTime));
                    }
                }
            }
        }
        return moduleScores.OrderByDescending(x => x.Item2).Select(x => x.Item1).ToList();
    }
}