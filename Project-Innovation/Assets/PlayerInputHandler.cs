using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public GameObject playerPrefab;
    private CharacterMovement _characterMovement;
    private Vector3 _startPos;

    private void Awake()
    {
        Vector3 startPos = gameObject.transform.position;
        if (playerPrefab != null)
        {
            _characterMovement = GameObject.Instantiate(playerPrefab, startPos, transform.rotation)
                .GetComponent<CharacterMovement>();
            transform.parent = _characterMovement.transform;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _characterMovement.OnMove(context);
    }
}
