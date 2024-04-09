using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Base class for heuristic strategies. But this implemented strategy is pretty stupid, so please don't use it.
//[CreateAssetMenu]
public interface Strategy : ScriptableObject
{
    // The list passed here is the action space. Maybe we should also provide the observation space to the strategies.
    public abstract GameObject act(GameObject caller, List<ModuleInformation> m_info);
}
