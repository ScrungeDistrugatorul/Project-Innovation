using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class TutorialScreen : NetworkBehaviour
{
    public static TutorialScreen Instance { get; private set; }

    [SerializeField] private Button continueButtonBalloon;
    [SerializeField] private Button continueButtonColorSwitch;

    public bool readyToPlay = false;

    void Awake()
    {
        Instance = this;

        if (continueButtonColorSwitch != null)
        {
            continueButtonColorSwitch.onClick.AddListener(() =>
            {

                SetTheReadinessClientRpc(true);

                ColorSwitchManager.Instance.startCountDown = true;
                StartCountDownClientRpc();

                Hide();
            });
        }

        if (continueButtonBalloon != null)
        {
            continueButtonBalloon.onClick.AddListener(() =>
        {

            SetTheReadinessClientRpc(true);

            Hide();
        });
        }
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    [ClientRpc]
    public void StartCountDownClientRpc()
    {
        ColorSwitchManager.Instance.startCountDown = true;
    }

    [ClientRpc]
    public void SetTheReadinessClientRpc(bool value)
    {
        readyToPlay = value;
    }
}
