using UnityEngine;

public class ControlledByArduino : MonoBehaviour
{
    public SerialCOM sc;
    private int S1, S2, S3, S4;

    #region Robot Components' Vectors
    //These variables are what control the rotations of the robot.
        #region Base Variables
            private Vector3 baseRotation;
        #endregion

        #region UpperJoint
            private Vector3 upperJointRotation;
        #endregion

        #region Lower Joint
            private Vector3 lowerJointRotation;
        #endregion

        #region Claw
            //Left Pincher
            private Vector3 leftClawRotation;

            //Right Pincher
            private Vector3 rightClawRotation;
        #endregion
    #endregion

    #region Robot Components' Transforms
    // These slots are where you will plug in the appropriate arm parts into the inspector.
        public Transform robotBase;
        public Transform UpperJoint;
        public Transform LowerJoint;
        public Transform ClawPincherLeft;
        public Transform ClawPincherRight;
        // public Transform ClawPivot;
    #endregion

    private readonly float lerpTime = 1.5f;

    void Update()
    {
        ValueAssignment();
        Movement();
    }

    void ValueAssignment()
    {
        S1 = sc.S1;
        S2 = sc.S2;
        S3 = sc.S3;
        S4 = sc.S4;
    }

    void Movement()
    {
        #region Base
            baseRotation = new Vector3(robotBase.transform.localRotation.x, robotBase.transform.localRotation.y,
                Mathf.Clamp(-S1 + 80, -80, 80));
            robotBase.transform.localRotation = Quaternion.Slerp(robotBase.transform.localRotation,
                Quaternion.Euler(baseRotation), Time.deltaTime * lerpTime);
        #endregion

        #region Upper Joint
            upperJointRotation = new Vector3(Mathf.Clamp(-S2, -70, 0),
                UpperJoint.transform.localRotation.y, UpperJoint.transform.localRotation.z);

            UpperJoint.transform.localRotation = Quaternion.Slerp(UpperJoint.transform.localRotation,
                Quaternion.Euler(upperJointRotation), Time.deltaTime * lerpTime);
        #endregion

        #region Lower Joint
            lowerJointRotation = new Vector3(Mathf.Clamp(S3 - 80, -46, 46),
                LowerJoint.transform.localRotation.y, LowerJoint.transform.localRotation.z);

            LowerJoint.transform.localRotation = Quaternion.Slerp(LowerJoint.transform.localRotation,
                Quaternion.Euler(lowerJointRotation), Time.deltaTime * lerpTime);
        #endregion

        #region Claw Pinchers
            leftClawRotation = new Vector3(ClawPincherLeft.transform.localRotation.x,
                ClawPincherLeft.transform.localRotation.y, Mathf.Clamp(S4, 0, 50));
            ClawPincherLeft.transform.localRotation = Quaternion.Slerp(ClawPincherLeft.transform.localRotation,
                Quaternion.Euler(-leftClawRotation), Time.deltaTime * lerpTime);

            rightClawRotation = new Vector3(ClawPincherRight.transform.localRotation.x,
                ClawPincherRight.transform.localRotation.y, Mathf.Clamp(S4, 0, 50));
            ClawPincherRight.transform.localRotation = Quaternion.Slerp(ClawPincherRight.transform.localRotation,
                Quaternion.Euler(rightClawRotation), Time.deltaTime * lerpTime);
        #endregion
    }
}