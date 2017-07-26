using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourceController : ValueSetter {

    public AudioSource source;
	
	// Update is called once per frame
	void Update () {
        source.volume = setValue(source.volume);
	}
}
