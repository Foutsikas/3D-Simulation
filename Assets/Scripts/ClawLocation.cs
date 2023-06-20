using TMPro;
using UnityEngine;

public class ClawLocation : MonoBehaviour
{
    public Transform locationOrb;
    public TMP_Text textField;

    void Update()
    {
        CoordinateUpdate();
    }

    void CoordinateUpdate()
    {
        string x = ((locationOrb.transform.position.x) + 3.767f).ToString("F3");
        string y = ((locationOrb.transform.position.y) - 3.456).ToString("F3");
        string z = ((locationOrb.transform.position.z) + 0.758).ToString("F3");

        textField.text = "Claw Coordinates: \nX: " + x + "\nY: " + y + "\nZ: " + z;
    }
}