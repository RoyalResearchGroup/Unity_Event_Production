using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentManager : MonoBehaviour
{
    public void EndEpisode()
    {
        BroadcastMessage("NotifyEndEpisode");
    }

}
