using TMPro;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : NetworkBehaviour
{
    public GameObject[] trashPrefab;
    public TMP_Text score;
    
    [Header("Difficulty setting")]
    [Range(0.1f, 10.0f)] public float spawnRate;
    [Range(1.0f, 20.0f)]public float difficultyIncreaseScale;
    [Range(0f, 3.0f)] public float minValue;
    [Range(0f, 3.0f)] public float maxValue;
    private float _nextSpawn;
    private float _difficultyScale;

    [HideInInspector]public int playerScore;


    // Update is called once per frame
    private void Awake()
    {
        _difficultyScale = difficultyIncreaseScale;
    }

    void Update()
    {
        if (Time.time > _nextSpawn)
        {
            _nextSpawn = Time.time + spawnRate;
            Instantiate(trashPrefab[Random.Range(0,trashPrefab.Length-1)], transform.position, Random.rotation);
        }

        if (Time.time > _difficultyScale)
        {
            _difficultyScale = Time.time + difficultyIncreaseScale;
            if (spawnRate <= 0.2f)
            {
                spawnRate = 0.2f;
            }
            else
            {
                spawnRate -= Random.Range(minValue, maxValue);
            }
        }
    }
}
