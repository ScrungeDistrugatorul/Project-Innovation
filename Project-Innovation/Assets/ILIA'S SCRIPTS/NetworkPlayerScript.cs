using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class NetworkPlayerScript : NetworkBehaviour
{

    [SerializeField] private Rigidbody rb;
    [SerializeField] private FixedJoystick fixedJoystick;
    [SerializeField] private float moveSpeed;

    private void Awake()
    {

        // if (!IsOwner) { return; } //do a check if it controls a specific player

        GameObject gameObject = GameObject.Find("Player Input(Clone)");

        fixedJoystick = gameObject.transform.Find("Fixed Joystick").GetComponent<FixedJoystick>();
    }

    private void FixedUpdate()
    {
        if (!IsOwner) { return; }

        rb.velocity = new Vector3(fixedJoystick.Horizontal * moveSpeed, rb.velocity.y, fixedJoystick.Vertical * moveSpeed);
    }
}
