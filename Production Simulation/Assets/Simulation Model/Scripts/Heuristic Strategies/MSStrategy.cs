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

[CreateAssetMenu(fileName = "NewStrategy", menuName = "Strategies/MSStrategy")]
public class MSStrategy : Strategy
{
    // All valid and ready machines
    private List<GameObject> ready_options = new List<GameObject>();
    // All valid machines
    private List<GameObject> all_options = new List<GameObject>();


    // Factor for rarity!
    // -> must be between 0 and 1!
    // When is a resource rare?
    // -> The higher the more likely a resource is handled as rare
    [SerializeField]
    public float resourceRarityFactor = 0.5f;
    // When should a different strategy be choosen because of to many rare resources
    // -> The higher the more likely a strategy for a system with rare machine is choosen
    [SerializeField]
    public float rareStrategyFactor = 0.5f;
    // When should a machine be saved because of its use of rare resources
    // -> The higher the more likely the maschine is handled as a rare machine
    [SerializeField]
    public float rareMaschineFactor = 0.5f;
    //Increases the sensitivity a strategy with less machines
    // -> The higher the higher the sensitivity
    [SerializeField]
    public float increasedFactorSensitivity = 0.9f;


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

        // Dictionary for the resources and the usage in every successor
        Dictionary<Resource, int> toHandleResources = new Dictionary<Resource, int>();

        // Is module that called the method a predessor or successor

