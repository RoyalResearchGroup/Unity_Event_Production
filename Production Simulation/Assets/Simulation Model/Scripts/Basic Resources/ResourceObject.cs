using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Helper to wrap the resource
/// </summary>
[System.Serializable]
public class ResourceObject
{
    public Resource Resource;

    public ResourceObject(Resource resource)
    {
        Resource = resource;
    }
}
