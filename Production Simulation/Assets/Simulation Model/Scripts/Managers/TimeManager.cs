using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [SerializeField]
    public float time {  get; set; }
    public float deltaTime = 0;

    public void ProgressTime(float dt)
    {
        time += dt;
        deltaTime = dt;
    }

}
