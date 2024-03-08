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

    [SerializeField] GameObject buttonObject;
    [SerializeField] TMP_Text text;

    void Awake()
    {
        Instance = this;
        Hide();

        joinButton.onClick.AddListener(() => {

            LobbyManager.Instance.JoinLobbyByCode(inputField.text);

            OnButtonClicked();
        });
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    void OnButtonClicked()
    {
        buttonObject.SetActive(false);
        text.text = "Joined!";
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
}
