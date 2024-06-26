using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Very simple, manages the simulation time
/// </summary>
public class TimeManager : MonoBehaviour
{
    public float time {  get; set; }
    public float deltaTime = 0;

    public void ProgressTime(float dt)
    {
        time += dt;
        deltaTime = dt;
    }

    public void ResetModule()
    {
        time = 0;
        deltaTime = 0;
    }
}
