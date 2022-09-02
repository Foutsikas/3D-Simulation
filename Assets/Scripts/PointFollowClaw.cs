using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointFollowClaw : MonoBehaviour
{
    GameObject ball;
    GameObject claw;

    Vector3 position;

    void Start()
    {
        ball = GameObject.Find("ClawFollowPivot");
        claw = GameObject.Find("Claw Pivot");
    }

    void Update()
    {
        position = claw.transform.position;
        
        Vector3 targetPosition = new Vector3 (claw.transform.position.x, claw.transform.position.y, claw.transform.position.z + 3.5f);
        transform.position = targetPosition;
    }
}
