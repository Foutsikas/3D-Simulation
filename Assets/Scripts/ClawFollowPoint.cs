using UnityEngine;

public class ClawFollowPoint : MonoBehaviour
{
    GameObject target;
    GameObject pivot;
    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find("ClawFollowPivot");
        pivot = GameObject.Find("Claw Pivot");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPosition = new Vector3(target.transform.localPosition.x,
                                                target.transform.localPosition.y,
                                                target.transform.localPosition.z);
        transform.LookAt(targetPosition);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(pivot.transform.position, pivot.transform.position + Vector3.forward);
    }
}
