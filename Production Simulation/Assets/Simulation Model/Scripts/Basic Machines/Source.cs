using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEditor;

enum Distribution
{
    Linear,
    Normal,
    Exponential
}

public class Source : Module
{
    [HideInInspector] public float[] parameters = new float[3] { 0.0f, 0.0f, 0.0f };
    private float creationTime;
    public Resource creationType;
    [SerializeField] private Distribution distribution = Distribution.Linear;

    public override void DetermineState()
    {
        //The states here are simple.
        if (d_event)
        {
            SetSTATE(STATE.OCCUPIED);
        }
        else
        {
            if (resourceBuffer.Count > 0)
            {

                if (resourceBuffer.Count < resourceBuffer.Limit)
                    SetSTATE(STATE.AVAILABLE);
                else
                    SetSTATE(STATE.BLOCKED);
            }
            else
            {
                SetSTATE(STATE.EMPTY);
            }
        }
    }

    public override void DispatchEvent()
    {
        base.DispatchEvent();

        e_manager.EnqueueEvent(new Event(DistributedCreationTime(), this, EVENTTYPE.CREATE));
    }

    public override void EventCallback(Event r_event)
    {
        base.EventCallback(r_event);

        //Create the resource object
        ResourceObject obj = new ResourceObject(creationType);
        AddResource(obj);
    }

    private float DistributedCreationTime()
    {
        switch (distribution)
        {
            case Distribution.Linear:
                float range = parameters[2] - parameters[1];
                creationTime = RandomFromDistribution.RandomLinear(parameters[0]) * range + parameters[1];
                break;
            case Distribution.Normal:
                creationTime = RandomFromDistribution.RandomNormalDistribution(parameters[0], parameters[1]);
                break;
            case Distribution.Exponential:
                creationTime = RandomFromDistribution.RandomFromExponentialDistribution(parameters[0],
                    RandomFromDistribution.Direction_e.Right);
                break;
            default:
                creationTime = float.PositiveInfinity;
                break;
        }

        return creationTime;
    }



    public override void Start()
    {
        base.Start();
        //init resource buffer with one slot
        resourceBuffer = new LimitedQueue<ResourceObject>(1);
        DispatchEvent();
        DetermineState();
    }

    public void Update()
    {
        //If the buffer is not full
        if (resourceBuffer.Count < resourceBuffer.Limit && GetSTATE() != STATE.OCCUPIED)
        {
            //Dispatch the Event to spawn a resource
            DispatchEvent();
            DetermineState();
        }
    }

    public override void NotifyEventBatch()
    {
        base.NotifyEventBatch();
    }




    public override void MoveToModule(Module module)
    {
        ResourceObject res = resourceBuffer.Dequeue();
        module.AddResource(res);
        //Update the states on both this and the target module
        DetermineState();
        module.DetermineState();
    }

    public override void UpdateCTRL(Module m)
    {
        //As long as we can find output objects and there are resources present, distribute them
        while (resourceBuffer.Count > 0)
        {
            //The source can only output resources, so no input model needed
            Module mod_out;

            //The current resource type in the buffer
            Resource res_peek = resourceBuffer.Peek().Resource;

            //Get a candidate for output
            mod_out = (Module)OutputCTRL(res_peek);

            if (m != null)
            {
                mod_out = m;
            }

            //There are no candidates, so break the loop and return.
            if (mod_out == null)
            {
                //Determine state before leaving (likely blocked)
                DetermineState();
                break;
            }

            //Otherwise, we can move the resource
            MoveToModule(mod_out);
            mod_out.UpdateCTRL();

            m = null;
        }
        //Dispatch a new Event creation
        if (resourceBuffer.Count < resourceBuffer.Limit)
        {
            DispatchEvent();
            DetermineState();
        }
    }


    //Override the Gizmo color:
    // Visualize connections in editor mode
    void OnDrawGizmos()
    {
        if (connectedObjects != null)
        {
            foreach (Module module in connectedObjects)
            {
                if (module != null)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(transform.position, module.transform.position);
                }
            }
        }
    }

    public override bool IsInputReady(Resource r)
    {
        //Never takes anything in
        return false;
    }

    public override bool IsOutputReady(List<Resource> r)
    {
        //Output readiness is based on the current state. If there are items in the buffer, they can be drawn
        if (resourceBuffer.Count > 0 && r.Contains(resourceBuffer.Peek().Resource))
        {
            return true;
        }
        return false;
    }

    public override ModuleInformation GetModuleInformation()
    {
        List<float> tList = new List<float>{ creationTime };
        return new ModuleInformation(TYPE.SOURCE, GetSTATE(), creationType, null, null, tList, null, resourceBuffer);
    }

    public override List<Resource> GetAcceptedResources()
    {
        return null;
    }

    public override Resource GetOutputResource()
    {
        return creationType;
    }

    public override void ResetModule()
    {
        base.ResetModule();
        resourceBuffer.Clear();
        DetermineState();
    }

    public override bool ResourceSetupBlueprint(Resource resource)
    {
        return true;
    }

    public override Resource GetProduct()
    {
        return resourceBuffer.Peek().Resource;
    }

    [CustomEditor(typeof(Source))]
    [Serializable]
    public class MyScriptEditor : Editor
    {
        private SerializedProperty parametersProp;

        private void OnEnable()
        {
            parametersProp = serializedObject.FindProperty("parameters");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            base.OnInspectorGUI();

            // Reference the variables in the script
            Source script = (Source)target;

            // Ensure the label and the value are on the same line
            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginChangeCheck();

            switch (script.distribution)
            {
                case Distribution.Linear:
                    EditorGUILayout.PropertyField(parametersProp.GetArrayElementAtIndex(0), new GUIContent("Slope"));
                    EditorGUILayout.EndHorizontal(); EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(parametersProp.GetArrayElementAtIndex(1), new GUIContent("Min"));
                    EditorGUILayout.EndHorizontal(); EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(parametersProp.GetArrayElementAtIndex(2), new GUIContent("Max"));
                    break;
                case Distribution.Normal:
                    EditorGUILayout.PropertyField(parametersProp.GetArrayElementAtIndex(0), new GUIContent("Mean"));
                    EditorGUILayout.EndHorizontal(); EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(parametersProp.GetArrayElementAtIndex(1), new GUIContent("Standard Deviation"));
                    break;
                case Distribution.Exponential:
                    EditorGUILayout.PropertyField(parametersProp.GetArrayElementAtIndex(0), new GUIContent("Exponent"));
                    break;
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}