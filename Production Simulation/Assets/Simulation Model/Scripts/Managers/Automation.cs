using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Automation : MonoBehaviour
{
    private List<GameObject> experimentInstances;
    
    private void Awake()
    {
        experimentInstances = GameObject.FindGameObjectsWithTag("EventManager").ToList();
        for (int i = 0; i < experimentInstances.Count; i++)
        {
            experimentInstances[i].layer = i+1;
        }
    }
}