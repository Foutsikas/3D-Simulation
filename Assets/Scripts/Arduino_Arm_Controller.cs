using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arduino_Arm_Controller : MonoBehaviour
{
     public SerialCOM sc;

     #region SetUp Variables
          #region Base Variables
               public float BaseRotationRate = 2.5f;
               public float baseYRotation;
               private float BaseValue;
          #endregion

          #region Upper Joint Variables
               public float UpperJointRotationRate = 2.0f;
               public float UpperJointXRotation;
               private float UpperJointValue;
          #endregion

          #region Lower Joint Variables
               public float LowerJointRotationRate = 2.0f;
               public float LowerJointXRotation;
               private float LowerJointValue;
          #endregion

          #region Claw Variables
               public float clawRotationRate = 1.5f;
               private float clawYRotationLeft;
               private float clawYRotationRight;

               public float ClawValue;
          #endregion
          private int S1, S2, S3, S4;
     #endregion

     #region SetUp Parts
     // These slots are where you will plug in the appropriate arm parts into the inspector.
          public Transform robotBase;
          public Transform UpperJoint;
          public Transform LowerJoint;
          public Transform ClawPincherLeft;
          public Transform ClawPincherRight;
     #endregion

     void Awake()
     {
          baseYRotation = robotBase.localEulerAngles.y;
          UpperJointXRotation = UpperJoint.localEulerAngles.x;
          LowerJointXRotation = LowerJoint.localEulerAngles.x;
          clawYRotationLeft = -ClawPincherLeft.localEulerAngles.x;
          clawYRotationRight = ClawPincherRight.localEulerAngles.x;
     }

     void Start()
     {
          _ValueAssignment();
     }

     void Update()
     {
          _VariableAssignment();
          _ValueAssignment();
          _ArmMovement();
     }

     #region Methods

          void _VariableAssignment()
          {
               S1 = sc.S1;
               S2 = sc.S2;
               S3 = sc.S3;
               S4 = sc.S4;
          }

          void _ValueAssignment()
          {
               //function is: (X - min) / (max - min) * 100. X = Arduino Data Coming in.
               //Max and Min are the servos' Max and Min values in Arduino.
               BaseValue = S1 + 80;//(S1 - 0)/(180-0)*100
               UpperJointValue = S2;//(S2 - 0)/(180-0)*100;
               LowerJointValue = S3 - 40; //(S3 - 34)/(180-34)*100;
               ClawValue = S4;//(S4 - 0)/(116-0)*100;
          }

          void _ArmMovement()
          {
               #region Base Calculations
                    baseYRotation = BaseRotationRate * BaseValue;
                    robotBase.localEulerAngles = new Vector3(robotBase.localEulerAngles.x, baseYRotation, robotBase.localEulerAngles.z); //* Time.deltaTime;
               #endregion

               #region Upper Arm Movement
                    //rotating our upper arm of the robot here around the X axis and multiplying
                    //the rotation by the slider's value and the turn rate for the upper arm.
                    UpperJointXRotation = UpperJointRotationRate * UpperJointValue;
                    if (UpperJointXRotation > -60 && UpperJointXRotation < 90)
                    {
                         UpperJoint.localEulerAngles = new Vector3(UpperJointXRotation, UpperJoint.localEulerAngles.y, UpperJoint.localEulerAngles.z); //* Time.deltaTime;
                    }

               #endregion

               #region Lower Arm Movement
                    //rotating our lower arm of the robot here around the X axis and multiplying
                    //the rotation by the slider's value and the turn rate for the lower arm.
                    LowerJointXRotation = LowerJointRotationRate * LowerJointValue;
                    if (LowerJointXRotation > -6 && LowerJointXRotation < 70)
                    {
                         LowerJoint.localEulerAngles = new Vector3(LowerJointXRotation, LowerJoint.localEulerAngles.y, LowerJoint.localEulerAngles.z); //* Time.deltaTime;
                    }
               #endregion

               // #region Claw Close/Open
               //      //Left Pincher
               //      clawYRotationLeft = clawRotationRate * ClawValue;
               //      ClawPincherLeft.eulerAngles = new Vector3(ClawPincherLeft.localEulerAngles.x, clawYRotationLeft, ClawPincherLeft.localEulerAngles.z);
               //      //Right Pincher
               //      clawYRotationRight = clawRotationRate * ClawValue;
               //      ClawPincherRight.eulerAngles = new Vector3(ClawPincherRight.localEulerAngles.x, -clawYRotationRight, ClawPincherRight.localEulerAngles.z);
               // #endregion
          }
     #endregion
}