using UnityEngine;
using FMODUnity;
using FMOD;
using System;

public class AudioLoudnessDetection : MonoBehaviour
{

    public int sampleWindow = 64;
    private AudioClip microphoneClip;

   void Start()
    {
        MicrophoneToAudioClip();
    }

    private void Update()
    {
        
    }

    public void MicrophoneToAudioClip()
    {
        UnityEngine.Debug.Log(Microphone.devices);
        string microphoneName = Microphone.devices[0];
        microphoneClip = Microphone.Start(microphoneName, true, 20, AudioSettings.outputSampleRate);
    }

    public float GetLoudnessFromMicrophone()
    {
        if (Microphone.IsRecording(Microphone.devices[0]))
        {
            return GetLoudnessFromAudioClip(Microphone.GetPosition(Microphone.devices[0]), microphoneClip);
        }
        else
        {
            return 0f; // or handle the case when the microphone is not recording
        }
    }

    public float GetLoudnessFromAudioClip(int clipPosition, AudioClip clip)
    {
        int startPosition = Mathf.Max(0, clipPosition - sampleWindow);
        float[] waveData = new float[sampleWindow];
        clip.GetData(waveData, startPosition);

        float Loudness = 0;

        for (int i = 0; i < sampleWindow; i++)
        {
            Loudness += Mathf.Abs(waveData[i]);
        }

        return Loudness / sampleWindow;
    }


}
