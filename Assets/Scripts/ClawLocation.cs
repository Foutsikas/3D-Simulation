using System.Collections;
using System.Collections.Generic;
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
        float x = locationOrb.transform.position.x;
        float y = locationOrb.transform.position.y;
        float z = locationOrb.transform.position.z;

        textField.text = "Claw Coordinates: \n" + "X: " + x + "\nY: " + y + "\nZ: " + z;
    }
}