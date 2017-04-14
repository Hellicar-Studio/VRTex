using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingController : ValueSetter {

    public Light l1;
	
	// Update is called once per frame
	void Update () {
        l1.intensity = setValue(l1.intensity);
	}
}
