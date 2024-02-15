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
    private float _timeCountdown;
    public float originalTime = 5f;
    private void Awake()
    {
        _timeCountdown = originalTime;
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        if (_gameManager != null)
        {
            Debug.Log("Found GM!");
        }
    }

    private void Update()
    {
        if (_playerCount == _gameManager.players.Length)
        {
            _timeCountdown -= Time.deltaTime;
            if (_timeCountdown <= 0)
            {
                SceneSwitch();
            }
            Debug.Log(_timeCountdown);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerCount++;
            Debug.Log(_playerCount);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerCount--;
            Debug.Log(_playerCount);
            _timeCountdown = originalTime;
        }
    }

    void SceneSwitch()
    {
        SceneManager.LoadScene(sceneToSwitch);
    }
}
