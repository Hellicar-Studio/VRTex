using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    public AudioInput input;

    [Range(0, 100)]
    public float percentage;

    public float progressRate = 0.1f;
    public float regressRate = 0.001f;

    public float progressLevel = 100;

    public float map(float value, float low1, float high1, float low2, float high2, bool clamp = false)
    {
        if(low2 > high2)
        {
            float temp = high2;
            high2 = low2;
            low2 = temp;
        }
        float output = low2 + (value - low1) * (high2 - low2) / (high1 - low1);
        if(clamp)
        {
            if (output < low2)
                return low2;
            if (output > high2)
                return high2;
        }
        return output;
    }

    // Use this for initialization
    void Start () {
        if (!input)
            input = FindObjectOfType<AudioInput>();
        percentage = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
        if(input.MicLoudness > progressLevel)
        {
            percentage += progressRate;
        } else if(percentage > 0)
        {
            percentage -= regressRate;
        }
        if (percentage > 120)
        {
            //SceneManager.LoadScene("Vortex-Outro_01", LoadSceneMode.Single);
        //    //end!
        //    percentage = 0.1f;
        }

    }
}
