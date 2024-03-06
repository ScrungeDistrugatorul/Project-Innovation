using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ColorSwitchManager : NetworkBehaviour
{
    [SerializeField] GameObject serverCanvas;
    [SerializeField] GameObject clientCanvas;

    void Start()
    {
        if (IsServer)
        {
            clientCanvas.SetActive(false);
        }
        else
        {
            serverCanvas.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
