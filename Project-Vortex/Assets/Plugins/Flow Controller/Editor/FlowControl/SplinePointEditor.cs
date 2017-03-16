/* Copyright Kupio Limited SC426881. All rights reserved. Source not for distribution. */

namespace com.kupio.FlowControlEditor
{
    using FlowControl;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(SplinePoint))]
    public class SplinePointEditor : Editor
    {
        private SplinePoint m_splinePoint;

        void OnEnable()
        {
            m_splinePoint = (SplinePoint)target;
        }

        public override void OnInspectorGUI()
        {
            //GUILayout.Label("Point mode: smooth"); /* TODO: Other modes. */
            m_splinePoint.HandleDepart = EditorGUILayout.Vector3Field("Depart from", m_splinePoint.HandleDepart);
            m_splinePoint.HandleApproach = EditorGUILayout.Vector3Field("Approach from", m_splinePoint.HandleApproach);
        }

        void OnSceneGUI()
        {
            Handles.color = Color.yellow;

            Transform t = m_splinePoint.gameObject.transform;

            m_splinePoint.HandleDepart = t.InverseTransformPoint(
                    Handles.FreeMoveHandle(
                            t.TransformPoint(
                                m_splinePoint.HandleDepart),
                            Quaternion.identity,
                            HandleUtility.GetHandleSize(m_splinePoint.HandleDepart) * 0.15f,
                            Vector3.zero,
                            Handles.CircleCap));

            m_splinePoint.HandleApproach = t.InverseTransformPoint(
                    Handles.FreeMoveHandle(
                            t.TransformPoint(
                                m_splinePoint.HandleApproach),
                            Quaternion.identity,
                            HandleUtility.GetHandleSize(m_splinePoint.HandleApproach) * 0.15f,
                            Vector3.zero,
                            Handles.CircleCap));

            Handles.DrawLine(t.TransformPoint(m_splinePoint.HandleDepart), t.TransformPoint(m_splinePoint.HandleApproach));

        }
    }
}
