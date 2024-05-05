using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Not part of the manager layer, goes on the canvas
/// </summary>
public class UIManager : MonoBehaviour
{
    public GameObject canvas;

    //Check If the user clicked on a module. If so, open this module's panel
    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray.origin, ray.direction, out hit, 10000.0f))
            {
                if (canvas != null && hit.collider.gameObject.GetComponent<SimulationObject>() && hit.collider.gameObject.GetComponent<SimulationObject>().UIPanel && !EventSystem.current.IsPointerOverGameObject())
                {
                    var panel = Instantiate(hit.collider.gameObject.GetComponent<SimulationObject>().UIPanel, canvas.transform);
                    panel.GetComponent<PanelController>().Init(hit.collider.gameObject.name, hit.collider.gameObject.GetComponent<SimulationObject>());
                }
            }
        }
    }
}
