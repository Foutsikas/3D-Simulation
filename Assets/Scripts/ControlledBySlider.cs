﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Mathematics;

public class ControlledBySlider : MonoBehaviour
{
    #region Arm Set Up
        public Slider Base_Slider;
        public Slider UpperArm_Slider;
        public Slider LowerArm_Slider;
        public Slider Claw_Slider; //Claw Open Close Slider

        #region Slider Values
            // slider value for base platform.
            private float Base_SliderValue = 0.0f;

            // slider value for Upper & lower arm.
            private float UpperArm_SliderValue = 0.0f;
            private float LowerArm_SliderValue = 0.0f;

            // slider value for the claw pinchers.
            private float Claw_SliderValue = 0.0f;
        #endregion

        #region Parts to slots Assignment
            // These slots are where you will plug in the appropriate arm parts into the inspector.
            public Transform Base;
            public Transform UpperArm;
            public Transform LowerArm;
            public Transform Claw_Left;
            public Transform Claw_Right;
        #endregion

        #region Part Rotations & rotation speed Variables
        // Allow us to have numbers to adjust in the inspector the speed of each part's rotation.

            #region Base Variables
                public float Base_Rotation;
                public float Base_RotMin;
                public float Base_RotMax;
            #endregion

            #region Upper Arm Variables                
                public float UpperArm_Rotation;
                public float UpperArm_RotMin;
                public float UpperArm_RotMax;
            #endregion

            #region Lower Arm Variables
                public float LowerArm_Rotation;
                public float LowerArm_RotMin;
                public float LowerArm_RotMax;
            #endregion

            #region Claw Variables
                //Claw Pincher Left
                public float Claw_LeftRotation;
                public float Claw_LeftRotMin;
                public float Claw_LeftRotMax;

                //Claw Pincher Right
                public float Claw_RightRotation;
                public float Claw_RightRotMin;
                public float Claw_RightRotMax;
            #endregion
        #endregion
    #endregion
    public float lerpTime = 10f;

    private void Start()
    {
        SliderValuesInitilize();
    }

    #region Methods

        //Sets the slider min and max according to the values set through Unity.
        void SliderValuesInitilize()
        {
            Base_Slider.minValue = Base_RotMin;
            Base_Slider.maxValue = Base_RotMax;

            UpperArm_Slider.minValue = UpperArm_RotMin;
            UpperArm_Slider.maxValue = UpperArm_RotMax;

            LowerArm_Slider.minValue = LowerArm_RotMin;
            LowerArm_Slider.maxValue = LowerArm_RotMax;

            Claw_Slider.minValue = Claw_RightRotMin;
            Claw_Slider.maxValue = Claw_RightRotMax;
        }
        void CheckInput()
        {
            Base_SliderValue = Base_Slider.value;
            UpperArm_SliderValue = UpperArm_Slider.value;
            LowerArm_SliderValue = LowerArm_Slider.value;
            Claw_SliderValue = Claw_Slider.value;
        }

        #region Robot Movement
            //Rotates the base of the robot according to the slider value.
            public void RotateBaseRotator(float value)
            {
                //baseZRotation = value * turnRate * Time.deltaTime;
                //baseZRotation = Mathf.Clamp(baseZRotation, baseZRotMin, baseZRotMax);
                float remapedBaseValue = math.remap(-80,80,45,135,value);
                Base.localEulerAngles = new Vector3(Base.localEulerAngles.x, Base.localEulerAngles.y, -value);
                SerialCOMSliders.Instance.baseValue = remapedBaseValue;
                Debug.Log("Base Value: " + remapedBaseValue);
            }

            //Rotates the Upper Arm of the robot according to the slider value.
            public void RotateUpperArmRotator(float value)
            {
                float remapedUpperArmValue = math.remap(0,-70,0,80,value);
                UpperArm.localEulerAngles = new Vector3(value, UpperArm.localEulerAngles.y, UpperArm.localEulerAngles.z);
                SerialCOMSliders.Instance.upperArmValue = remapedUpperArmValue;
            }

            //Rotates the Lower Arm of the robot according to the slider value.
            public void RotateLowerArmRotator(float value)
            {
                float remapedLowerArmValue = math.remap(-80,30,35,145,value);
                LowerArm.localEulerAngles = new Vector3(value, LowerArm.localEulerAngles.y, LowerArm.localEulerAngles.z);
                SerialCOMSliders.Instance.lowerArmValue = remapedLowerArmValue;
            }

            //Opens and closes the pinchers
            public void RotatePincherArmRotator(float value)
            {
                float remapedClawValue = math.remap(0,50,0,115,value);
                Claw_Left.localEulerAngles = new Vector3(Claw_Left.localEulerAngles.x, Claw_Left.localEulerAngles.y, -value);
                Claw_Right.localEulerAngles = new Vector3(Claw_Right.localEulerAngles.x, Claw_Right.localEulerAngles.y, value);
                SerialCOMSliders.Instance.clawValue = remapedClawValue;
            }
        #endregion

        public void ResetTransformRotation()
            {
                StartCoroutine(LerpRotationToZero(Base));
                StartCoroutine(LerpRotationToZero(UpperArm));
                StartCoroutine(LerpRotationToZero(LowerArm));
                StartCoroutine(LerpRotationToZero(Claw_Left));
                StartCoroutine(LerpRotationToZero(Claw_Right));
            }

        IEnumerator LerpRotationToZero(Transform transform)
        {
            Quaternion initialRotation = transform.localRotation;
            Quaternion targetRotation = Quaternion.identity;
            float timeElapsed = 0f;

            while (timeElapsed < lerpTime)
            {
                timeElapsed += Time.deltaTime;
                transform.localRotation = Quaternion.Lerp(initialRotation, targetRotation, timeElapsed / lerpTime);
                yield return null;
            }
        }
    #endregion
}