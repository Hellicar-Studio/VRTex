using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadWobbler : MonoBehaviour {

    public float speed;
    public float noiseOffset;
    public float noiseMagnitude;
    public float val;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        val = Mathf.PerlinNoise((Time.time + noiseOffset) * speed, (Time.time + noiseOffset) * speed);
        val -= 0.5f;
        val *= 2.0f;
        this.transform.rotation = Quaternion.Euler(0, val * noiseMagnitude, 0);//.AngleAxis(val * noiseMagnitude, new Vector3(0, 1, 0));
	}
}
