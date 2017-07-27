using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System.Collections.Generic;

public class LightManager : MonoBehaviour {

    public List<GameObject> lights;
    public Material[] mats;
    public List<Material> lightMats;
	// Use this for initialization
	void Start () {
        for(int i = 0; i < lights.Count; i++)
        {
            lightMats.Add(lights[i].GetComponent<MeshRenderer>().material);
        }
        Debug.Log(lightMats.Count);
    }
	
	// Update is called once per frame
	void Update () {
        Vector4[] lightPositions = new Vector4[lights.Count];
        for(int i = 0; i < lights.Count; i++)
        {
            lightPositions[i] = lights[i].transform.position;
        }
        Vector4[] lightColors = new Vector4[lights.Count];
        for(int i = 0; i < lights.Count; i++)
        {
            lightColors[i] = lightMats[i].GetColor("_EmissionColor");
        }
        for (int i = 0; i < mats.Length; i++)
        {
            mats[i].SetVectorArray("_LightPos", lightPositions);
            mats[i].SetVectorArray("_LightCol", lightColors);
        }
	}

    public void addLight(GameObject l)
    {
        lights.Add(l);
        lightMats.Add(l.GetComponent<MeshRenderer>().material);
    }
}
