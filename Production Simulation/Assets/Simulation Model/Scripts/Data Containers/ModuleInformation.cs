using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The module information Data container is a List of all relevant state information necessary to know for an agent.
/// </summary>

[System.Serializable]
public class ModuleInformation
{
    public TYPE type;
    public STATE state;
    public Resource product;
    public List<Resource> input;
    public Blueprint setup;

    public List<float> processingTimes;

    //SET BY AGENT
    public bool valid = false;
    public GameObject module;
    public bool ready;

    /// <summary>
    /// Needs Module type y, State s, Product resource type p, list of accepted input resources i, Setup Blueprint u, Process time list t.
    /// </summary>
    /// <param name="y"></param>
    /// <param name="s"></param>
    /// <param name="p"></param>
    /// <param name="i"></param>
    /// <param name="u"></param>
    /// <param name="t"></param>
    public ModuleInformation(TYPE y, STATE s, Resource p, List<Resource> i, Blueprint u, List<float> t)
    {
        type = y;
        state = s;
        product = p;
        input = i;
        setup = u;
        processingTimes = t;
    }
}
