using UnityEngine;
using System.Collections;
using VRStandardAssets.Utils;

public class AudioButtonOverride : MonoBehaviour {
	public VRInput m_VRInput;
    public AudioInput audioInput;
    private float timeOverriden;
    public float overrideDuration = 1.0f;
	// Use this for initialization
	void Start () {
        if (!m_VRInput)
            m_VRInput = FindObjectOfType<VRInput>();
        if (!audioInput)
            audioInput = FindObjectOfType<AudioInput>();

        if(m_VRInput)
        {
            m_VRInput.OnDown += StartEmit;
            m_VRInput.OnUp += StopEmit;
        }

        timeOverriden = Time.time;
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKey(KeyCode.W))
        {
            StartEmit();
        }
        else
        {
            if(!audioInput.Active)
            {
                StopEmit();
            }
        }

        if(!audioInput.Active)
        {
            if(Time.time - timeOverriden > overrideDuration)
            {
                audioInput.Active = true;
            }
        }
	}

	void StartEmit() {
        audioInput.Active = false;
        timeOverriden = Time.time;
        audioInput.MicLoudness = Mathf.Lerp(audioInput.MicLoudness, 1000.0f, 0.1f);
    }

	void StopEmit() {
        audioInput.MicLoudness = Mathf.Lerp(audioInput.MicLoudness, 0.0f, 0.1f);
    }
}
