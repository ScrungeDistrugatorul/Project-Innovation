using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PadController : NetworkBehaviour
{
    public int amountOfPlayersOnPlatform = 0;

    private void Start()
    {
        //if (IsServer)
        //{
        //    Debug.LogError(NetworkManager.Singleton.ConnectedClients.Count);
        //}
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            amountOfPlayersOnPlatform++;

            if (IsServer && amountOfPlayersOnPlatform == NetworkManager.Singleton.ConnectedClients.Count)
            {
                NetworkManager.SceneManager.LoadScene("BALOON GAME", UnityEngine.SceneManagement.LoadSceneMode.Single);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            amountOfPlayersOnPlatform--;
        }
    }

    void StartTheTimer()
    {

    }
}
