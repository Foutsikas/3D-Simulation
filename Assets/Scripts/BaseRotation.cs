using UnityEngine;

public class BaseRotation : MonoBehaviour
{
    public SerialCOM sc;
    private float S1;
    private Quaternion endPosition;
    private Quaternion startPosition;
    private float desiredDuration = 2.5f;
    private float elapsedTime;

    void Start()
    {
        startPosition = transform.rotation;
        S1 = sc.S1;
        endPosition = new Quaternion(0, S1, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        float percentageComplete = elapsedTime / desiredDuration;

        transform.rotation = Quaternion.Lerp (startPosition, endPosition, percentageComplete);
    }
}
