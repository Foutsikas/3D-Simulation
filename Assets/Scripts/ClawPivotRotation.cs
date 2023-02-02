using UnityEngine;

public class ClawPivotRotation : MonoBehaviour
{
    public Transform UpperJointPivot;
    public Transform ClawPivot;
    private float ZDistance;
    public Transform GroundZ;// = new Vector3(0, 0, 0);
    private Vector3 ClawZ;

    private readonly float lerpTime = 1.5f;

    void Update()
    {
        CheckRotation();
        // Debug.Log("Distance: " + ZDistance);
    }

    void CheckRotation()
    {
        var x = UpperJointPivot.localEulerAngles.x;
        ClawZ = new Vector3(x, 0, 0);
        ZDistance = GroundZ.position.x - ClawZ.x;
        Debug.Log("ZDistance: " + ZDistance);
        if (Mathf.Abs(ZDistance) < 0.01f)
        {
            ClawPivot.localRotation = Quaternion.Slerp(ClawPivot.localRotation, Quaternion.AngleAxis(0, Vector3.right), Time.deltaTime * lerpTime);
        }
        else if (ZDistance < 0f)
        {
            ClawPivot.localRotation = Quaternion.Slerp(ClawPivot.localRotation, Quaternion.AngleAxis(-50, -Vector3.right), Time.deltaTime * lerpTime);
        }
        else if (ZDistance > 0f)
        {
            ClawPivot.localRotation = Quaternion.Slerp(ClawPivot.localRotation, Quaternion.AngleAxis(50, Vector3.right), Time.deltaTime * lerpTime);
        }
    }
}