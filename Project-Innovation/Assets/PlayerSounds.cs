using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{

    [SerializeField] private FMODUnity.EventReference _footsteps;
    private FMOD.Studio.EventInstance footsteps;

    [SerializeField] private FMODUnity.EventReference _playercollision;
    private FMOD.Studio.EventInstance playercollision;

    private void Awake()
    {
        if (!_footsteps.IsNull)
        {
            footsteps = FMODUnity.RuntimeManager.CreateInstance(_footsteps);
        }
        if (!_playercollision.IsNull)
        {
            playercollision = FMODUnity.RuntimeManager.CreateInstance(_playercollision);
        }
    }

    public void PlayFootsteps()
    {
        if (footsteps.isValid())
        {

            footsteps.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));
            footsteps.start();
        }
    }

    public void PlayPlayercollision()
    {
        if (playercollision.isValid())
        {

            playercollision.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));
            playercollision.start();
        }
        else
        {
            Debug.LogError("Invalid playercollision event instance.");
        }
    }
}
