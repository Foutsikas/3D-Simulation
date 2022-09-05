using UnityEngine;

public class ClawFollowPoint : MonoBehaviour
{
    GameObject pivot;

    Vector3 pos;

    void Start()
    {
        pivot = GameObject.Find("Claw Pivot");
    }

    void Update()
    {
        pos = pivot.transform.position;
        transform.LookAt(pos, pos + Vector3.forward);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(pos, pos + Vector3.forward);
    }
}
