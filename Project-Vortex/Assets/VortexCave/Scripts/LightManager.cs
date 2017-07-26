using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour {

    public GameObject[] lights;
    public Material mat;
    public Material[] lightMats;
    public Color col;
	// Use this for initialization
	void Start () {
        Debug.Log("Lights Length " + lights.Length);
        lightMats = new Material[lights.Length];
        for(int i = 0; i < lights.Length; i++)
        {
            lightMats[i] = lights[i].GetComponent<MeshRenderer>().material;
        }
    }
	
	// Update is called once per frame
	void Update () {
        for(int i = 0; i < lights.Length; i++)
        {
            lightMats[i].SetColor("_RimColor", col);
        }
        Vector4[] lightPositions = new Vector4[lights.Length];
        for(int i = 0; i < lights.Length; i++)
        {
            lightPositions[i] = lights[i].transform.position;
        }
        Vector4[] lightColors = new Vector4[lights.Length];
        for(int i = 0; i < lights.Length; i++)
        {
            lightColors[i] = lightMats[i].GetColor("_RimColor");
        }
        mat.SetVectorArray("_LightPos", lightPositions);
        mat.SetVectorArray("_LightCol", lightColors);
	}
}
