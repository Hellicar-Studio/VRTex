/* Copyright Kupio Limited SC426881. All rights reserved. Source not for distribution. */

namespace com.kupio.FlowControl
{
#if UNITY_EDITOR
    using UnityEditor;
#endif

    using UnityEngine;
    using System;
    using System.Threading;
    using System.Collections.Generic;
    using uTween;
    using UtilityBelt;

    /// <summary>
    /// The Flow control region marks an AABB within which child spline curves can incluence
    /// a flow grid.
    /// </summary>
    [ExecuteInEditMode]
    [Serializable]
    public class FlowControlRegion : MonoBehaviour
    {
        private class WorkGroup
        {
            [NonSerialized]
            public ManualResetEvent doneEvent;
            private int offsetX;
            private int span;
            private double maxMag2;
            private int m_resolution;
            private FieldCell[,,] field;
            private List<FieldCell> hasSplines;
            private float m_approach;
            private float m_xmin, m_ymin, m_zmin;
            private Vector3 tscale;

            public WorkGroup(int offsetX, int span, FieldCell[,,] field, int res, List<FieldCell> hasSplines, float approach, float xmin, float ymin, float zmin, Vector3 tscale)
            {
                doneEvent = new ManualResetEvent(false);
                this.offsetX = offsetX;
                this.field = field;
                this.span = span;
                this.maxMag2 = (tscale.x * tscale.x * 4) + (tscale.y * tscale.y * 4) + (tscale.z * tscale.z * 4);
                this.hasSplines = hasSplines;
                this.m_resolution = res;
                this.m_approach = approach;

                this.m_xmin = xmin;
                this.m_ymin = ymin;
                this.m_zmin = zmin;

                this.tscale = tscale;
            }

