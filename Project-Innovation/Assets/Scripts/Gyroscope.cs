using Unity.Netcode;
using UnityEngine;
using TMPro;


public class Gyroscope : NetworkBehaviour
{
    public GameObject[] colors;
    public TMP_Text text;

    public int whichColor;

    // Start is called before the first frame update
    void Start()
    {
        Input.gyro.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        text.text = "x: " + Input.gyro.attitude.x + " y; " + Input.gyro.attitude.y;

        //red
        if (Input.gyro.attitude.x >= 0.1 && Input.gyro.attitude.y <= -0.1 || Input.GetKey(KeyCode.W))
        {
            colors[0].SetActive(true);
            colors[1].SetActive(false);
            colors[2].SetActive(false);
            colors[3].SetActive(false);

            whichColor = 0;
        }
        //blue
        else if (Input.gyro.attitude.x <= -0.1 && Input.gyro.attitude.y >= 0.1 || Input.GetKey(KeyCode.S))
        {
            colors[0].SetActive(false);
            colors[1].SetActive(true);
            colors[2].SetActive(false);
            colors[3].SetActive(false);

            whichColor = 1;
        }
        //yellow
        else if (Input.gyro.attitude.x >= 0.1 && Input.gyro.attitude.y >= 0.1 || Input.GetKey(KeyCode.A))
        {
            colors[0].SetActive(false);
            colors[1].SetActive(false);
            colors[2].SetActive(true);
            colors[3].SetActive(false);

            whichColor = 2;
        }
        //green
        else if (Input.gyro.attitude.x <= -0.1 && Input.gyro.attitude.y <= -0.1 || Input.GetKey(KeyCode.D))
        {
            colors[0].SetActive(false);
            colors[1].SetActive(false);
            colors[2].SetActive(false);
            colors[3].SetActive(true);

            whichColor = 3;
        }
        //nothing
        else if (Input.gyro.attitude.x > -0.1 && Input.gyro.attitude.x < 0.1 && Input.gyro.attitude.y > -0.1 && Input.gyro.attitude.y < 0.1)
        {
            colors[0].SetActive(false);
            colors[1].SetActive(false);
            colors[2].SetActive(false);
            colors[3].SetActive(false);

            whichColor = -1;
        }

        else
        {
            colors[0].SetActive(false);
            colors[1].SetActive(false);
            colors[2].SetActive(false);
            colors[3].SetActive(false);

            whichColor = -1;
        }
    }
}
