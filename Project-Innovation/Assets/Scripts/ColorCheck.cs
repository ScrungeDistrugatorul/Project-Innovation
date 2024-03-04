using TMPro;
using Unity.Netcode;
using UnityEngine;

public class ColorCheck : NetworkBehaviour
{
    private Gyroscope _gyros;
    public Spawner spawner;

    private int _score;
    // Start is called before the first frame update
    void Start()
    {
        _gyros = FindObjectOfType<Gyroscope>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        string _trashColor = other.gameObject.tag;

        switch (_trashColor)
            {
                case "Red":
                    if (_gyros.colors[0].activeSelf)
                    {
                        spawner.playerScore++;
                        spawner.score.text = spawner.playerScore.ToString();
                        Debug.Log(spawner.playerScore);
                    }
                    Destroy(other.gameObject);
                    break;
                case "Yellow":
                    if (_gyros.colors[1].activeSelf)
                    {
                        spawner.playerScore++;
                        spawner.score.text = spawner.playerScore.ToString();
                        Debug.Log(spawner.playerScore);
                    }
                    Destroy(other.gameObject);
                    break;
                case "Blue":
                    if (_gyros.colors[2].activeSelf)
                    {
                        spawner.playerScore++;
                        spawner.score.text = spawner.playerScore.ToString();
                        Debug.Log(spawner.playerScore);
                    }
                    Destroy(other.gameObject);
                    break;
                case "Green":
                    if (_gyros.colors[3].activeSelf)
                    {
                        spawner.playerScore++;
                        spawner.score.text = spawner.playerScore.ToString();
                        Debug.Log(spawner.playerScore);
                    }
                    Destroy(other.gameObject);
                    break;
            }
        Destroy(other.gameObject);
    }
}
