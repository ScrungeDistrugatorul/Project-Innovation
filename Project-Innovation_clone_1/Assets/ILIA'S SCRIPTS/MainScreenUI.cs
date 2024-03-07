using UnityEngine;
using UnityEngine.UI;

public class MainScreenUI : MonoBehaviour
{
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button joinLobbyButton;

    void Start()
    {
        //if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXPlayer)
        //{
        //    Debug.Log("PC");
        //}

        //if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
        //{
        //    LobbyManager.Instance.Authenticate("Default");
        //}



        createLobbyButton.onClick.AddListener(() =>
        {
            LobbyManager.Instance.AuthenticateAsServer("Server");

            LobbyHostScreen.Instance.Show();

            Hide();
        });

        joinLobbyButton.onClick.AddListener(() =>
        {
            EditNameScreen.Instance.Show();
            Hide();
        });
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }


}

