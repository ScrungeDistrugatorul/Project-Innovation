using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetworkPlayerScript : NetworkBehaviour
{

    [SerializeField] private Rigidbody rb;

    [Header ("Lobby related")]
    [SerializeField] private FixedJoystick fixedJoystick;
    [SerializeField] private float moveSpeed;
    [SerializeField] GameObject lobbyCharacter;

    [Header("Baloon game related")]
    [SerializeField] GameObject baloon;
    bool isTrue= false;

    GameObject slider;

    [Header("Color switch related")] 
    [SerializeField] private GameObject spawner;

    public int numberOfPlayers;

    private void Awake()
    {

        // if (!IsOwner) { return; } //do a check if it controls a specific player

        numberOfPlayers = LobbyManager.Instance.GetJoinedLobby().Players.Count - 1;

        if (SceneManager.GetActiveScene().name == "ILIA'S SCENE")
        {
            GameObject gameObject = GameObject.Find("Player Input(Clone)");

            fixedJoystick = gameObject.transform.Find("Fixed Joystick").GetComponent<FixedJoystick>();
        }
            
    }

    private void FixedUpdate()
    {
        if (!IsOwner) { return; }

        if (SceneManager.GetActiveScene().name == "ILIA'S SCENE")
        {
            rb.velocity = new Vector3(fixedJoystick.Horizontal * moveSpeed, rb.velocity.y, fixedJoystick.Vertical * moveSpeed);
        }

        if (SceneManager.GetActiveScene().name == "BALOON GAME" && slider != null)
        {
            rb.AddForce(transform.up * slider.GetComponent<Slider>().value * 18);
        }
    }

    private void Update()
    {
        if (!IsOwner) { return; }

        if (SceneManager.GetActiveScene().name == "BALOON GAME" && !isTrue)
        {
            rb.velocity = new Vector3(0, 0, 0);

            float areaForEachPlayer = 32 / numberOfPlayers;

            transform.position = new Vector3(((NetworkManager.LocalClientId-1) * areaForEachPlayer + areaForEachPlayer/2) - 16, -3, 0);
            rb.velocity = new Vector3(0, 0, 0);

            lobbyCharacter.SetActive(false);
            baloon.SetActive(true);
            ChangeServerRpc();

            isTrue = true;

            slider = GameObject.Find("Slider");
        }

        if (SceneManager.GetActiveScene().name == "Adrian Test2")
        {
            transform.position = new Vector3(((NetworkManager.LocalClientId-1) * areaForEachPlayer + areaForEachPlayer/2) - 16, -3, 0);

            lobbyCharacter.SetActive(false);
            spawner.SetActive(true);
            ColorSwtichServerRpc();
        }
    }

    //[ServerRpc(RequireOwnership = false)]
    //public void SetNumberOfPlayersServerRPC()
    //{
    //    numberOfPlayers = NetworkManager.Singleton.ConnectedClients.Count;
    //}

    [ServerRpc]
    public void GetClientsServerRpc()
    {
        numberOfPlayers = NetworkManager.Singleton.ConnectedClients.Count;
    }

    [ServerRpc]
    public void ChangeServerRpc()
    {
        lobbyCharacter.SetActive(false);
        baloon.SetActive(true);
    }

    [ServerRpc]
    public void ColorSwtichServerRpc()
    {
        lobbyCharacter.SetActive(false);
        spawner.SetActive(true);
    }
}
