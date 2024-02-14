using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class TriggerTeleport : MonoBehaviour
{
    private GameManager _gameManager;
    private int _playerCount;
    public int sceneToSwitch;
    public float timeCountdown = 5f;
    private void Awake()
    {
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        if (_gameManager != null)
        {
            Debug.Log("Found GM!");
        }
    }

    private void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerCount++;
            if (_playerCount == _gameManager.players.Length)
            {
                Timer();
            }
            Debug.Log(_playerCount);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerCount--;
            Debug.Log(_playerCount);
        }
    }

    void SceneSwitch()
    {
        SceneManager.LoadScene(sceneToSwitch);
    }

    void Timer()
    {
        
    }
}