            public void DistanceCallback(System.Object o)
            {

                int x, y, z;

                float xmin = m_xmin + (tscale.x / m_resolution);
                float ymin = m_ymin + (tscale.y / m_resolution);
                float zmin = m_zmin + (tscale.z / m_resolution);

                float xmul = (tscale.x * 2 / m_resolution);
                float ymul = (tscale.y * 2 / m_resolution);
                float zmul = (tscale.z * 2 / m_resolution);

                for (x = offsetX; x < m_resolution; x += span)
                {
                    for (y = 0; y < m_resolution; y++)
                    {
                        for (z = 0; z < m_resolution; z++)
                        {

                            if (field[x, y, z].spline != null)
                            {
                                field[x, y, z].distance2ToSpline = 0;
                            }
                            else
                            {
                                for (int i = 0; i < hasSplines.Count; i++)
                                {
                                    FieldCell cell = hasSplines[i];
                                    cell.Vector.Normalize();

                                    for (int j = 0; j < cell.Points.Count; j++)
                                    {
                                        Vector3 splinePoint = cell.Points[j];

                                        Vector3 mag = new Vector3(
                                            splinePoint.x - (xmin + x * xmul),
                                            splinePoint.y - (ymin + y * ymul),
                                            splinePoint.z - (zmin + z * zmul));

                                        float dist2 = (float)(mag.sqrMagnitude / maxMag2);

                                        if (dist2 < field[x, y, z].distance2ToSpline)
                                        {
                                            field[x, y, z].distance2ToSpline = dist2;
                                            float approach = Mathf.Min(1, m_approach * (1f - UTween.EaseOutCubic01(Mathf.Sqrt(dist2))));
                                            field[x, y, z].vecToSpline = Vector3.Lerp(mag.normalized, cell.Vector, approach);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                doneEvent.Set();
            }

            public void VectorCallback(System.Object o)
            {

                int x, y, z;

                for (x = offsetX; x < m_resolution; x += span)
                {
                    for (y = 0; y < m_resolution; y++)
                    {
                        for (z = 0; z < m_resolution; z++)
                        {
                            if (field[x, y, z].spline == null)
                            {
                                field[x, y, z].Vector = field[x, y, z].vecToSpline.normalized;
                            }
                        }
                    }
                }

                doneEvent.Set();
            }
        } /* /Workgroup */

        private WorkGroup[] workGroups;
        private ManualResetEvent[] doneEvents;

        private class FieldCell
        {
            public Vector3 Vector;
            public Spline spline;
            public float distance2ToSpline;
            public Vector3 vecToSpline;
            public int x;
            public int y;
            public int z;
            public List<Vector3> Points = new List<Vector3>();

        }

        public enum OuterBehaviour
        {
            MoveToCenter = 0,
            ApplyVector = 1,
            AwayFromCenter = 2
        }

        public OuterBehaviour outerBehaviour;
        public Vector3 outerVector;

        public float forceDisplayCutoff = 0.6f;

        private List<FieldCell> hasSplines;

        [SerializeField]
        private int m_resolution = 10;
        public int Resolution
        {
            get { return m_resolution; }
            set
            {
                if (m_resolution == value)
                {
                    return;
                }
                m_resolution = value;
                regen = true;
            }
        }

        [SerializeField]
        private float m_approach = 0.5f;
        public float Approach
        {
            get
            {
                return m_approach;
            }

            set
            {
                if (m_approach == value)
                {
                    return;
                }
                m_approach = value;
                regen = true;
            }
        }

        [SerializeField]
        public bool ShowForces = true;

        [SerializeField]
        public bool ShowSplines = true;

        [SerializeField]
        public bool ShowTestPoints = true;

        [SerializeField]
        public float SpeedFactor;

        [SerializeField]
        private Vector3[] m_field; /* This should be a multidimensional array, but Unity doesn't serialize those. Sigh. */
        private Color[] m_colours;

        private bool regen = true; /* Something has changed. Regenerate everything. */

        [SerializeField]
        private float m_xmin;
        [SerializeField]
        private float m_ymin;
        [SerializeField]
        private float m_zmin;
        [SerializeField]
        private float m_xmax;
        [SerializeField]
        private float m_ymax;
        [SerializeField]
        private float m_zmax;

        private Vector3 VTWO = new Vector3(2, 2, 2);

        void OnValidate()
        {
            m_resolution = Math.Max(5, m_resolution);
            m_resolution = Math.Min(30, m_resolution);
            m_approach = Math.Max(0f, m_approach);
            m_approach = Math.Min(2.0f, m_approach);
            regen = true;
        }

        public void AddSpline()
        {
            GameObject go = new GameObject();
            go.tag = "EditorOnly";
            go.name = GameObjectHelper.GenerateNextName(typeof(Spline), "Spline", gameObject);
            go.AddComponent<Spline>();
            go.transform.parent = transform;

        }

        private void OnSplineChanged()
        {
            regen = true;
        }

        private void UpdateBounds()
        {
            m_xmin = transform.position.x - transform.lossyScale.x;
            m_ymin = transform.position.y - transform.lossyScale.y;
            m_zmin = transform.position.z - transform.lossyScale.z;

            m_xmax = transform.position.x + transform.lossyScale.x;
            m_ymax = transform.position.y + transform.lossyScale.y;
            m_zmax = transform.position.z + transform.lossyScale.z;

        }

        private FieldCell World2Cell(FieldCell[,,] cells, Vector3 pos)
        {
            if (pos.x < m_xmin || pos.x > m_xmax || pos.y < m_ymin || pos.y > m_ymax || pos.z < m_zmin || pos.z > m_zmax)
            {
                return null;
            }

            float x = m_resolution * (pos.x - m_xmin) / (m_xmax - m_xmin);
            float y = m_resolution * (pos.y - m_ymin) / (m_ymax - m_ymin);
            float z = m_resolution * (pos.z - m_zmin) / (m_zmax - m_zmin);
            
            return cells[
                Mathf.Clamp((int)x, 0, m_resolution - 1),
                Mathf.Clamp((int)y, 0, m_resolution - 1),
                Mathf.Clamp((int)z, 0, m_resolution - 1)];
        }

#if UNITY_EDITOR
        private void Regenerate()
        {

            regen = false;

            int i, x, y, z;

            FieldCell[,,] field = new FieldCell[m_resolution, m_resolution, m_resolution];
            if(this.hasSplines == null)
            {
                this.hasSplines = new List<FieldCell>();
            }
            else
            {
                this.hasSplines.Clear();
            }

            for (x = 0; x < m_resolution; x++)
            {
                for (y = 0; y < m_resolution; y++)
                {
                    for (z = 0; z < m_resolution; z++)
                    {
                        field[x, y, z] = new FieldCell()
                        {
                            Vector = Vector3.zero,
                            spline = null,
                            distance2ToSpline = float.MaxValue,
                            x = x,
                            y = y,
                            z = z
                        };
                    }
                }
            }


            /* First, fill vectors that trace along the path of each spline that passes through the field. */
            Spline[] splines = GetComponentsInChildren<Spline>();
            if (splines.Length <= 0)
            {
                return;
            }

            m_field = new Vector3[m_resolution * m_resolution * m_resolution];
            m_colours = new Color[m_resolution * m_resolution * m_resolution];

            UpdateBounds(); /* Make sure our worldspace limits are up-to-date */

            FieldCell lastHit = null;
            for (int j = 0; j < splines.Length; j++)
            {
                Spline spline = splines[j];

                spline.CrawlSpline((a, b) => {
                    FieldCell fc = World2Cell(field, a);
                    if (fc != null)
                    {
                        b -= a;
                        fc.Vector += b.normalized;
                        fc.spline = spline;
                        if (fc != lastHit)
                        {
                            this.hasSplines.Add(fc);
                            lastHit = fc;
                        }
                        lastHit.Points.Add(a);
                    }
                }, 60);
            }

            /* Secondly, calculate a crude path distance */
            int threadCount = SystemInfo.processorCount - 1;
            workGroups = new WorkGroup[threadCount];
            doneEvents = new ManualResetEvent[threadCount];

            for (i = 0; i < threadCount; i++)
            {
                workGroups[i] = new WorkGroup(i, threadCount, field, m_resolution, this.hasSplines, this.m_approach, m_xmin, m_ymin, m_zmin, transform.lossyScale);
                doneEvents[i] = workGroups[i].doneEvent;
                doneEvents[i].Reset();
                ThreadPool.QueueUserWorkItem(workGroups[i].DistanceCallback);
            }
            WaitHandle.WaitAll(doneEvents);

            /* Thirdly, fill in the gaps */
            for (i = 0; i < threadCount; i++)
            {
                doneEvents[i].Reset();
                ThreadPool.QueueUserWorkItem(workGroups[i].VectorCallback);
            }
            WaitHandle.WaitAll(doneEvents);


            /* Copy our new field into the serialized quaternion field */
            for (x = 0; x < m_resolution; x++)
            {
                for (y = 0; y < m_resolution; y++)
                {
                    for (z = 0; z < m_resolution; z++)
                    {
                        if (field[x, y, z].Vector == Vector3.zero)
                        {
                            int idx = z + m_resolution * (y + m_resolution * x);
                            m_field[idx] = Vector3.forward;
                            m_colours[idx] = Color.black;
                        }
                        else
                        {
                            int idx = z + m_resolution * (y + m_resolution * x);
                            m_field[idx] = field[x, y, z].Vector;
                            m_colours[idx] = Color.Lerp(Color.green, Color.red, 6*Mathf.Sqrt(field[x, y, z].distance2ToSpline));
                        }
                    }
                }
            }

        }

        void Awake()
        {
            regen = true;
        }

        void OnDrawGizmos()
        {
            if (regen)
            {
                Regenerate();
            }

            Gizmos.color = Selection.activeGameObject == gameObject? Color.yellow : Color.cyan;
            Gizmos.DrawWireCube(transform.position, Vector3.Scale(transform.lossyScale, VTWO));

            if (!ShowForces || m_field == null)
            {
                return;
            }

            float xmin = m_xmin + (transform.lossyScale.x / m_resolution);
            float ymin = m_ymin + (transform.lossyScale.y / m_resolution);
            float zmin = m_zmin + (transform.lossyScale.z / m_resolution);

            Vector3 pos = new Vector3();
            Quaternion q = new Quaternion();

            float midsize = HandleUtility.GetHandleSize(this.gameObject.transform.position);

            for (int x = 0; x < m_resolution; x++)
            {
                for (int y = 0; y < m_resolution; y++)
                {
                    for (int z = 0; z < m_resolution; z++)
                    {
                        pos.Set(
                            xmin + x * (transform.lossyScale.x * 2 / m_resolution),
                            ymin + y * (transform.lossyScale.y * 2 / m_resolution),
                            zmin + z * (transform.lossyScale.z * 2 / m_resolution));

                        int idx = z + m_resolution * (y + m_resolution * x);

                        Handles.color = m_colours[idx];

                        if (m_colours[idx].r < forceDisplayCutoff || forceDisplayCutoff == 1f)
                        {
                            q.SetLookRotation(m_field[idx]);
                            
                            Handles.ConeCap(0, pos, q, midsize * 0.1f); /* TODO: Draw these from back to front. */
                        }
                    }
                }
            }

        }
#endif

        public Vector3 SampleWorldCoord(Vector3 pos)
        {
            Vector3 c = transform.position;
            Vector3 ext = transform.localScale;

            if (pos.x > (c.x + ext.x) || pos.x < m_xmin ||
                pos.y > (c.y + ext.y) || pos.y < m_ymin ||
                pos.z > (c.z + ext.z) || pos.z < m_zmin)
            {
                switch (outerBehaviour)
                {
                    case OuterBehaviour.ApplyVector:
                        return this.outerVector;

                    case OuterBehaviour.AwayFromCenter:
                        pos = pos - c;
                        return pos.normalized;
                    case OuterBehaviour.MoveToCenter:
                    default:
                        pos = c - pos;
                        return pos.normalized;
                }
            }

            int maxr = m_resolution - 1;

            int ix = Mathf.Clamp((int)(((pos.x - m_xmin) / (2 * ext.x)) * m_resolution), 0, maxr);
            int iy = Mathf.Clamp((int)(((pos.y - m_ymin) / (2 * ext.y)) * m_resolution), 0, maxr);
            int iz = Mathf.Clamp((int)(((pos.z - m_zmin) / (2 * ext.z)) * m_resolution), 0, maxr);

        
            int idx = iz + m_resolution * (iy + m_resolution * ix);

            return m_field[idx]*SpeedFactor;
        }
    }
}
