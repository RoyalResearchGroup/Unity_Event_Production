using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBlueprint", menuName = "Resource Management/Blueprint")]
public class Blueprint : ScriptableObject
{
    public List<ResourceEntry> resources = new List<ResourceEntry>();
    public List<ResourceEntry> products = new List<ResourceEntry>();
    public float processingTime;
    public float setupTime;
}
