using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arduino_Arm_Controller : MonoBehaviour
{
   private SerialCOM sc;

   #region SetUp Variables
        public int BaseRotationRate = 1;
        public int baseYRotation = 80;
        public int BaseNumber;

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

   void _ArmMovement()
   {
        baseYRotation = BaseRotationRate * BaseNumber;
        robotBase.localEulerAngles = new Vector3(robotBase.localEulerAngles.x, baseYRotation, robotBase.localEulerAngles.z);
   }
}
