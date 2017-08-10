using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundObject : MonoBehaviour {

    public Material glowMat;
    public AudioSource audio;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Collision!");
        if(collision.gameObject.name == "GlowBall")
        {
            Debug.Log("Collision with Glowball!");
            audio.Play();
            StartCoroutine(Glow());
        }
    }

    IEnumerator Glow()
    {
        Debug.Log("Glowing Up!");
        Color col = glowMat.GetColor("_EmissionColor");
        Color tarCol = Color.white;
        while (col.r < 0.99f)
        {
            col = Color.Lerp(col, tarCol, 0.001f);
            glowMat.SetColor("_EmissionColor", col);
            yield return null;
        }
        Debug.Log("Done Glowing Up!");
        // Emit more!
    }

    private void OnApplicationQuit()
    {
        glowMat.SetColor("_EmissionColor", Color.black);

    }

}
