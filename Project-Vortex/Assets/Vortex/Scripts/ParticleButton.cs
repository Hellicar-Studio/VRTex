using UnityEngine;
using System.Collections;
using VRStandardAssets.Utils;

public class ParticleButton : MonoBehaviour {

	public ParticleSystem particleSystem;
	public VRInput m_VRInput;

    public AudioInput audioInput;
	// Use this for initialization
	void Start () {
		if (!particleSystem)
			particleSystem = GetComponent<ParticleSystem> ();
        if (!m_VRInput)
            m_VRInput = FindObjectOfType<VRInput>();
        if (!audioInput)
            audioInput = FindObjectOfType<AudioInput>();

        m_VRInput.OnDown += StartEmit;
        m_VRInput.OnUp += StopEmit;

    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKey(KeyCode.W))
        {
            StartEmit();
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            StopEmit();
        }
	}

	void StartEmit() {
		ParticleSystem.EmissionModule emission = particleSystem.emission;
		emission.rate = 100;
	}

	void StopEmit() {
		ParticleSystem.EmissionModule emission = particleSystem.emission;
		emission.rate = 0;
	}
}
