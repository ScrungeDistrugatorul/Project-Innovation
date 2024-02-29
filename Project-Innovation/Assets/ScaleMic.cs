using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleMic : MonoBehaviour
{
    public AudioSource source;
    public Vector3 minScale;
    public Vector3 maxScale;
    public AudioLoudnessDetection detector;

    public float loudnessSensibility = 100;
    public float threshold = 0.1f;
    public float smoothness = 5f; // Adjust this value for the smoothness of scaling

    private Vector3 targetScale;

    void Update()
    {
        float loudness = detector.GetLoudnessFromMicrophone() * loudnessSensibility;

        if (loudness < threshold)
            loudness = 0;

        // Smoothly interpolate between the current scale and the target scale
        targetScale = Vector3.Lerp(minScale, maxScale, loudness);
        targetScale = new Vector3(
            Mathf.Clamp(targetScale.x, minScale.x, maxScale.x),
            Mathf.Clamp(targetScale.y, minScale.y, maxScale.y),
            Mathf.Clamp(targetScale.z, minScale.z, maxScale.z)
        );

        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * smoothness);
    }
}
