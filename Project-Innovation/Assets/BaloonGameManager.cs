using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class BaloonGameManager : NetworkBehaviour
{
    public static BaloonGameManager Instance { get; private set; }

    [SerializeField] GameObject serverCanvas;
    [SerializeField] GameObject clientCanvas;

    [SerializeField] Animator endScreenAnimator;

    [SerializeField] string winnerName;

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
        }
        else
        {
            serverCanvas.SetActive(false);
        }
    }

    public void ToRunRpcFromPlayer(string _playerName)
    {
        
        SetTheWinnerServerRpc(_playerName);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetTheWinnerServerRpc(string _playerName)
    {
        Debug.LogError("CALLED FROM CLIENT");

        if (winnerName == "")
        {
            Debug.LogError("WINNER NAME != NULL");

            winnerName = _playerName;
            endScreenAnimator.SetBool("GoDown", true);

            winnerText.text = "The winner is: " + winnerName;
        }
    }
}
