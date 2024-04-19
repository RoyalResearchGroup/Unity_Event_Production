using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PanelController : MonoBehaviour
{
    //General Panel modules
    [SerializeField]
    private TextMeshProUGUI nameText;

    //Connected module
    protected SimulationObject simObject;

    protected TimeManager t_manager;

    private void Start()
    {
        t_manager = GameObject.FindWithTag("EventManager").GetComponent<TimeManager>();
    }

    //Init the panel.
    public void Init(string name, SimulationObject obj)
    {
        nameText.text = name;
        simObject = obj;
    }

    //Destroy command (button)
    public void DestroyPanel()
    {
        Destroy(gameObject);
    }
}
