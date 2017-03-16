/* Copyright Kupio Limited SC426881. All rights reserved. Source not for distribution. */

namespace com.kupio.FlowControlEditor
{
    using FlowControl;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(FlowControlTestingPoint))]
    public class FlowControlTestingPointEditor : Editor
    {
        private SerializedProperty step;
        private SerializedProperty iterations;
        private SerializedProperty inertia;

        void OnEnable()
        {
            step = serializedObject.FindProperty("step");
            iterations = serializedObject.FindProperty("iterations");
            inertia = serializedObject.FindProperty("inertia");
        }

        public override void OnInspectorGUI()
        {
            iterations.intValue = EditorGUILayout.IntField("Iterations:", iterations.intValue);
            step.floatValue = EditorGUILayout.FloatField("Step:", step.floatValue);
            inertia.floatValue = EditorGUILayout.FloatField("Inertia:", inertia.floatValue);

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }

        }
    }
}
