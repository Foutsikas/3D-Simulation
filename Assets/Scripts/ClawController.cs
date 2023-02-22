using UnityEngine;

public class ClawController : MonoBehaviour
{
    public GameObject leftPincher;
    public GameObject rightPincher;
    public GameObject cube;

    private bool isHolding = false;

    void FixedUpdate()
    {
        if (!isHolding)
        {
            // Check if the pinchers are touching the cube
            bool leftTouching = leftPincher.GetComponentInChildren<Collider>().bounds.Intersects(cube.GetComponent<Collider>().bounds);
            bool rightTouching = rightPincher.GetComponentInChildren<Collider>().bounds.Intersects(cube.GetComponent<Collider>().bounds);

            if (leftTouching && rightTouching)
            {
                // Set the cube as a child of the pinchers
                cube.transform.parent = transform;

                // Disable the cube's rigidbody
                Rigidbody cubeRb = cube.GetComponent<Rigidbody>();
                cubeRb.isKinematic = true;

                // Set the holding flag to true
                isHolding = true;
            }
        }
        else
        {
            // Check if the pinchers are still touching the cube
            bool leftTouching = leftPincher.GetComponentInChildren<Collider>().bounds.Intersects(cube.GetComponent<Collider>().bounds);
            bool rightTouching = rightPincher.GetComponentInChildren<Collider>().bounds.Intersects(cube.GetComponent<Collider>().bounds);

            if (!leftTouching || !rightTouching)
            {
                // Remove the cube as a child of the pinchers
                cube.transform.parent = null;

                // Enable the cube's rigidbody
                Rigidbody cubeRb = cube.GetComponent<Rigidbody>();
                cubeRb.isKinematic = false;
                cubeRb.velocity = (leftPincher.transform.position - rightPincher.transform.position) * 0.2f;

                // Set the holding flag to false
                isHolding = false;
            }
        }
    }
}
