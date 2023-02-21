using System.Collections;
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
    public float lerpTime = 20f;
    bool m_isLerping;
    public bool isLerping
    {
        get { return m_isLerping; }
    }

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

        #region Robot Movement
            //Rotates the base of the robot according to the slider value.
            public void RotateBaseRotator(float value)
            {
                //baseZRotation = value * turnRate * Time.deltaTime;
                //baseZRotation = Mathf.Clamp(baseZRotation, baseZRotMin, baseZRotMax);
                float remapedBaseValue = math.remap(-80,80,45,135,value);
                Base.localEulerAngles = new Vector3(Base.localEulerAngles.x, Base.localEulerAngles.y, -value);
                SerialCOMSliders.Instance.baseValue = remapedBaseValue;
                SerialCOMSliders.Instance.WriteSerial();
            }

            //Rotates the Upper Arm of the robot according to the slider value.
            public void RotateUpperArmRotator(float value)
            {
                float remapedUpperArmValue = math.remap(0,-70,0,80,value);
                UpperArm.localEulerAngles = new Vector3(value, UpperArm.localEulerAngles.y, UpperArm.localEulerAngles.z);
                SerialCOMSliders.Instance.upperArmValue = remapedUpperArmValue;
                SerialCOMSliders.Instance.WriteSerial();
            }

            //Rotates the Lower Arm of the robot according to the slider value.
            public void RotateLowerArmRotator(float value)
            {
                float remapedLowerArmValue = math.remap(-80,30,35,145,value);
                LowerArm.localEulerAngles = new Vector3(value, LowerArm.localEulerAngles.y, LowerArm.localEulerAngles.z);
                SerialCOMSliders.Instance.lowerArmValue = remapedLowerArmValue;
                SerialCOMSliders.Instance.WriteSerial();
            }

            //Opens and closes the pinchers
            public void RotatePincherArmRotator(float value)
            {
                float remapedClawValue = math.remap(0,50,0,115,value);
                Claw_Left.localEulerAngles = new Vector3(Claw_Left.localEulerAngles.x, Claw_Left.localEulerAngles.y, -value);
                Claw_Right.localEulerAngles = new Vector3(Claw_Right.localEulerAngles.x, Claw_Right.localEulerAngles.y, value);
                SerialCOMSliders.Instance.clawValue = remapedClawValue;
                SerialCOMSliders.Instance.WriteSerial();
            }
        #endregion

        public void SaveServoPosition1()
        {
            SaveServoPositions(1);
        }

        public void LoadServoPosition1()
        {
            LoadServoPositions(1);
        }

        public void SaveServoPosition2()
        {
            SaveServoPositions(2);
        }

        public void LoadServoPosition2()
        {
            LoadServoPositions(2);
        }

        public void SaveServoPosition3()
        {
            SaveServoPositions(3);
        }

        public void LoadServoPosition3()
        {
            LoadServoPositions(3);
        }

        public void SaveServoPositions(int positionIndex)
        {
            PlayerPrefs.SetFloat("Position" + positionIndex + "_BaseSliderValue", Base_Slider.value);
            PlayerPrefs.SetFloat("Position" + positionIndex + "_UpperArmSliderValue", UpperArm_Slider.value);
            PlayerPrefs.SetFloat("Position" + positionIndex + "_LowerArmSliderValue", LowerArm_Slider.value);
            PlayerPrefs.SetFloat("Position" + positionIndex + "_ClawSliderValue", Claw_Slider.value);

            PlayerPrefs.SetFloat("Base_Rotation" + positionIndex, Base_Rotation);
            PlayerPrefs.SetFloat("UpperArm_Rotation" + positionIndex, UpperArm_Rotation);
            PlayerPrefs.SetFloat("LowerArm_Rotation" + positionIndex, LowerArm_Rotation);
            PlayerPrefs.SetFloat("Claw_LeftRotation" + positionIndex, Claw_LeftRotation);
            PlayerPrefs.SetFloat("Claw_RightRotation" + positionIndex, Claw_RightRotation);

            PlayerPrefs.Save();
        }

        public void LoadServoPositions(int positionIndex)
        {
            if (m_isLerping)
            {
                return;
            }
            m_isLerping = true;
            if (PlayerPrefs.HasKey("Position" + positionIndex + "_BaseSliderValue"))
            {
                // Load the saved slider values from PlayerPrefs.
                Base_SliderValue = PlayerPrefs.GetFloat("Position" + positionIndex + "_BaseSliderValue");
                UpperArm_SliderValue = PlayerPrefs.GetFloat("Position" + positionIndex + "_UpperArmSliderValue");
                LowerArm_SliderValue = PlayerPrefs.GetFloat("Position" + positionIndex + "_LowerArmSliderValue");
                Claw_SliderValue = PlayerPrefs.GetFloat("Position" + positionIndex + "_ClawSliderValue");

                // Smoothly move the sliders to the saved positions over time.
                StartCoroutine(LerpSliders(Base_SliderValue, UpperArm_SliderValue, LowerArm_SliderValue, Claw_SliderValue));
            }
        }

        private IEnumerator LerpSliders(float baseEndPoint, float upperArmEndPoint, float lowerArmEndPoint, float clawEndPoint)
        {
            float elapsedTime = 0;
            float baseSliderStartValue = Base_Slider.value;
            float upperArmSliderStartValue = UpperArm_Slider.value;
            float lowerArmSliderStartValue = LowerArm_Slider.value;
            float clawSliderStartValue = Claw_Slider.value;

            while (elapsedTime < lerpTime)
            {
                Base_Slider.value = Mathf.Lerp(baseSliderStartValue, baseEndPoint, elapsedTime / lerpTime);
                UpperArm_Slider.value = Mathf.Lerp(upperArmSliderStartValue, upperArmEndPoint, elapsedTime / lerpTime);
                LowerArm_Slider.value = Mathf.Lerp(lowerArmSliderStartValue, lowerArmEndPoint, elapsedTime / lerpTime);
                Claw_Slider.value = Mathf.Lerp(clawSliderStartValue, clawEndPoint, elapsedTime / lerpTime);

                elapsedTime += Time.deltaTime;
                yield return null;
            }
            m_isLerping = false;
        }

        //Resets the Robot on the default position.
        public void ResetTransformRotation()
        {
            StartCoroutine(LerpSliders(SerialCOMSliders.Instance.baseValue,
                                        SerialCOMSliders.Instance.upperArmValue,
                                        SerialCOMSliders.Instance.lowerArmValue,
                                        SerialCOMSliders.Instance.clawValue));
        }
    #endregion
}