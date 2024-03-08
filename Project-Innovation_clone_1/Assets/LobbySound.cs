using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LobbySound : NetworkBehaviour
{

    public AudioSource audio;
    public GameObject audioCondition;
    public AudioClip lobbyLoop;
    private bool _played=false;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!audioCondition.activeSelf && !audio.isPlaying && IsServer && !_played)
        {
            audio.PlayOneShot(audio.clip);
            _played = true;
        }

        if (!audio.isPlaying && _played && lobbyLoop != null)
        {
            audio.PlayOneShot(lobbyLoop);
            audio.loop = true;
        }
    }
}
