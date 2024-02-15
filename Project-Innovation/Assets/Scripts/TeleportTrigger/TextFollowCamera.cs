using System;
using UnityEngine;

public class TextFollowCamera : MonoBehaviour
{
    private Transform _mainCam;

    private void Start()
    {
        if (Camera.main != null) _mainCam = Camera.main.transform;
    }

    private void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - _mainCam.transform.position);
    }
}
