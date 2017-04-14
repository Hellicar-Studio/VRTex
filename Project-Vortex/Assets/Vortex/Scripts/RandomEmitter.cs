// This takes the audio input and scales the particle emission with the audio level.

using UnityEngine;
using System.Collections;

public class RandomEmitter : MonoBehaviour {

    public ParticleSystem particleSystem;
    public float noiseStep;
    public float noiseScale;
    public float noiseOffset;

    float map(float value, float low1, float high1, float low2, float high2)
    {
        return low2 + (value - low1) * (high2 - low2) / (high1 - low1);
    }

    // Use this for initialization
    void Start () {
        // Setup the particle system and audioInput if they aren't already assigned
        if (!particleSystem)
            particleSystem = GetComponent<ParticleSystem>();
    }
	
	// Update is called once per frame
	void Update () {
        float value = noiseScale * Mathf.PerlinNoise(Time.time * noiseStep + noiseOffset, 0.0f);
        ParticleSystem.EmissionModule emission = particleSystem.emission;
        emission.rateOverTime = map(value, 0.4f, 1.0f, 0.0f, 300.0f);//input.MicLoudness/3;
    }
}
