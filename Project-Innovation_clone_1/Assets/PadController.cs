using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PadController : NetworkBehaviour
{
    public int amountOfPlayersOnPlatform = 0;

    public string nameOfScene;

    private void Start()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            amountOfPlayersOnPlatform++;

            if (IsServer && amountOfPlayersOnPlatform == NetworkManager.Singleton.ConnectedClients.Count)
            {
                NetworkManager.SceneManager.LoadScene(nameOfScene, UnityEngine.SceneManagement.LoadSceneMode.Single);
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
