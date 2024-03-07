using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyClientScreen : MonoBehaviour
{
    public static LobbyClientScreen Instance { get; private set; }

    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button joinButton;

    void Awake()
    {
        Instance = this;
        Hide();

        joinButton.onClick.AddListener(() => { LobbyManager.Instance.JoinLobbyByCode(inputField.text); });
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
}
