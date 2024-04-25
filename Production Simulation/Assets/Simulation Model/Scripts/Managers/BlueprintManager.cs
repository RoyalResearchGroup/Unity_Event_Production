using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlueprintManager : MonoBehaviour
{

    //The blueprint this machine is producing
    [SerializeField]
    public List<Blueprint> blueprints = new List<Blueprint>();

    //A list of resources that this machine takes in and its capacity (set by blueprint)
    private List<float> resourceProcessingTimes = new List<float>();
    private List<float> resourceSetupTimes = new List<float>();
    private List<int> capacities = new List<int>();

    //Current accepting states
    private List<Resource> allowedResources = new List<Resource>();
    private int currentCapacity;
    private List<Blueprint> currentBlueprints = new List<Blueprint>();

    public List<Resource> GetAllowedResources()
    {
        return allowedResources;
    }

    public int GetCurrentCapacity()
    {
        return currentCapacity;
    }

    public List<float> GetProcessingTimes()
    {
        return resourceProcessingTimes;
    }


    public void InitializeBlueprintSettings()
    {
        int maxCapacity = 0; // Variable to keep track of the largest capacity

        foreach (Blueprint blueprint in blueprints)
        {
            resourceProcessingTimes.Add(blueprint.processingTime);
            resourceSetupTimes.Add(blueprint.setupTime);

            int blueprintCapacity = 0;

            foreach (ResourceEntry resourceEntry in blueprint.resources)
            {
                blueprintCapacity += resourceEntry.number;
            }

            capacities.Add(blueprintCapacity);

            // Update maxCapacity if this blueprint's capacity is greater than the current max
            if (blueprintCapacity > maxCapacity)
            {
                maxCapacity = blueprintCapacity;
            }
        }

        // Set the currentCapacity to the largest value found
        currentCapacity = maxCapacity;
    }


    public void UpdateAllowedResourcesAndBlueprints(LimitedQueue<ResourceObject> currentInputBuffer)
    {
        // Aggregate resources from the current input buffer for comparison.
        var aggregatedResources = currentInputBuffer
            .GroupBy(ro => ro.Resource)
            .ToDictionary(group => group.Key, group => group.Count());

        // Determine blueprints that are viable based on current resources.
        List<Blueprint> newViableBlueprints = blueprints.Where(bp =>
            aggregatedResources.All(kv => bp.resources.Any(re => re.resource == kv.Key))
        ).ToList();

        // Identify resources that are not needed: not required by any blueprint or fully satisfy all viable blueprints.
        HashSet<Resource> unneededResources = new HashSet<Resource>();
        foreach (var resource in aggregatedResources.Keys)
        {
            bool isNeededByAnyBlueprint = newViableBlueprints.Any(bp => bp.resources.Any(re => re.resource == resource));
            bool fulfillsAllViableBlueprints = newViableBlueprints.All(bp =>
                bp.resources.Any(re => re.resource == resource && re.number <= aggregatedResources[resource]));

            if (!isNeededByAnyBlueprint || fulfillsAllViableBlueprints)
            {
                unneededResources.Add(resource);
            }
        }

        // Allowed resources are those not marked as unneeded.
        allowedResources = blueprints.SelectMany(bp => bp.resources)
                                     .Select(re => re.resource)
                                     .Distinct()
                                     .Except(unneededResources)
                                     .ToList();

        currentBlueprints = newViableBlueprints; // Update the list of viable blueprints.

        // Recalculate capacities based on the new set of viable blueprints.
        RecalculateCapacities(newViableBlueprints);
    }

    private void RecalculateCapacities(List<Blueprint> viableBlueprints)
    {
        capacities.Clear();
        int maxCapacity = 0;
        foreach (var blueprint in viableBlueprints)
        {
            int blueprintCapacity = blueprint.resources.Sum(r => r.number);
            capacities.Add(blueprintCapacity);
            if (blueprintCapacity > maxCapacity)
            {
                maxCapacity = blueprintCapacity;
            }
        }
        currentCapacity = maxCapacity;
    }


    public Blueprint FindFirstBlueprintMeetingRequirements(LimitedQueue<ResourceObject> currentInputBuffer)
    {
        // Aggregate resources from the current input buffer for comparison.
        var aggregatedResources = currentInputBuffer
            .GroupBy(ro => ro.Resource)
            .ToDictionary(group => group.Key, group => group.Count());

        // Iterate over all blueprints to find the first one that fully meets the requirements.
        foreach (var blueprint in blueprints)
        {
            bool allRequirementsMet = true;

            foreach (var requiredResource in blueprint.resources)
            {
                // Check if the required resource is present in the input buffer in sufficient quantity.
                if (!aggregatedResources.TryGetValue(requiredResource.resource, out int availableQuantity) ||
                    availableQuantity < requiredResource.number)
                {
                    allRequirementsMet = false;
                    break; // Break out of the inner loop as soon as a requirement is not met.
                }
            }

            if (allRequirementsMet)
            {
                return blueprint; // Return the first blueprint that fully meets the requirements.
            }
        }

        return null; // Return null if no blueprint fully meets the requirements.
    }



    /// <summary>
    /// Checks if adding a single resource to existing resources can help in completing the blueprint in the future.
    /// </summary>
    /// <param name="blueprint">The blueprint to check against.</param>
    /// <param name="currentResourceObjects">List of current resource objects already counted towards the blueprint.</param>
    /// <param name="newResource">The new resource to add.</param>
    /// <returns>True if adding the new resource does not exceed the blueprint's requirements and is required by the blueprint; otherwise, false.</returns>
    public bool CanAddResourceToCompleteBlueprint(Blueprint blueprint, LimitedQueue<ResourceObject> currentResourceObjects, Resource newResource)
    {
        // Ensure that blueprint, blueprint's resources, and newResource are not null
        if (blueprint == null || blueprint.resources == null || newResource == null)
        {
            //Debug.LogWarning("Blueprint and resources must not be null.");
            return true;  // Safe fail, inputs are not valid
        }

        // Check if the new resource is part of the blueprint requirements.
        if (!blueprint.resources.Any(r => r.resource == newResource))
        {
            return false;  // Return false immediately if the new resource is not required by the blueprint.
        }

        // Create a dictionary from the current resources objects to count each resource.
        var resourceCounts = currentResourceObjects?.GroupBy(ro => ro.Resource)
                                                   .ToDictionary(g => g.Key, g => g.Count()) ?? new Dictionary<Resource, int>();

        // Add the new resource to the dictionary.
        if (resourceCounts.ContainsKey(newResource))
        {
            resourceCounts[newResource]++;
        }
        else
        {
            resourceCounts[newResource] = 1;
        }

        // Check if the count of any resource exceeds its requirement in the blueprint.
        foreach (var requirement in blueprint.resources)
        {
            if (resourceCounts.TryGetValue(requirement.resource, out int count))
            {
                // If the count of any resource exceeds its requirement, return false.
                if (count > requirement.number)
                {
                    return false;
                }
            }
        }

        // If no resources exceed their required amount, return true because it's possible to complete the blueprint in the future.
        return true;
    }
}
