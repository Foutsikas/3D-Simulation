using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arduino_Arm_Controller : MonoBehaviour
{
   private SerialCOM sc;

   #region SetUp Variables
        public int BaseRotationRate = 1;
        public int baseYRotation = 80;
        public int BaseValue;
        public int LowerArmValue;
        public int UpperArmValue;
        public int ClawValue;

   #endregion

   #region SetUp Parts
        // These slots are where you will plug in the appropriate arm parts into the inspector.
        public Transform robotBase;
        public Transform upperArm;
        public Transform lowerArm;
        public Transform clawPart;
        public Transform cOpenCloseLeft;
        public Transform cOpenCloseRight;
    #endregion

    void _ValueAssignment()
    {
        BaseValue = sc.S1;
        LowerArmValue = sc.S2;
        UpperArmValue = sc.S3;
        ClawValue = sc.S4;
    }

    void _ArmMovement()
   {
        baseYRotation = BaseRotationRate * BaseValue;
        robotBase.localEulerAngles = new Vector3(robotBase.localEulerAngles.x, baseYRotation, robotBase.localEulerAngles.z);
   }
}
