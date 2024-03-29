using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBlueprint", menuName = "Resource Management/Blueprint")]
public class Blueprint : ScriptableObject
{
    public List<ResourceEntry> resources = new List<ResourceEntry>();
}
