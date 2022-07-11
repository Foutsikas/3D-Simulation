using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arduino_Arm_Controller : MonoBehaviour
{
     private SerialCOM sc;

     #region SetUp Variables
          #region Base Variables
               public float BaseRotationRate = 1.0f;
               public float baseYRotation = 0;
               public float BaseValue;
          #endregion

          #region Lower Joint Variables
               public float LowerJointRotationRate = 1.0f;
               public float LowerJointXRotation = 60.0f;
               public float LowerJointValue;
          #endregion

          #region Upper Joint Variables
               public float UpperJointRotationRate = 1.0f;
               public float UpperJointXRotation = 130.0f;
               public float UpperJointValue;
          #endregion

          #region Claw Variables
               public float clawRotationRate = 1.0f;
               private float clawYRotationLeft = -180.0f;
               private float clawYRotationRight = -180.0f;

               public float ClawValue;
          #endregion
     #endregion

     #region SetUp Parts
     // These slots are where you will plug in the appropriate arm parts into the inspector.
          public Transform robotBase;
          public Transform UpperJoint;
          public Transform LowerJoint;
          public Transform ClawPincherLeft;
          public Transform ClawPincherRight;
     #endregion


     void Update()
     {
          _ValueAssignment();
          _ArmMovement();
     }


     #region Methods
          void _ValueAssignment()
          {
               //function is: (X - min) / (max - min) * 100. X = Arduino Data Coming in.
               //Max and Min are the servos' Max and Min values in Arduino.
               BaseValue = (sc.S1 - 0)/(180-0)*100;
               LowerJointValue = (sc.S2 - 0)/(180-0)*100;
               UpperJointValue = (sc.S3 - 34)/(180-34)*100;
               ClawValue = (sc.S4 - 0)/(116-0)*100;
          }

          void _ArmMovement()
          {
               #region Base Calculations
                    baseYRotation = BaseRotationRate * BaseValue;
                    robotBase.localEulerAngles = new Vector3(robotBase.localEulerAngles.x, baseYRotation, robotBase.localEulerAngles.z);
               #endregion

               #region Upper Arm Movement
                    //rotating our upper arm of the robot here around the X axis and multiplying
                    //the rotation by the slider's value and the turn rate for the upper arm.
                    UpperJointXRotation += UpperJointRotationRate * UpperJointValue;
                    UpperJoint.localEulerAngles = new Vector3(UpperJointXRotation, UpperJoint.localEulerAngles.y, UpperJoint.localEulerAngles.z);
               #endregion

               #region Lower Arm Movement
                    //rotating our lower arm of the robot here around the X axis and multiplying
                    //the rotation by the slider's value and the turn rate for the lower arm.
                    LowerJointXRotation += LowerJointRotationRate * LowerJointValue;
                    LowerJoint.localEulerAngles = new Vector3(LowerJointXRotation, LowerJoint.localEulerAngles.y, LowerJoint.localEulerAngles.z);
               #endregion

               #region Claw Close/Open
                    //Left Pincher
                    clawYRotationLeft += clawRotationRate * ClawValue;
                    ClawPincherLeft.localEulerAngles = new Vector3(ClawPincherLeft.localEulerAngles.x, clawYRotationLeft, ClawPincherLeft.localEulerAngles.z);
                    //Right Pincher
                    clawYRotationRight += clawRotationRate * ClawValue;
                    ClawPincherRight.localEulerAngles = new Vector3(ClawPincherRight.localEulerAngles.x, -clawYRotationRight, ClawPincherRight.localEulerAngles.z);
               #endregion
          }
     #endregion
}