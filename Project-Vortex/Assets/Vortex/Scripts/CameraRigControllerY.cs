using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRigControllerY : ValueSetter {

    public Transform t;

	// Update is called once per frame
	void Update () {
        float y = setValue(t.localPosition.y);
        t.localPosition = new Vector3(t.localPosition.x, y, t.localPosition.z);
	}
}
