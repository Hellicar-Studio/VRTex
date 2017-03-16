/* Copyright Kupio Limited SC426881. All rights reserved. Source not for distribution. */

namespace com.kupio.FlowControl
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// A quick and simple example of the boids algorithm to show how a flow controller can be
    /// used as a steering value.
    /// For boid simulations, you're probably best writing one yourself that suits your own
    /// needs.
    /// </summary>
    public class FlockingBoidSteering : MonoBehaviour
    {
        private Boid m_boidSelf;
        private BoidRegion m_boidRegion;

        public float Speed = 2f;

        void Start()
        {
            m_boidSelf = GetComponent<Boid>();
            m_boidRegion = m_boidSelf.boidRegion;
            //m_boidRegion.Register(m_boidSelf);
        }

        void Update()
        {
            FlowControlRegion control = m_boidSelf.controlRegion;

            if (control != null)
            {
                Vector3 dir = control.SampleWorldCoord(transform.position);

                List<Boid> boids = m_boidRegion.GetBoids();

                float sqrSpace = 1 * 1;
                float sqrSight = 2 * 2;

                for (int i = 0; i < boids.Count; i++)
                {
                    Boid boid = boids[i];

                    if (boid != this)
                    {
                        Vector3 gap = boid.transform.position - transform.position;
                        float sqrDistance = gap.sqrMagnitude;

                        if (sqrDistance < sqrSpace)
                        {
                            dir -= (gap * 0.85f); /* Separate */
                        }
                        else if (sqrDistance < sqrSight)
                        {
                            dir += (gap * 0.05f); /* Attract */
                        }

                        if (sqrDistance < sqrSight)
                        {
                            dir += boid.Velocity * 0.03f; /* Align */
                        }
                    }
                }

                m_boidSelf.Velocity = 0.45f * (m_boidSelf.Velocity * 1.8f + dir.normalized); /* Inertia */

                transform.position = transform.position + m_boidSelf.Velocity * Time.deltaTime * Speed;
            }

        }
    }
}
