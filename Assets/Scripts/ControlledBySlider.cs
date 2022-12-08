using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlledBySlider : MonoBehaviour
{
    #region Arm Set Up
        public Slider baseSlider;
        public Slider armUpSlider;
        public Slider armLowSlider;
        public Slider clawSlider;
        public Slider cOpClSlider; //Claw Open Close Slider

        #region Slider Values
            // slider value for base platform that goes from -1 to 1.
            private float baseSliderValue = 0.0f;

            // slider value for upper & lower arm that goes from -1 to 1.
            private float upperArmSliderValue = 0.0f;
            private float lowerArmSliderValue = 0.0f;

            // slider value for the claw component from -1 to 1.
            private float clawSliderValue = 0.0f;

            // slider value for the left and right pinchers from -1 to 1.
            private float cOpClSliderValue = 0.0f;
        #endregion

        #region Parts to slots Assignment
            // These slots are where you will plug in the appropriate arm parts into the inspector.
            public Transform robotBase;
            public Transform upperArm;
            public Transform lowerArm;
            public Transform clawPart;
            public Transform cOpenCloseLeft;
            public Transform cOpenCloseRight;
        #endregion

        #region Part Rotations & rotation speed Variables
        // Allow us to have numbers to adjust in the inspector the speed of each part's rotation.

            public float turnRate;
            #region Base Variables
                private float baseZRotation;
                public float baseZRotMin;
                public float baseZRotMax;
            #endregion

            #region Upper Arm Variables                
                private float upperArmXRotation;
                public float upperArmXRotMin;
                public float upperArmXRotMax;
            #endregion

            #region Lower Arm Variables
                private float lowerArmXRotation;
                public float lowerArmXRotMin;
                public float lowerArmXRotMax;
            #endregion

            #region Claw Variables                
                private float clawXRotator;
                public float clawXRotMin;
                public float clawXRotMax;

                #region Claw's Pinchers Variables

                    //Claw Pincher Left
                    private float ClawZRotLeft;
                    public float ClawZRotMinLeft;
                    public float ClawZRotMaxLeft;

                    //Claw Pincher Right
                    private float ClawZRotRight;
                    public float ClawZRotMinRight;
                    public float ClawZRotMaxRight;
                #endregion
            #endregion
        #endregion
    #endregion

    void Start()
    {
        /* Set default values to that we can bring our UI sliders into negative values */
        baseSlider.minValue = -1;
        armUpSlider.minValue = -1;
        armLowSlider.minValue = -1;
        clawSlider.minValue = -1;
        cOpClSlider.minValue = -1;
        baseSlider.maxValue = 1;
        armUpSlider.maxValue = 1;
        armLowSlider.maxValue = 1;
        clawSlider.maxValue = 1;
        cOpClSlider.maxValue = 1;
    }

    void Update()
    {
        CheckInput();
        ProcessMovement();
    }

    #region Methods
        void CheckInput()
        {
            baseSliderValue = baseSlider.value;
            upperArmSliderValue = armUpSlider.value;
            lowerArmSliderValue = armLowSlider.value;
            clawSliderValue = clawSlider.value;
            cOpClSliderValue = cOpClSlider.value;
        }

        void ProcessMovement()
        {
            #region Base Movement
            //rotating our base of the robot here around the Y axis and multiplying
            //the rotation by the slider's value and the turn rate for the base.
                baseZRotation += baseSliderValue * turnRate;
                baseZRotation = Mathf.Clamp(baseZRotation, baseZRotMin, baseZRotMax);
                robotBase.localEulerAngles = new Vector3(robotBase.localEulerAngles.x, robotBase.localEulerAngles.y, baseZRotation);
            #endregion

            #region Upper Arm Movement
            //rotating our upper arm of the robot here around the X axis and multiplying
            //the rotation by the slider's value and the turn rate for the upper arm.
                upperArmXRotation += upperArmSliderValue * turnRate;
                upperArmXRotation = Mathf.Clamp(upperArmXRotation, upperArmXRotMin, upperArmXRotMax);
                upperArm.localEulerAngles = new Vector3(upperArmXRotation, upperArm.localEulerAngles.y, upperArm.localEulerAngles.z);
            #endregion

            #region Lower Arm Movement
            //rotating our lower arm of the robot here around the X axis and multiplying
            //the rotation by the slider's value and the turn rate for the lower arm.
                lowerArmXRotation += lowerArmSliderValue * turnRate;
                lowerArmXRotation = Mathf.Clamp(lowerArmXRotation, lowerArmXRotMin, lowerArmXRotMax);
                lowerArm.localEulerAngles = new Vector3(lowerArmXRotation, lowerArm.localEulerAngles.y, lowerArm.localEulerAngles.z);
            #endregion

            #region Claw Movement
            //rotating our claw of the robot here around the X axis and multiplying
            //the rotation by the slider's value and the turn rate for the claw component.
                clawXRotator += clawSliderValue * turnRate;
                clawXRotator = Mathf.Clamp(clawXRotator, clawXRotMin, clawXRotMax);
                clawPart.localEulerAngles = new Vector3(clawXRotator, clawPart.localEulerAngles.y, clawPart.localEulerAngles.z);

                #region Claw's Pincher Movement
                    //rotating our left claw pincher of the robot on the Y axis and multiplying
                    //the rotation by the slider's value and the turn rate for the claw pincher.
                    ClawZRotLeft += cOpClSliderValue * turnRate;
                    ClawZRotLeft = Mathf.Clamp(ClawZRotLeft, ClawZRotMinLeft, ClawZRotMaxLeft);
                    cOpenCloseLeft.localEulerAngles = new Vector3(cOpenCloseLeft.localEulerAngles.x, cOpenCloseLeft.localEulerAngles.y, -ClawZRotLeft);

                    //rotating our right claw pincher of the robot on the Y axis and multiplying
                    //the rotation by the slider's value and the turn rate for the claw pincher.
                    ClawZRotRight += cOpClSliderValue * turnRate;
                    ClawZRotRight = Mathf.Clamp(ClawZRotRight, ClawZRotMinRight, ClawZRotMaxRight);
                    cOpenCloseRight.localEulerAngles = new Vector3(cOpenCloseRight.localEulerAngles.x, cOpenCloseRight.localEulerAngles.y, ClawZRotRight);
                #endregion
            #endregion
        }

        public void ResetSliders()
        {
            //resets the sliders back to 0 when you lift up on the mouse click down (snapping effect)
            baseSliderValue = 0.0f;
            upperArmSliderValue = 0.0f;
            lowerArmSliderValue = 0.0f;
            clawSliderValue = 0.0f;
            cOpClSliderValue = 0.0f;
            baseSlider.value = 0.0f;
            armUpSlider.value = 0.0f;
            armLowSlider.value = 0.0f;
            clawSlider.value = 0.0f;
            cOpClSlider.value = 0.0f;
        }
    #endregion
}