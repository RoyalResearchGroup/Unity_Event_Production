using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "NewBlueprint", menuName = "Resource Management/Blueprint")]
public class Blueprint : ScriptableObject
{
    
    [HideInInspector] public float[] parameters = new float[3] { 0.0f, 0.0f, 0.0f };
    public List<ResourceEntry> resources = new List<ResourceEntry>();
    public Resource product;
    public float processingTime;
    public float setupTime;
    [SerializeField] private Distribution distribution = Distribution.Linear;

    
    public float DistributedProcessingTime()
    {
        switch (distribution)
        {
            case Distribution.Linear:
                float range = parameters[2] - parameters[1];
                processingTime = RandomFromDistribution.RandomLinear(parameters[0]) * range + parameters[1];
                break;
            case Distribution.Normal:
                processingTime = RandomFromDistribution.RandomNormalDistribution(parameters[0], parameters[1]);
                break;
            case Distribution.Exponential:
                processingTime = RandomFromDistribution.RandomFromExponentialDistribution(parameters[0],
                    RandomFromDistribution.Direction_e.Right);
                break;
            default:
                processingTime = float.PositiveInfinity;
                break;
        }

        return processingTime;
    }
    
    [CustomEditor(typeof(Blueprint))]
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
            Blueprint script = (Blueprint)target;

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
