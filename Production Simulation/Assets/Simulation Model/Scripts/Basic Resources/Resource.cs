using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewResource", menuName = "Resource Management/Resource")]
public class Resource : ScriptableObject
{
    //Simple for now, might contain things like expected time later
    public string r_name;
    public Color r_color;
}
