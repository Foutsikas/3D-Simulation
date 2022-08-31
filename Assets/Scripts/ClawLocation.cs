using UnityEngine;
using TMPro;

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
        string x = locationOrb.transform.position.x.ToString("f3");
        string y = locationOrb.transform.position.y.ToString("f3");
        string z = locationOrb.transform.position.z.ToString("f3");

        textField.text = "Claw Coordinates: \nX: " + x + "\nY: " + y + "\nZ: " + z;
    }
}