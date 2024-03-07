using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Unity.Collections;

public class NetworkPlayerScript : NetworkBehaviour
{


    NetworkVariable<FixedString128Bytes> _netPlayerName = new NetworkVariable<FixedString128Bytes>();

    [SerializeField] string playerName;

    [SerializeField] TMP_Text playerNameText;

    public int numberOfPlayers;

    [SerializeField] private Rigidbody rb;

    [Header ("Lobby related")]
    [SerializeField] private FixedJoystick fixedJoystick;
    [SerializeField] private float moveSpeed;
    [SerializeField] GameObject lobbyCharacter;

    bool lobbyBool = false;

    [SerializeField] Transform tutorialScreenObject;


    [Header("Baloon game related")]
    [SerializeField] GameObject baloon;
    bool ballonGameBool= false;
    GameObject slider;
    public GameObject colorSwticher;

    TutorialScreen balloonTutorialScreenScript;

    [Header("Color switch related")] 
    [SerializeField] private GameObject spawner;

    GameObject clientCanvas;

    public Gyroscope gyroscopeScript;

    bool colorSwitchBool = false;

    [SerializeField] GameObject[] trashPrefab;
    public TMP_Text scoreText;

    public int score = 0;

    private bool temporaryBool = false;

    [Header("Difficulty setting for color switch")]
    [Range(0.1f, 10.0f)] public float spawnRate;
    [Range(1.0f, 20.0f)] public float difficultyIncreaseScale;
    [Range(0f, 3.0f)] public float minValue;
    [Range(0f, 3.0f)] public float maxValue;
    private float _nextSpawn;
    private float _difficultyScale;

    int previousColor = -1;
    int whichColorToSpawn = -2;
    
    [SerializeField] ColorCheck colorCheckScript;

    private void Awake()
    {

        // if (!IsOwner) { return; } //do a check if it controls a specific player

        numberOfPlayers = LobbyManager.Instance.GetJoinedLobby().Players.Count - 1;

        if (SceneManager.GetActiveScene().name == "ILIA'S SCENE")
        {
            GameObject gameObject = GameObject.Find("Player Input(Clone)");

            fixedJoystick = gameObject.transform.Find("Fixed Joystick").GetComponent<FixedJoystick>();

            playerName = LobbyManager.Instance.GetPlayerName();

            _netPlayerName.Value = playerName;
            //CmdChangeNameServerRpc(playerName);



            //if (!IsOwner) { return; }
            //SetPlayerNameTextServerRpc();
        }
            
    }


    //[ServerRpc (RequireOwnership = false)]
    //public void CmdChangeNameServerRpc(string newName)
    //{
    //    pname.Value = newName;
    //    //playerNameText.text = pname.Value.ToString();
    //    Debug.LogError(pname.Value.ToString());

    //    Debug.LogError("Fuck you");
    //}

    [ServerRpc]
    public void TestServerRpc(string _playerName)
    {
        playerNameText.text = _playerName;
        //Debug.LogError("Client id: " + serverRpcParams.Receive.SenderClientId);
    }

    private void FixedUpdate()
    {
        if (!IsOwner) { return; }

        if (SceneManager.GetActiveScene().name == "ILIA'S SCENE")
        {
            rb.velocity = new Vector3(fixedJoystick.Horizontal * moveSpeed, rb.velocity.y, fixedJoystick.Vertical * moveSpeed);
        }

    }

    private void Update()
    {
        if (!IsOwner) { return; }

        if (SceneManager.GetActiveScene().name == "ILIA'S SCENE" && !lobbyBool)
        {

            TestServerRpc(playerName);
            lobbyBool = true;
        }

        if (SceneManager.GetActiveScene().name == "BALOON GAME" && !ballonGameBool)
        {
            rb.velocity = new Vector3(0, 0, 0);

            float areaForEachPlayer = 32 / numberOfPlayers;

            transform.position = new Vector3(((NetworkManager.LocalClientId - 1) * areaForEachPlayer + areaForEachPlayer / 2) - 16, -3, 0);
            rb.velocity = new Vector3(0, 0, 0);

            lobbyCharacter.SetActive(false);
            baloon.SetActive(true);
            ChangeBalloonServerRpc();

            ballonGameBool = true;

            slider = GameObject.Find("Slider");

            //tutorialScreenObject = transform.Find("Tutorial screen");

            //balloonTutorialScreenScript = tutorialScreenObject.GetComponent<TutorialScreen>();
        }

        if (SceneManager.GetActiveScene().name == "BALOON GAME" && slider != null && TutorialScreen.Instance.readyToPlay)
        {

            rb.AddForce(transform.up * slider.GetComponent<Slider>().value * 40);
        }

        if (SceneManager.GetActiveScene().name == "COLOR SWITCH" && clientCanvas != null && TutorialScreen.Instance.readyToPlay)
        {
            Debug.LogError("ColorSwitchManager.Instance value" + ColorSwitchManager.Instance.timeIsUp);

            if (ColorSwitchManager.Instance.timeIsUp && !temporaryBool)
            {
                temporaryBool = true;

                ColorSwitchManager.Instance.ToRunRpcFromPlayer(score, (int)NetworkManager.LocalClientId, playerName);
            }
            else
            {
                colorCheckScript.SetTheColor(gyroscopeScript.whichColor);
                score = colorCheckScript.score;
                gyroscopeScript.text.text = "Score: " + score;

                TimedFunction();
            }

        }

        if (SceneManager.GetActiveScene().name == "COLOR SWITCH" && !colorSwitchBool)
        {
            rb.velocity = new Vector3(0, 0, 0);
            rb.useGravity = false;

            float areaForEachPlayer = 32 / numberOfPlayers;

            transform.position = new Vector3(((NetworkManager.LocalClientId - 1) * areaForEachPlayer + areaForEachPlayer / 2) - 16, 6.5f, 0);
            //rb.velocity = new Vector3(0, 0, 0);

            lobbyCharacter.SetActive(false);
            spawner.SetActive(true);
            ChangeColorSwitchServerRpc();

            colorSwitchBool = true;

            clientCanvas = GameObject.Find("Client stuff");
            gyroscopeScript = clientCanvas.GetComponent<Gyroscope>();
        }
    }

    void TimedFunction()
    {

        if (Time.time > _nextSpawn)
        {
            _nextSpawn = Time.time + spawnRate;

            whichColorToSpawn = Random.Range(0, trashPrefab.Length);

            Instantiate(trashPrefab[whichColorToSpawn], transform.position, Random.rotation);
            SpawningTrashServerRpc(whichColorToSpawn);
        }

        if (Time.time > _difficultyScale)
        {
            _difficultyScale = Time.time + difficultyIncreaseScale;
            if (spawnRate <= 0.2f)
            {
                spawnRate = 0.2f;
            }
            else
            {
                spawnRate -= Random.Range(minValue, maxValue);
            }
        }
    }

   

    [ServerRpc]
    public void SpawningTrashServerRpc(int index)
    {
        Instantiate(trashPrefab[index], transform.position, Random.rotation);
    }

    [ServerRpc]
    public void ChangeBalloonServerRpc()
    {
        lobbyCharacter.SetActive(false);
        baloon.SetActive(true);
    }

    [ServerRpc]
    public void ChangeColorSwitchServerRpc()
    {
        lobbyCharacter.SetActive(false);
        spawner.SetActive(true);
    }
}
