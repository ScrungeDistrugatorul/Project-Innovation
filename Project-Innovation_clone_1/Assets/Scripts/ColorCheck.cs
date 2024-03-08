using TMPro;
using Unity.Netcode;
using UnityEngine;

public class ColorCheck : NetworkBehaviour
{
    public int score;

    public int colorFromGyro;

    private void OnTriggerEnter(Collider other)
    {

        //if (!IsOwner) { return; }

        if (other.CompareTag("Red"))
        {
            if (colorFromGyro == 0)
            {
                score++;
            }

            Destroy(other.gameObject);
        }

        if (other.CompareTag("Blue"))
        {
            if (colorFromGyro == 1)
            {
                score++;
            }
            Destroy(other.gameObject);
        }

        if (other.CompareTag("Yellow"))
        {
            if (colorFromGyro == 2)
            {
                score++;
            }

            Destroy(other.gameObject);
        }

        if (other.CompareTag("Green"))
        {
            if (colorFromGyro == 3)
            {
                score++;
            }

            Destroy(other.gameObject);
        }
    }

    //[ServerRpc]
    //public void RemoveObjectFromServerServerRpc(GameObject objectToDestroy)
    //{
    //    Destroy(objectToDestroy);
    //    NetworkObjectReference(objectToDestroy)
    //}

    public void SetTheColor(int colorIndex)
    {
        colorFromGyro = colorIndex;
    }

}
