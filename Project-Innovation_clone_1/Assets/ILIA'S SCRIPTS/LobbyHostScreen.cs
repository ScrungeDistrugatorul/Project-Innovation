using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyHostScreen : MonoBehaviour
{
    public static LobbyHostScreen Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI lobbyEntryCode;
    [SerializeField] private Button startButton;

    void Awake()
    {
        Instance = this;
        Hide();

        startButton.onClick.AddListener(() => { LobbyManager.Instance.StartTheGame(); Hide(); });
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);

    }

    public void SetTheLobbyCode(string lobbyCode)
    {
        lobbyEntryCode.text = lobbyCode;
    }

    private void Update()
    {
        lobbyEntryCode.text = LobbyManager.Instance.GetLobbyCode();
    }
}
