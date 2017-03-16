/* Copyright Kupio Limited SC426881. All rights reserved. Source not for distribution. */

namespace com.kupio.FlowControl
{
    using System;
    using UnityEngine;
    using UtilityBelt;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [ExecuteInEditMode]
    [Serializable]
    public class Spline : MonoBehaviour
    {
        private int m_resolution = 20; /* Not currently editable */

        private bool regen = true; /* Something has changed. Regenerate everything. */

        [SerializeField]
        private bool m_isClosed;
        public bool IsClosed
        {
            get
            {
                return m_isClosed;
            }
            set
            {
                if (m_isClosed != value)
                {
                    m_isClosed = value;
                    SendMessageToParent("OnSplineChanged");
                }
            }
        }

        [SerializeField]
        private SplinePoint[] m_points;

        public SplinePoint[] Points
        {
            get
            {
                return m_points;
            }
        }

        private void Regenerate()
        {
#if UNITY_EDITOR
            regen = false;
            if (Application.isPlaying) return; //or whatever script needs to be run when in edition mode.
            SplinePoint[] points = gameObject.GetComponentsInChildren<SplinePoint>();

            if (points.Length < 2)
            {
                AddPoint(new Vector3(0f, 0f, 0f), new Vector3(0.2f, 0f, 0f));
                AddPoint(new Vector3(1f, 1f, 0f), new Vector3(1f, 1.2f, 0f));
            }

            SendMessageToParent("OnSplineChanged");

            m_points = points;
#endif
        }

        public void AddPoint(Vector3? pos = null, Vector3? departTowards = null)
        {
            if(!pos.HasValue || !departTowards.HasValue)
            {
                pos = new Vector3(1, 1, 1);
                departTowards = new Vector3(2, 2, 2);
                /* TODO: Take this from the last point, extended. */
            }

            GameObject go = new GameObject();
            go.transform.position = pos.Value;
            go.name = GameObjectHelper.GenerateNextName(typeof(SplinePoint), "Point", gameObject);
            SplinePoint sp = go.AddComponent<SplinePoint>();
            sp.HandleDepart = departTowards.Value;
            go.transform.parent = transform;

            regen = true;
        }

        private void OnPointChanged()
        {
            regen = true;
        }

        void OnValidate()
        {
            regen = true;
        }

        void Awake()
        {
            regen = true;
        }

        /// <summary>
        /// Find a point between two spline points in world-space
        /// </summary>
        /// <param name="t">0..1</param>
        public Vector3 GetPointOnCurve(SplinePoint from, SplinePoint to, float t)
        {
            t = Mathf.Clamp01(t);

            Vector3 b = from.transform.TransformPoint(from.HandleDepart);
            Vector3 c = to.transform.TransformPoint(to.HandleApproach);

            return Vector3.Lerp(
                Vector3.Lerp(
                    Vector3.Lerp(from.transform.position, b, t),
                    Vector3.Lerp(b, c, t),
                    t),
                Vector3.Lerp(
                    Vector3.Lerp(b, c, t),
                    Vector3.Lerp(c, to.transform.position, t),
                    t),
                t
                );
        }

        /// <summary>
        /// Call a function along points on the curve in world-splace
        /// </summary>
        public void CrawlSpline(Action<Vector3, Vector3> cb, int resolution)
        {
            SplinePoint from, to;
            Vector3 lastPoint;
            int max = resolution + 1;
            if(m_points == null)
            {
                Regenerate();
                SendMessageToParent("OnSplineChanged");
            }

            for (int i = 0; i < m_points.Length - 1; i++)
            {
                from = m_points[i];
                to = m_points[i + 1];
                if (from == null || to == null)
                {
                    Regenerate();
                    SendMessageToParent("OnSplineChanged");
                    return;
                }

                lastPoint = from.transform.position;

                for (int j = 1; j < max; j++)
                {
                    Vector3 currentPoint = GetPointOnCurve(from, to, j / (float)resolution);
                    cb(lastPoint, currentPoint);
                    lastPoint = currentPoint;
                }
            }

            if(m_isClosed)
            {
                from = m_points[m_points.Length - 1];
                to = m_points[0];
                if (from == null || to == null)
                {
                    Regenerate();
                    SendMessageToParent("OnSplineChanged");
                    return;
                }

                lastPoint = from.transform.position;

                for (int j = 1; j < max; j++)
                {
                    Vector3 currentPoint = GetPointOnCurve(from, to, j / (float)resolution);
                    cb(lastPoint, currentPoint);
                    lastPoint = currentPoint;
                }
            }
        }

        public bool ShowSplines
        {
            get
            {
                FlowControlRegion fcr = GetComponentInParent<FlowControlRegion>();
                return (fcr == null || fcr.ShowSplines);
            }
        }

        private void SendMessageToParent(string msg)
        {
            Transform pt = this.gameObject.transform.parent;
            if (pt == null)
            {
                return;
            }
            pt.gameObject.SendMessage(msg, SendMessageOptions.DontRequireReceiver);
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (regen)
            {
                Regenerate();
            }

            if(ShowSplines)
            {
                Gizmos.color = Selection.activeGameObject == gameObject ? Color.yellow : Color.magenta;
                CrawlSpline((a, b) => {
                    Gizmos.DrawLine(a, b);
                }, m_resolution);
            }
        }
#endif
    }
}
