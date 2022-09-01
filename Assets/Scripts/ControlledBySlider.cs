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
            public float baseSliderValue = 0.0f;

            // slider value for upper & lower arm that goes from -1 to 1.
            public float upperArmSliderValue = 0.0f;
            public float lowerArmSliderValue = 0.0f;

            // slider value for the claw component from -1 to 1.
            public float clawSliderValue = 0.0f;

            // slider value for the left and right pinchers from -1 to 1.
            public float cOpClSliderValue = 0.0f;
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

            #region Base Variables
                public float baseTurnRate = 1.0f;
                private float baseYRotation = 80.0f;
                public float baseYRotMin = -45.0f;
                public float baseYRotMax = 45.0f;
            #endregion

            #region Upper Arm Variables
                public float upperArmTurnRate = 1.0f;
                private float upperArmXRotation = 60.0f;
                public float upperArmXRotMin = -45.0f;
                public float upperArmXRotMax = 45.0f;
            #endregion

            #region Lower Arm Variables
                public float lowerArmTurnRate = 1.0f;
                private float lowerArmXRotation = 130.0f;
                public float lowerArmXRotMin = -45.0f;
                public float lowerArmXRotMax = 45.0f;
            #endregion

            #region Claw Variables
                public float clawPartTurnRate = 1.0f;
                private float clawXRotator = -25.0f;
                public float clawXRotMin = -70.0f;
                public float clawXRotMax = 60.0f;

                #region Claw's Pinchers Variables
                    public float clawOpClTurnRate = 1.0f;

                    //Claw Pincher Left
                    private float cOpClYRotLeft = 0.0f;
                    public float cOpClYRotMinLeft = -45.0f;
                    public float cOpClYRotMaxLeft = 0f;

                    //Claw Pincher Right
                    private float cOpClYRotRight = 0.0f;
                    public float cOpClYRotMinRight = 0f;
                    public float cOpClYRotMaxRight = 45.0f;
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
                baseYRotation += baseSliderValue * baseTurnRate;
                baseYRotation = Mathf.Clamp(baseYRotation, baseYRotMin, baseYRotMax);
                robotBase.localEulerAngles = new Vector3(robotBase.localEulerAngles.x, baseYRotation, robotBase.localEulerAngles.z);
            #endregion

            #region Upper Arm Movement
            //rotating our upper arm of the robot here around the X axis and multiplying
            //the rotation by the slider's value and the turn rate for the upper arm.
                upperArmXRotation += upperArmSliderValue * upperArmTurnRate;
                upperArmXRotation = Mathf.Clamp(upperArmXRotation, upperArmXRotMin, upperArmXRotMax);
                upperArm.localEulerAngles = new Vector3(upperArmXRotation, upperArm.localEulerAngles.y, upperArm.localEulerAngles.z);
            #endregion

            #region Lower Arm Movement
            //rotating our lower arm of the robot here around the X axis and multiplying
            //the rotation by the slider's value and the turn rate for the lower arm.
                lowerArmXRotation += lowerArmSliderValue * lowerArmTurnRate;
                lowerArmXRotation = Mathf.Clamp(lowerArmXRotation, lowerArmXRotMin, lowerArmXRotMax);
                lowerArm.localEulerAngles = new Vector3(lowerArmXRotation, lowerArm.localEulerAngles.y, lowerArm.localEulerAngles.z);
            #endregion

            #region Claw Movement
            //rotating our claw of the robot here around the X axis and multiplying
            //the rotation by the slider's value and the turn rate for the claw component.
                clawXRotator += clawSliderValue * clawPartTurnRate;
                clawXRotator = Mathf.Clamp(clawXRotator, clawXRotMin, clawXRotMax);
                clawPart.localEulerAngles = new Vector3(clawXRotator, clawPart.localEulerAngles.y, clawPart.localEulerAngles.z);

                #region Claw's Pincher Movement
                    //rotating our left claw pincher of the robot on the Y axis and multiplying
                    //the rotation by the slider's value and the turn rate for the claw pincher.
                    cOpClYRotLeft += cOpClSliderValue * clawOpClTurnRate;
                    cOpClYRotLeft = Mathf.Clamp(cOpClYRotLeft, cOpClYRotMinLeft, cOpClYRotMaxLeft);
                    cOpenCloseLeft.localEulerAngles = new Vector3(cOpenCloseLeft.localEulerAngles.x, cOpClYRotLeft, cOpenCloseLeft.localEulerAngles.z);

                    //rotating our right claw pincher of the robot on the Y axis and multiplying
                    //the rotation by the slider's value and the turn rate for the claw pincher.
                    cOpClYRotRight += cOpClSliderValue * clawOpClTurnRate;
                    cOpClYRotRight = Mathf.Clamp(cOpClYRotRight, cOpClYRotMinRight, cOpClYRotMaxRight);
                    cOpenCloseRight.localEulerAngles = new Vector3(cOpenCloseRight.localEulerAngles.x, cOpClYRotRight, cOpenCloseRight.localEulerAngles.z);
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