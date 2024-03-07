using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class ColorSwitchManager : NetworkBehaviour
{
    public static ColorSwitchManager Instance { get; private set; }

    [SerializeField] GameObject serverCanvas;
    [SerializeField] GameObject clientCanvas;

    [SerializeField] Animator endScreenAnimator;

    [Header("Timer related")]
    public TMP_Text timerText;

    public float currentTime = 7;

    public bool startCountDown = false;

    public bool timeIsUp = false;


    [Header("Players related")]
    [SerializeField] string[] playersNames;
    [SerializeField] int[] scores;

    [Header("End screen related")]
    [SerializeField] TMP_Text winnerText;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
      
        if (IsServer)
        {
            clientCanvas.SetActive(false);

            scores = new int[NetworkManager.ConnectedClients.Count];
            playersNames = new string[NetworkManager.ConnectedClients.Count];
        }
        else
        {
            serverCanvas.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (startCountDown)
        {
            currentTime -= Time.deltaTime;

            
            if (currentTime <= 0)
            {
                timeIsUp = true;

                currentTime = 0;

                endScreenAnimator.SetBool("GoDown", true);

                if (IsServer && IsPlayersArrayFilled())
                {
                    CompareScore();
                }
            }
            else
            {
                UpdateTimerDisplay();
            }
        }



    }

    bool IsPlayersArrayFilled()
    {
        int amountOfFilledPlayers = 0;

        for (int i = 0; i < NetworkManager.ConnectedClients.Count; i++)
        {
            if (playersNames[i] != null)
            {
                amountOfFilledPlayers++;
            }
        }

        if (amountOfFilledPlayers == NetworkManager.ConnectedClients.Count)
        {
            return true;
        }
        else {
            return false;
        }
    }

    void CompareScore()
    {
        int highestScore = 0;

        int winnerIndex = 0;

        for (int i = 0; i < NetworkManager.ConnectedClients.Count; i++)
        {
            
            if (scores[i] >= highestScore)
            {
                highestScore = scores[i];
                winnerIndex = i;
            }
        }

        winnerText.text = playersNames[winnerIndex];

    }

    public void ToRunRpcFromPlayer(int _score, int _id, string _name)
    {
        //if (!IsOwner) { return; }

        Debug.LogError("Score: " + _score + " id: " + _id + " name: " + _name);

        GetTheScoreFromThePlayerServerRpc(_score, _id, _name);
    }

    [ServerRpc(RequireOwnership = false)]
    public void GetTheScoreFromThePlayerServerRpc(int _score, int _id, string _name)
    {
        scores[_id - 1] = _score;
        playersNames[_id - 1] = _name;
    }

    void UpdateTimerDisplay()
    {
        int seconds = Mathf.FloorToInt(currentTime % 60f);

        timerText.text = "Time remaining: " + seconds.ToString();
    }

}
