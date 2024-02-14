using UnityEngine;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public GameObject[] players;

    private void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log(players.Length);
    }
}
