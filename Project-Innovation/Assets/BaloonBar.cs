using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaloonBar : MonoBehaviour
{
    public AudioSource source;
    public Vector3 minScale;
    public Vector3 maxScale;
    public AudioLoudnessDetection detector;

    public float loudnessSensibility = 100;
    public float threshold = 0.1f;
    public float smoothness = 5f; // Adjust this value for the smoothness of scaling

    private Vector3 targetScale;

    public float loudness;
    public Slider slider;

    public Rigidbody rb;

    void Update()
    {
        loudness = detector.GetLoudnessFromMicrophone() * loudnessSensibility;

        if (loudness < threshold)
            loudness = 0;

        float targetValue = Mathf.Lerp(0, 1, loudness);
        targetValue = Mathf.Clamp(targetValue, 0, 1);
        slider.value = Mathf.Lerp(slider.value, targetValue, Time.deltaTime * smoothness);

        //rb.AddForce(transform.right * slider.value * 3);

        //targetScale = Vector3.Lerp(minScale, maxScale, loudness);
        //targetScale = new Vector3(
        //    Mathf.Clamp(targetScale.x, minScale.x, maxScale.x),
        //    Mathf.Clamp(targetScale.y, minScale.y, maxScale.y),
        //    Mathf.Clamp(targetScale.z, minScale.z, maxScale.z)
        //);

        //transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * smoothness);
    }
}
