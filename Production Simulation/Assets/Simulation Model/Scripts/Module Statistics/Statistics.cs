using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Top level class to manage statistics. Provides basic headers and variables.
/// </summary>
public abstract class Statistics : MonoBehaviour
{
    //Time manager
    protected TimeManager t_manager;


    public virtual void Start()
    {
        t_manager = GameObject.FindWithTag("TimeManager").GetComponent<TimeManager>();
    }

}
