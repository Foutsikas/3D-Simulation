using UnityEngine;

public class Test : MonoBehaviour
{
    public SerialCOM sc;
    private int S1, S2, S3, S4;

    #region SetUp Component Variables
        #region Base Variables
            private Vector3 baseStartingPoint;
            private Vector3 baseEndingPoint;
        #endregion

        #region UpperJoint
            private Vector3 upperJointStartingPoint;
            private Vector3 upperJointEndingPoint;
        #endregion

        #region Lower Joint
            private Vector3 lowerJointStartingPoint;
            private Vector3 lowerJointEndingPoint;
        #endregion

        #region Claw
            //Left Pincher
            private Vector3 clawStartingPointLeft;
            private Vector3 clawEndingPointLeft;

            //Right Pincher
            private Vector3 clawStartingPointRight;
            private Vector3 clawEndingPointRight;
        #endregion
    #endregion

    #region Robot Components' Transforms
    // These slots are where you will plug in the appropriate arm parts into the inspector.
        public Transform robotBase;
        public Transform UpperJoint;
        public Transform LowerJoint;
        public Transform ClawPincherLeft;
        public Transform ClawPincherRight;
    #endregion

    public bool rotate;
    private float lerpTime = 1.5f;

    void Start()
    {
        // endQuaternion = new Quaternion();
    }

    void Update()
    {
        StartingPointInit();
        valueAssignment();
        movement();
    }

    void valueAssignment()
    {
        S1 = sc.S1;
        S2 = sc.S2;
        S3 = sc.S3;
        S4 = sc.S4;
    }

    void StartingPointInit()
    {
        baseStartingPoint = new Vector3 (robotBase.transform.rotation.x, robotBase.transform.rotation.y, robotBase.transform.rotation.z);
        upperJointStartingPoint = new Vector3 (UpperJoint.transform.rotation.x, UpperJoint.transform.rotation.y, UpperJoint.transform.rotation.z);
        lowerJointStartingPoint = new Vector3 (LowerJoint.transform.rotation.x, LowerJoint.transform.rotation.y, LowerJoint.transform.rotation.z);
        clawStartingPointLeft = new Vector3 (ClawPincherLeft.transform.rotation.x, ClawPincherLeft.transform.rotation.y, ClawPincherLeft.transform.rotation.z);
        clawStartingPointRight = new Vector3 (ClawPincherRight.transform.rotation.x, ClawPincherRight.transform.rotation.y, ClawPincherRight.transform.rotation.z);
    }

    void movement()
    {
        #region Base
            baseEndingPoint = new Vector3(robotBase.transform.localRotation.x, S1, robotBase.transform.localRotation.z);
            if (rotate)
                robotBase.transform.localRotation = Quaternion.Slerp(robotBase.transform.localRotation, Quaternion.Euler(baseEndingPoint), Time.deltaTime * lerpTime);
        #endregion

        #region Upper Joint
            upperJointEndingPoint = new Vector3(S2, UpperJoint.transform.localRotation.y, UpperJoint.transform.localRotation.z);
            if (rotate)
                UpperJoint.transform.localRotation = Quaternion.Slerp(UpperJoint.transform.localRotation, Quaternion.Euler(upperJointEndingPoint), Time.deltaTime * lerpTime);
        #endregion

        #region Lower Joint
            lowerJointEndingPoint = new Vector3(S3 - 30, LowerJoint.transform.localRotation.y, LowerJoint.transform.localRotation.z);
            if (rotate)
                LowerJoint.transform.localRotation = Quaternion.Slerp(LowerJoint.transform.localRotation, Quaternion.Euler(lowerJointEndingPoint), Time.deltaTime * lerpTime);
        #endregion

        #region Claw Pinchers
            clawEndingPointLeft = new Vector3(ClawPincherLeft.transform.localRotation.x, S4, ClawPincherLeft.transform.localRotation.z);
            if (rotate)
                ClawPincherLeft.transform.localRotation = Quaternion.Slerp(ClawPincherLeft.transform.localRotation, Quaternion.Euler(clawEndingPointLeft), Time.deltaTime * lerpTime);

            clawEndingPointRight = new Vector3(ClawPincherRight.transform.localRotation.x, S4, ClawPincherRight.transform.localRotation.z);
            if (rotate)
                ClawPincherRight.transform.localRotation = Quaternion.Slerp(ClawPincherRight.transform.localRotation, Quaternion.Euler(-clawEndingPointRight), Time.deltaTime * lerpTime);
        #endregion
    }
}