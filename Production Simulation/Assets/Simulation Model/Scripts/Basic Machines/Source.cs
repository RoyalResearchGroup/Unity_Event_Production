using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Source : Module
{
    public float rate;


    //Override the Gizmo color:
    // Visualize connections in editor mode
    void OnDrawGizmos()
    {
        if (connectedModules != null)
        {
            foreach (Module module in connectedModules)
            {
                if (module != null)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(transform.position, module.transform.position);
                }
            }
        }
    }
}


