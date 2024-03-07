using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;
using FMOD.Studio;
public class TriggerTeleport : MonoBehaviour
{
    private GameManager _gameManager;
    private int _playerCount;
    private float _timeCountdown;
    private TMP_Text _floatingText;
    public int sceneToSwitch;
    public float originalTime = 5f;
    private EventInstance countdownLobbyEventInstance;
    private bool isSoundPlaying = false;


    private void Awake()
    {
        _floatingText = GetComponentInChildren<TMP_Text>();
        _timeCountdown = originalTime;
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        if (_gameManager != null)
        {
            Debug.Log("Found GM!");
        }
    }

    private void Start()
    {
        UpdateText(0);
    }

    private void Update()
    {
        if (_playerCount == _gameManager.players.Length)
        {
            _timeCountdown -= Time.deltaTime;
            UpdateText(1);
            
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
            if (!isSoundPlaying)
            {
                countdownLobbyEventInstance = FMODUnity.RuntimeManager.CreateInstance(FMODEvents.instance.CountdownLobby);
                countdownLobbyEventInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(this.transform.position));
                countdownLobbyEventInstance.start();
                isSoundPlaying = true;
            }
            _playerCount++;
            
            UpdateText(0);
            Debug.Log(_playerCount);
        }
    }

    private void OnTriggerExit(Collider other)
    {
      
        if (other.CompareTag("Player"))
        {
            _playerCount--;
            Debug.Log(_playerCount);
            if (countdownLobbyEventInstance.isValid())
            {
                countdownLobbyEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                countdownLobbyEventInstance.release(); // Release the EventInstance
            }
            _timeCountdown = originalTime;
            UpdateText(0);

            isSoundPlaying = false;

        }
    }

    void SceneSwitch()
    {
        SceneManager.LoadScene(sceneToSwitch);
    }

    void UpdateText(int type)  // UpdateText(0) for players, UpdateText(1) for countdown
    {
        switch (type)
        {
            case 0: 
                _floatingText.text = "Players: " + _playerCount + "/" + _gameManager.players.Length;
                break;
            case 1:
                _floatingText.text = _gameManager.ShowTimer(_timeCountdown) + "...";
                break;
        }
    }
}
