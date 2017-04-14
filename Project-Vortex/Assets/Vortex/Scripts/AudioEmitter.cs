// This takes the audio input and scales the particle emission with the audio level.

using UnityEngine;
using System.Collections;

public class AudioEmitter : MonoBehaviour {

    public AudioInput input;
    public ParticleSystem particleSystem;
	// Use this for initialization
	void Start () {
        // Setup the particle system and audioInput if they aren't already assigned
        if (!particleSystem)
            particleSystem = GetComponent<ParticleSystem>();
        if (!input)
            input = FindObjectOfType<AudioInput>();
    }
	
	// Update is called once per frame
	void Update () {
        ParticleSystem.EmissionModule emission = particleSystem.emission;
        emission.rateOverTime = input.MicLoudness/3;
    }
}
