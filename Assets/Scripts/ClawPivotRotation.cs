using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClawPivotRotation : MonoBehaviour
{
    public Transform UpperJointPivot;
    public Transform ClawPivot;
    private float ZDistance;
    private Vector3 GroundZ = new(0,0,0);
    private Vector3 ClawZ;

    private readonly float lerpTime = 1.5f;

    void Update()
    {
        ClawZ = new(UpperJointPivot.transform.localRotation.x, 0, 0);
        ZDistance = GroundZ.x - ClawZ.x;

        if (ZDistance == 0)
        {
            ClawPivot.transform.localRotation = Quaternion.Slerp(ClawPivot.transform.localRotation,Quaternion.AngleAxis(0, Vector3.left), Time.deltaTime * lerpTime);
        }
        else if(ZDistance < 0)
        {
            ClawPivot.transform.localRotation = Quaternion.Slerp(ClawPivot.transform.localRotation,Quaternion.AngleAxis(50, Vector3.left), Time.deltaTime * lerpTime);
        }
        else
        {
            ClawPivot.transform.localRotation = Quaternion.Slerp(ClawPivot.transform.localRotation,Quaternion.AngleAxis(50, Vector3.right), Time.deltaTime * lerpTime);
        }
        // Debug.Log("Distance: " + ZDistance);
    }
}
