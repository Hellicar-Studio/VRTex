/* Copyright Kupio Limited SC426881. All rights reserved. Source not for distribution. */

namespace com.kupio.FlowControl
{
    using System;
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [ExecuteInEditMode]
    [Serializable]
    public class SplinePoint : MonoBehaviour
    {
#if UNITY_EDITOR
        private Vector3 oldPos;
        private Quaternion oldRot;
#endif


        public Vector3 Position
        {
            get
            {
                return transform.position;
            }
            set
            {
                if (transform.position != value)
                {
                    transform.position = value;
                    SendMessageToParent("OnPointChanged");
                }
            }
        }

        [SerializeField]
        private Vector3 m_handleApproach;
        public Vector3 HandleApproach
        {
            get
            {
                return m_handleApproach;
            }
            set
            {
                if (m_handleApproach != value)
                {
                    m_handleApproach = value;
                    HandleDepart = new Vector3(-m_handleApproach.x, -m_handleApproach.y, -m_handleApproach.z);
                    SendMessageToParent("OnPointChanged");
                }
            }
        }

        [SerializeField]
        private Vector3 m_handleDepart;
        public Vector3 HandleDepart
        {
            get
            {
                return m_handleDepart;
            }
            set
            {
                if(m_handleDepart != value)
                {
                    m_handleDepart = value;
                    HandleApproach = new Vector3(-m_handleDepart.x, -m_handleDepart.y, -m_handleDepart.z);
                    SendMessageToParent("OnPointChanged");
                }
            }
        }

        void Awake()
        {
#if UNITY_EDITOR
#endif
        }

        void OnValidate()
        {
            SendMessageToParent("OnPointChanged");
        }

#if UNITY_EDITOR

        public bool ShowSplinePoints
        {
            get
            {
                Spline s = GetComponentInParent<Spline>();
                return (s == null || s.ShowSplines);
            }
        }

        void OnDrawGizmos()
        {
            if (oldPos != transform.position || oldRot != transform.rotation)
            {
                SendMessageToParent("OnPointChanged");
                oldPos = transform.position;
                oldRot = transform.rotation;
            }

            if (!ShowSplinePoints || Selection.activeGameObject == gameObject)
            {
                return;
            }

            Handles.color = Color.yellow;

            Transform t = gameObject.transform;

            Handles.DrawDottedLine(t.TransformPoint(HandleDepart), t.TransformPoint(HandleApproach), 3f);


        }
#endif
        private void SendMessageToParent(string msg)
        {
            Transform pt = this.gameObject.transform.parent;
            if (pt == null)
            {
                return;
            }
            pt.gameObject.SendMessage(msg, SendMessageOptions.DontRequireReceiver);
        }

    }
}
