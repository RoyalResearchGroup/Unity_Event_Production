using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExperimentManager : MonoBehaviour
{
    public void CheckState()
    {
        //NYI
    }

    public void ResetScene()
    {
        BroadcastMessage("CallbackIllegalAction");
        BroadcastMessage("ResetModule");
    }
}
