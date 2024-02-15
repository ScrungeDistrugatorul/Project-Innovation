using UnityEngine;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public GameObject[] players;

    private void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log(players.Length);
    }
    
    public string ShowTimer(float time)
    {
    	int intTime = (int) time;
    	int seconds = intTime % 60;
    	string timeText = $"{seconds+1}";
    	return timeText;
    }
}
