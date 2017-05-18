// This takes the audio input and scales the particle emission with the audio level.

using UnityEngine;
using System.Collections;

public class AudioEmitter : MonoBehaviour {

    public AudioInput input;
    public ParticleSystem particles;
    public float loudness;
    public bool networked = false;
	// Use this for initialization
	void Start () {
        // Setup the particle system and audioInput if they aren't already assigned
        if (!particles)
            particles = GetComponent<ParticleSystem>();
        if (!input)
            input = FindObjectOfType<AudioInput>();
    }
	
	// Update is called once per frame
	void Update () {
        ParticleSystem.EmissionModule emission = particles.emission;
        if(!networked)
        {
            loudness = input.MicLoudness / 3;
        }
        emission.rateOverTime = loudness;
    }

    public void setLoudness(float _loudness)
    {
        loudness = _loudness;
    }
}