        // ------------------------------------------------
        if (!callerInFront) // --> If caller is predecessor
        // ------------------------------------------------
        {

            // Can "many" successor handle the resource/product that should moved? 
            bool rareResource = false;
            // The resource or product that has to be moved
            Resource resource = caller.GetComponent<Module>().GetOutputResource();

            foreach (ModuleInformation info in m_info)
            {
                if (info.valid)
                {
                    foreach (Resource temp_resource in info.input)
                    {
                        if (toHandleResources.ContainsKey(temp_resource))
                        {
                            toHandleResources[temp_resource]++;
                        }
                        else
                        {
                            toHandleResources.Add(temp_resource, 1);
                        }
                    }
                }
            }

            // Depending on the amount of successor a different strategy is used
            int amountOfValidSuc = 0;
            amountOfValidSuc = toHandleResources.TryGetValue(resource, out amountOfValidSuc) ? amountOfValidSuc : 0;

            // -----------
            // 2. strategy
            // -----------
            if (amountOfValidSuc == 1 && ready_options.Count == 1)
            {
                return ready_options[0];
            }

            // -----------
            // 3. strategy
            // -----------
            if (amountOfValidSuc > 4)
            {
                // List that will be sorted at the end according to modules with a low number of rare resources 
                List<GameObject> selectedModules = new List<GameObject>();
                // Temporare List to save resources that can only be processed by few machines
                List<Resource> rareProcessingResource = getRareResourcesPre(toHandleResources, increasedFactorSensitivity, resource, ref rareResource);

                // A strategy is selected based on the number of rare resources 
                if (rareProcessingResource.Count > (all_options.Count * rareStrategyFactor * increasedFactorSensitivity))
                {
                    selectedModules = sortReadyOptionsWithRarityPre(rareProcessingResource, increasedFactorSensitivity, rareResource);
                }
                else
                {
                    // All possible options are sorted by the amount of rare resources the machine can handle
                    selectedModules = sortReadyOptionsWithProcessingTimePre(resource);
                }
                if (selectedModules.Count > 0)
                {
                    target = selectedModules[0];
                }
            }

            // -----------
            // 4. strategy
            // -----------
            else if (amountOfValidSuc > 1)
            {
                // List that will be sorted at the end according to modules with a low number of rare resources 
                List<GameObject> selectedModules = new List<GameObject>();
                // Temporare List to save resources that can only be processed by few machines
                List<Resource> rareProcessingResource = getRareResourcesPre(toHandleResources, 1f, resource, ref rareResource);

                // A strategy is selected based on the number of rare resources 
                if (rareProcessingResource.Count > (all_options.Count * rareStrategyFactor))
                {
                    selectedModules = sortReadyOptionsWithRarityPre(rareProcessingResource, 1f, rareResource);
                }
                else
                {
                    // All possible options are sorted by the processing time
                    selectedModules = sortReadyOptionsWithProcessingTimePre(resource);
                }
                if (selectedModules.Count > 0)
                {
                    target = selectedModules[0];
                }
            }
        }
        // -------------------------------
        else // --> If caller is successor
        // -------------------------------
        {
            int amountOfSuc = all_options.Count;

            // -----------
            // 2. Strategy
            // -----------
            if (amountOfSuc == 1 && ready_options.Count == 1)
            {
                return ready_options[0];
            }

            foreach (ModuleInformation info in m_info)
            {
                if (!info.valid)
                {
                    foreach (Resource temp_resource in info.input)
                    {
                        if (toHandleResources.ContainsKey(temp_resource))
                        {
                            toHandleResources[temp_resource]++;
                        }
                        else
                        {
                            toHandleResources.Add(temp_resource, 1);
                        }
                    }
                }
            }
            // -----------
            // 3. Strategy
            // -----------
            if (amountOfSuc > 4)
            {
                List<GameObject> selectedModules = new List<GameObject>();
                // Temporare List to save resources that can only be processed by few machines
                List<Resource> rareProcessingResource = getRareResourcesSuc(toHandleResources, increasedFactorSensitivity);

                // A strategy is selected based on the number of rare resources 
                if (rareProcessingResource.Count > (all_options.Count * rareStrategyFactor * increasedFactorSensitivity))
                {
                    selectedModules = sortReadyOptionsWithRaritySuc(rareProcessingResource, increasedFactorSensitivity, caller);
                }
                else
                {
                    // All possible options are sorted by processing time
                    selectedModules = sortReadyOptionsWithProcessingTimeSuc(caller.GetComponent<Module>().GetModuleInformation(), ready_options);
                }
                if (selectedModules.Count > 0)
                {
                    target = selectedModules[0];
                }
            }

            // -----------
            // 4. Strategy
            // -----------
            else if (amountOfSuc > 1)
            {
                List<GameObject> selectedModules = new List<GameObject>();
                // Temporare List to save resources that can only be processed by few machines
                List<Resource> rareProcessingResource = getRareResourcesSuc(toHandleResources, increasedFactorSensitivity);

                // A strategy is selected based on the number of rare resources 
                if (rareProcessingResource.Count > (all_options.Count * rareStrategyFactor))
                {
                    selectedModules = sortReadyOptionsWithRaritySuc(rareProcessingResource, 1f, caller);
                }
                else
                {
                    // All possible options are sorted by processing time
                    selectedModules = sortReadyOptionsWithProcessingTimeSuc(caller.GetComponent<Module>().GetModuleInformation(), ready_options);
                }
                if (selectedModules.Count > 0)
                {
                    target = selectedModules[0];
                }



                // A strategy is selected based on the number of rare resources 
                if (rareProcessingResource.Count > (all_options.Count * rareStrategyFactor))
                {
                    selectedModules = sortReadyOptionsWithRaritySuc(rareProcessingResource, increasedFactorSensitivity, caller);
                }
                else
                {
                    // All possible options are sorted by processing time
                    selectedModules = sortReadyOptionsWithProcessingTimeSuc(caller.GetComponent<Module>().GetModuleInformation(), ready_options);
                }
                if (selectedModules.Count > 0)
                {
                    target = selectedModules[0];
                }
            }
        }
        return target;
    }

    // -------------------
    // Predecessor methods  
    // -------------------
    private List<GameObject> sortReadyOptionsWithProcessingTimePre(Resource resource)
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

    private List<GameObject> sortReadyOptionsWithRarityPre(List<Resource> rareProcessingResource, float l_increasedFactorSensitivity, bool rareResource)
    {
        // Sort all machines descendendly by the amount of rare resources they can handle
        int amountOfHandledRareResourcesInMachine = 0;
        List<Tuple<GameObject, int>> moduleScores = new List<Tuple<GameObject, int>>();

        foreach (GameObject pre in ready_options)
        {
            foreach (Resource temp_resource in rareProcessingResource)
            {
                if (pre.GetComponent<Module>().GetAcceptedResources().Contains(temp_resource))
                {
                    amountOfHandledRareResourcesInMachine++;
                }
            }
            if (amountOfHandledRareResourcesInMachine < rareProcessingResource.Count * (1 - rareMaschineFactor * l_increasedFactorSensitivity) && rareResource)
            {
                moduleScores.Add(new Tuple<GameObject, int>(pre, amountOfHandledRareResourcesInMachine));
            }
        }
        return moduleScores.OrderByDescending(x => x.Item2).Select(x => x.Item1).ToList();
    }

    private List<Resource> getRareResourcesPre(Dictionary<Resource, int> toHandleResources, float l_increasedFactorSensitivity, Resource resource, ref bool rareResource)
    {
        // Fill rareProcessingResource and determine if the product/resource that has to be transported is rare
        List<Resource> rareProcessingResource = new List<Resource>();
        foreach (var temp_resource in toHandleResources)
        {
            if (temp_resource.Value < (all_options.Count * resourceRarityFactor * l_increasedFactorSensitivity))
            {
                rareProcessingResource.Add(temp_resource.Key);
                if (temp_resource.Key == resource)
                {
                    rareResource = true;
                }
            }
        }
        return rareProcessingResource;
    }

    // ----------------
    // Successor method
    // ----------------
    private List<Resource> getRareResourcesSuc(Dictionary<Resource, int> toHandleResources, float l_increasedFactorSensitivity)
    {
        // Fill rareProcessingResource and determine if the product/resource that has to be transported is rare
        List<Resource> rareProcessingResource = new List<Resource>();
        foreach (var temp_resource in toHandleResources)
        {
            if (temp_resource.Value < (all_options.Count * resourceRarityFactor * l_increasedFactorSensitivity))
            {
                rareProcessingResource.Add(temp_resource.Key);
            }
        }
        return rareProcessingResource;
    }
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

    private List<GameObject> sortReadyOptionsWithRaritySuc(List<Resource> rareProcessingResource, float l_increasedFactorSensitivity, GameObject caller)
    {
        // Sort all machines descendendly by rarity of the product
        // If the product is not rare the resource will not be considered
        int amountOfHandledRareResourcesInMachine = 0;
        List<GameObject> moduleScores = new List<GameObject>();

        foreach (GameObject pre in ready_options)
        {
            Resource outProduct = pre.GetComponent<Module>().GetModuleInformation().product;
            foreach (Resource temp_resource in rareProcessingResource)
            {
                if (outProduct == temp_resource)
                {
                    moduleScores.Add(pre);
                    break;
                }
            }
        }
        return sortReadyOptionsWithProcessingTimeSuc(caller.GetComponent<Module>().GetModuleInformation(), moduleScores);
    }
}