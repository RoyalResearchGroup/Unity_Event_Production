using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The module information Data container is a List of all relevant state information necessary to know for an agent.
/// </summary>

[System.Serializable]
public class ModuleInformation
{
    
    // SET IN BASE AGENT GetObservationInformation()
    public TYPE type;
    public STATE state;
    public Resource product;
    public List<Resource> input;
    public Blueprint setup;
    public List<float> processingTimes;
    
    
    // OBSERVED ATTRIBUTES COUNT
    public int observedAttributes; 
    // BOOLEAN DICTIONARY
    public Dictionary<string, bool> attributeBooleans;
    
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
        
        // Initialize the dictionary
        attributeBooleans = new Dictionary<string, bool>();

        // Populate the dictionary with boolean values for each attribute
        PopulateAttributeBooleans();
    }
    
    private void PopulateAttributeBooleans()
    {
        var fields = this.GetType().GetFields(); // Get all public fields of the class
        
        foreach (var field in fields)
        {   
            // Get the value of the field
            var value = field.GetValue(this);
            // Check if the field is not a dictionary
            if (field.FieldType != typeof(Dictionary<string, bool>) && value != null)
            {
                // Add the field name and set its corresponding boolean value to false
                attributeBooleans[field.Name] = false;
            }
        }
    }

    public void setAttrBoolean(string attributeName)
    {
        attributeBooleans[attributeName] = true;
    }
}
