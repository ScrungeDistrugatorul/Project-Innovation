using UnityEngine;

public class Gyroscope : MonoBehaviour
{
    public GameObject[] colors;

    // Start is called before the first frame update
    void Start()
    {
        Input.gyro.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(Input.gyro.attitude);
        if (Input.gyro.attitude.x >= 0.2)
        {
            colors[0].SetActive(false);
            colors[1].SetActive(true);
            colors[2].SetActive(false);
            colors[3].SetActive(false);
        }
        else if (Input.gyro.attitude.x <= -0.2)
        {
            colors[2].SetActive(false);
            colors[1].SetActive(false);
            colors[0].SetActive(false);
            colors[3].SetActive(true);
        }
        else if (Input.gyro.attitude.y >= 0.2)
        {
            colors[1].SetActive(false);
            colors[0].SetActive(false);
            colors[2].SetActive(true);
            colors[3].SetActive(false);
        }
        else if (Input.gyro.attitude.y <= -0.2 )
        {
            colors[3].SetActive(false);
            colors[1].SetActive(false);
            colors[2].SetActive(false);
            colors[0].SetActive(true);
        }
        else
        {
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i].SetActive(false);
            }
        }
    }
}
