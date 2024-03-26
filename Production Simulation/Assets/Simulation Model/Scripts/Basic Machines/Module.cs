using System.Collections.Generic;
using UnityEngine;

public class Module : MonoBehaviour
{
    // A list of modules this one is connected to
    [SerializeField]
    public List<Module> connectedModules;

    // Should the goods be forwarded automatically or wait for a controller
    public bool shouldForwardAutomatically;

    // Visualize connections in editor mode
    void OnDrawGizmos()
    {
        if (connectedModules != null)
        {
            foreach (Module module in connectedModules)
            {
                if (module != null)
                {
                    Gizmos.color = Color.black;
                    Gizmos.DrawLine(transform.position, module.transform.position);
                }
            }
        }
    }
}
