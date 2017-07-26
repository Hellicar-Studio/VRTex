using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.kupio.FlowControl;


public class FlowRegionSpeedController : ValueSetter {

    public FlowControlRegion flowController;
	
	// Update is called once per frame
	void Update () {
        flowController.SpeedFactor = setValue(flowController.SpeedFactor);
	}
}
