using TMPro;
using UnityEditor.ShaderGraph;
using UnityEngine;

public class ColorCheck : MonoBehaviour
{
    private Gyroscope _colorSelection;

    public TMP_Text text;

    private int _score;
    // Start is called before the first frame update
    void Start()
    {
        _colorSelection = GetComponent<Gyroscope>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        string _trashColor = collision.gameObject.tag;
        if (collision.gameObject.CompareTag("Red") && _colorSelection.colors[0].activeSelf)
        {
            Debug.Log("RED");
        }

        switch (_trashColor)
        {
            case "Red":
                if (_colorSelection.colors[0].activeSelf)
                {
                    _score++;
                    text.text = _score.ToString();
                }
                break;
            case "Yellow":
                if (_colorSelection.colors[1].activeSelf)
                {
                    _score++;
                    text.text = _score.ToString();
                }
                break;
            case "Blue":
                if (_colorSelection.colors[2].activeSelf)
                {
                    _score++;
                    text.text = _score.ToString();
                }
                break;
            case "Green":
                if (_colorSelection.colors[3].activeSelf)
                {
                    _score++;
                    text.text = _score.ToString();
                }
                break;
        }
        Destroy(collision.gameObject);
    }
}
