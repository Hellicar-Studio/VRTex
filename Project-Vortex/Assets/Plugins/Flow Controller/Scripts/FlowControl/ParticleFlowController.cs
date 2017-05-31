/* Copyright Kupio Limited SC426881. All rights reserved. Source not for distribution. */

namespace com.kupio.FlowControl
{
    using UnityEngine;

    /// <summary>
    /// Add this component to a particle system GameObject to have it dictate the movement
    /// of particles according to a flow control region.
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleFlowController : MonoBehaviour
    {
        private ParticleSystem particleSys;
        private ParticleSystem.Particle[] particles;


        public FlowControlRegion flowControlRegion;

        [Range(0f, 1f)]
        public float DirectionalInertia = 0f;
        public float Speed = 3f;

        void Update()
        {
            if (particles == null)
            {
                particleSys = GetComponent<ParticleSystem>();
                particles = new ParticleSystem.Particle[particleSys.main.maxParticles];
            }

            int numParticles = particleSys.GetParticles(this.particles);

            float scaledInertia = 0.9f + DirectionalInertia / 10f;

            for (int i = 0; i < numParticles; i++)
            {
                ParticleSystem.Particle p = particles[i];
                Vector3 force = flowControlRegion.SampleWorldCoord(p.position);

                if (p.velocity == Vector3.zero)
                {
                    particles[i].velocity = force * Speed;
                }
                else
                {
                    particles[i].velocity = (p.velocity * scaledInertia) + (force * Speed * (1f - scaledInertia));
                    /* Note: To be totally perfect, we should normalize and multiply by Speed, but it doesn't seem to make much difference. */
                }
            }

            particleSys.SetParticles(particles, numParticles);
            
        }
    }
}