/* Copyright Kupio Limited SC426881. All rights reserved. Source not for distribution. */

namespace com.kupio.FlowControlEditor
{
    using UnityEngine;
    using UnityEditor;
    using System;
    using FlowControl;

    [CustomEditor(typeof(FlowControlRegion))]
    public class FlowControlRegionEditor : Editor
    {
        private FlowControlRegion m_field;
        private SerializedProperty resolution;
        private SerializedProperty showForces;
        private SerializedProperty showSplines;
        private SerializedProperty showTestPoints;
        private SerializedProperty forceDisplayCutoff;
        private SerializedProperty approach;
        private SerializedProperty outerVector;

        void OnEnable()
        {
            m_field = (FlowControlRegion)target;

            showForces = serializedObject.FindProperty("ShowForces");
            showSplines = serializedObject.FindProperty("ShowSplines");
            showTestPoints = serializedObject.FindProperty("ShowTestPoints");
            forceDisplayCutoff = serializedObject.FindProperty("forceDisplayCutoff");
            outerVector = serializedObject.FindProperty("outerVector");
            resolution = serializedObject.FindProperty("m_resolution");
            approach = serializedObject.FindProperty("m_approach");
        }

        [MenuItem("GameObject/Create Other/Flow control region")]
        public static void CreateRegion(MenuCommand command)
        {
            GameObject region = new GameObject("Flow control region");
            region.AddComponent<FlowControlRegion>();
        }

        public override void OnInspectorGUI()
        {
            resolution.intValue = EditorGUILayout.IntField("Resolution:", resolution.intValue);
            approach.floatValue = EditorGUILayout.FloatField("Approach:", approach.floatValue);

            EditorGUILayout.Separator();

            int selected = (int)m_field.outerBehaviour;
            string[] options = new string[]
            {
                FlowControlRegion.OuterBehaviour.MoveToCenter.ToString(),
                FlowControlRegion.OuterBehaviour.AwayFromCenter.ToString(),
                FlowControlRegion.OuterBehaviour.ApplyVector.ToString()
            };

            selected = EditorGUILayout.Popup("Outer behaviour", selected, options);
            m_field.outerBehaviour = (FlowControlRegion.OuterBehaviour)Enum.Parse(typeof(FlowControlRegion.OuterBehaviour), options[selected]);

            /* TODO: Unity has a fit if I uncomment this. I do not yet understand why. */
            //if (m_field.outerBehaviour == FlowControlRegion.OuterBehaviour.ApplyVector)
            {
                outerVector.vector3Value = EditorGUILayout.Vector3Field("Outer vector:", m_field.outerVector);
            }

            EditorGUILayout.Separator();

            if (GUILayout.Button("Add Spline"))
            {
                m_field.AddSpline();
            }

            EditorGUILayout.Separator();

            showForces.boolValue = EditorGUILayout.Toggle("Show forces", showForces.boolValue);
            if (showForces.boolValue)
            {
                forceDisplayCutoff.floatValue = EditorGUILayout.Slider("Force display cutoff", forceDisplayCutoff.floatValue, 0f, 0.9f);
            }
            showSplines.boolValue = EditorGUILayout.Toggle("Show splines", showSplines.boolValue);
            showTestPoints.boolValue = EditorGUILayout.Toggle("Show test points", showTestPoints.boolValue);

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }

        }

    }
}
