using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.kupio.FlowControl;


public class FlowRegionYController : ValueSetter {

    public Transform flowController;
	
	// Update is called once per frame
	void Update () {
        float y = setValue(flowController.localPosition.y);
        flowController.localPosition = new Vector3(flowController.localPosition.x, y, flowController.localPosition.z);
	}
}
