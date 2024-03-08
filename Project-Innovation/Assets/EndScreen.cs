using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class EndScreen : NetworkBehaviour
{

    [SerializeField] private Button continueButtonBalloon;
    [SerializeField] private Button continueButtonColorSwitch;

    [SerializeField] string nameOfScene;

    void Awake()
    {
        if (continueButtonColorSwitch != null)
        {
            continueButtonColorSwitch.onClick.AddListener(() =>
            {
                NetworkManager.SceneManager.LoadScene(nameOfScene, UnityEngine.SceneManagement.LoadSceneMode.Single);
            });
        }

        if (continueButtonBalloon != null)
        {
            continueButtonBalloon.onClick.AddListener(() =>
            {
                NetworkManager.SceneManager.LoadScene(nameOfScene, UnityEngine.SceneManagement.LoadSceneMode.Single);
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
