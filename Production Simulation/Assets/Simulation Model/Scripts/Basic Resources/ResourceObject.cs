using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResourceObject
{
    public Resource Resource;

    public ResourceObject(Resource resource)
    {
        Resource = resource;
    }
}
