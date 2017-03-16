using UnityEngine;
using System.Collections;

public enum Frequency
{
    Speech = 8000,
    LowQuality = 11025,
    Wideband = 16000,
    Record = 22050,
    DAT = 32000,
    CD = 44100,
    Pro = 48000,
    DVD = 96000

}
public class AudioInput : MonoBehaviour
{

    public float MicLoudness;

    public Frequency samplingRate = Frequency.Wideband;
    public int lengthSeconds = 1;

    private string _device;

    //mic initialization
    void InitMic()
    {
        if (_device == null) _device = Microphone.devices[0];
        _clipRecord = Microphone.Start(_device, true, lengthSeconds, (int)samplingRate);
    }

    void StopMicrophone()
    {
        Microphone.End(_device);
    }

    AudioClip _clipRecord = new AudioClip();
    int _sampleWindow = 128;

    //get data from microphone into audioclip
    float LevelMax()
    {
        float levelMax = 0;
        float[] waveData = new float[_sampleWindow];
        int micPosition = Microphone.GetPosition(null) - (_sampleWindow + 1); // null means the first microphone
        if (micPosition < 0) return 0;
        _clipRecord.GetData(waveData, micPosition);
        // Getting a peak on the last 128 samples
        for (int i = 0; i < _sampleWindow; i++)
        {
            float wavePeak = waveData[i] * waveData[i];
            if (levelMax < wavePeak)
            {
                levelMax = wavePeak;
            }
        }
        return levelMax;
    }

    // Use this for initialization
    void Start()
    {
        InitMic();
        _isInitialized = true;
    }

    void Update()
    {
        // levelMax equals to the highest normalized value power 2, a small number because < 1
        // pass the value to a static var so we can access it from anywhere

        // if muted, just switch variable to zero, don't actually disable hardware as Unity update was
        // having probs with stop and starting mics
        MicLoudness = LevelMax() * 1000f;
    }

    bool _isInitialized;
    // start mic when scene starts
    void OnEnable()
    {
        InitMic();
        _isInitialized = true;
    }

    //stop mic when loading a new level or quit application
    void OnDisable()
    {
        StopMicrophone();
    }

    void OnDestroy()
    {
        StopMicrophone();
    }

    // make sure the mic gets started & stopped when application gets focused
    void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            //Debug.Log("Focus");

            if (!_isInitialized)
            {
                //Debug.Log("Init Mic");
                InitMic();
                _isInitialized = true;
            }
        }
        //if (!focus)
        //{
        //    Debug.Log("Pause");
        //    StopMicrophone();
        //    Debug.Log("Stop Mic");
        //    _isInitialized=false;
        //}
    }
}