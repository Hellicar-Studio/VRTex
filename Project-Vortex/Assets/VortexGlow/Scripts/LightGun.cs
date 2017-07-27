using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightGun : MonoBehaviour {
    public GameObject l;
    private Material mat;
    private Transform t;
    public LightManager lightManager;
    private Rigidbody bulletBody;
	// Use this for initialization
	void Start () {
        t = this.transform;
        l = FindObjectOfType<LightBullet>().gameObject;
        lightManager = FindObjectOfType<LightManager>();
        mat = l.GetComponent<MeshRenderer>().material;
        bulletBody = l.GetComponent<Rigidbody>();
        bulletBody.interpolation = RigidbodyInterpolation.Interpolate;
    }
	
	// Update is called once per frame
	void Update () {
        if(Input.GetKeyUp("p"))
        {
            fireLight();
        }
    }

    void fireLight()
    {
        l.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        StartCoroutine(Fade(new Color(1.0f, 1.0f, 1.0f)));
        bulletBody.velocity = transform.forward;
    }

    IEnumerator Fade(Color target)
    {
        Color col = mat.GetColor("_EmissionColor");
        while(col != target)
        {
            col = Color.Lerp(col, target, 0.1f);
            mat.SetColor("_EmissionColor", col);
            yield return null;
        }
    }
}
