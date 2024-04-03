using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [SerializeField]
    public float time {  get; set; }

    public void ProgressTime(float dt)
    {
        time += dt;
    }

}
