using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arduino_Arm_Controller : MonoBehaviour
{
   private SerialCOM sc;

   #region SetUp Variables
        public float BaseRotationRate = 1.0f;
        public float baseYRotation = 0;
        public float BaseValue;

        public float LowerArmRotationRate = 1.0f;
        public float LowerArmYRotation = 60.0f;
        public float LowerArmValue;

        public float UpperArmRotationRate = 1.0f;
        public float UpperArmYRotation = 130.0f;
        public float UpperArmValue;

        public float clawRotationRate = 1.0f;
        private float clawYRotLeft = -180.0f;
        private float clawYRotRight = -180.0f;

        public float ClawValue;

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
     //function is: (X - min) / (max - min) * 100. X = Arduino Data Coming in.
     //Max and Min are the servos' Max and Min values in Arduino.
     BaseValue = (sc.S1 - 0)/(180-0)*100;
     LowerArmValue = (sc.S2 - 0)/(180-0)*100;
     UpperArmValue = (sc.S3 - 34)/(180-34)*100;
     ClawValue = (sc.S4 - 0)/(116-0)*100;
    }

    void _ArmMovement()
   {
     #region Base Calculations
          baseYRotation = BaseRotationRate * BaseValue;
          robotBase.localEulerAngles = new Vector3(robotBase.localEulerAngles.x, baseYRotation, robotBase.localEulerAngles.z);
     #endregion

     #region Claw Open/Close
          clawYRotLeft += clawRotationRate * ClawValue;
          cOpenCloseLeft.localEulerAngles = new Vector3(cOpenCloseLeft.localEulerAngles.x, clawYRotLeft, cOpenCloseLeft.localEulerAngles.z);

          clawYRotRight += clawRotationRate * ClawValue;
          cOpenCloseLeft.localEulerAngles = new Vector3(cOpenCloseLeft.localEulerAngles.x, clawYRotRight, cOpenCloseLeft.localEulerAngles.z);
     #endregion

     

   }
}