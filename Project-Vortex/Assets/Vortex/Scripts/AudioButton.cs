using UnityEngine;
using System.Collections;

public class AudioButton : MonoBehaviour {

    public AudioInput input;

	// Use this for initialization
	void Start () {
        if (input == null)
            input = FindObjectOfType<AudioInput>();
	}
	
	// Update is called once per frame
	void Update () {
        transform.localScale = new Vector3(1, input.MicLoudness, 1);
	}
}
