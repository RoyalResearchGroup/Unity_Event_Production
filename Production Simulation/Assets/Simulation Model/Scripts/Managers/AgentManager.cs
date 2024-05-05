using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// global episode termination
/// </summary>
public class AgentManager : MonoBehaviour
{
    public void EndEpisode()
    {
        BroadcastMessage("NotifyEndEpisode");
    }

}
