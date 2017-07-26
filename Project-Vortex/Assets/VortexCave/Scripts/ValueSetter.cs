using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValueSetter : MonoBehaviour {

    public GameController gameController;

    [Range(0.0f, 1.0f)]
    public float attack = 0.01f;
    [Range(0.0f, 100.0f)]
    public float percentageStart = 99.0f;
    [Range(0.0f, 100.0f)]
    public float percentageEnd = 100.0f;
    public float minVal = 0.0f;
    public float maxVal = 1.0f;
    // Use this for initialization
    void Start()
    {
        if (!gameController)
            gameController = FindObjectOfType<GameController>();
    }

    public float setValue(float value)
    {
        return Mathf.Lerp(value, gameController.map(gameController.percentage, percentageStart, percentageEnd, minVal, maxVal, true), attack);
    }
}
