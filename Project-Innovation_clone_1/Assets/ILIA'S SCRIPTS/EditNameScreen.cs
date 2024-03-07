using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EditNameScreen : MonoBehaviour
{
    public static EditNameScreen Instance { get; private set; }

    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button continueButton;

    void Awake()
    {
        Instance = this;
        Hide();

        continueButton.onClick.AddListener(() => {

            LobbyManager.Instance.Authenticate(inputField.text);

            LobbyClientScreen.Instance.Show();
            Hide();
        });
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
