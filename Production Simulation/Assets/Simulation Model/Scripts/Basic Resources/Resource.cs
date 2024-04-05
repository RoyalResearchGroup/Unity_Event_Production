using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewResource", menuName = "Resource Management/Resource")]
public class Resource : ScriptableObject
{
    public string r_name;
    public Color r_color;
}
