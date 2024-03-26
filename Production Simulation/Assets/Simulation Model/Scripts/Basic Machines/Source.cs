using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Source : Module
{
    public float creationRate;


    //Override the Gizmo color:
    // Visualize connections in editor mode
    void OnDrawGizmos()
    {
        if (connectedObjects != null)
        {
            foreach (Module module in connectedObjects)
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


