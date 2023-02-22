using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Mathematics;

public class ControlledBySlider : MonoBehaviour
{
    //Slider Variables
    public Slider baseSlider;
    public Slider upperArmSlider;
    public Slider lowerArmSlider;
    public Slider clawSlider;

    //Robot Parts Variables
    [SerializeField] private Transform baseTransform;
    [SerializeField] private Transform upperArmTransform;
    [SerializeField] private Transform lowerArmTransform;
    [SerializeField] private Transform leftClawTransform;
    [SerializeField] private Transform rightClawTransform;

    //Slider Values
    private float Base_SliderValue = 0.0f;
    private float UpperArm_SliderValue = 0.0f;
    private float LowerArm_SliderValue = 0.0f;
    private float Claw_SliderValue = 0.0f;

    [SerializeField] private float baseRotationSpeed;
    [SerializeField] private float baseMinRotation;
    [SerializeField] private float baseMaxRotation;

    [SerializeField] private float upperArmRotationSpeed;
    [SerializeField] private float upperArmMinRotation;
    [SerializeField] private float upperArmMaxRotation;

    [SerializeField] private float lowerArmRotationSpeed;
    [SerializeField] private float lowerArmMinRotation;
    [SerializeField] private float lowerArmMaxRotation;

    [SerializeField] private float leftClawRotationSpeed;
    [SerializeField] private float leftClawMinRotation;
    [SerializeField] private float leftClawMaxRotation;

    [SerializeField] private float rightClawRotationSpeed;
    [SerializeField] private float rightClawMinRotation;
    [SerializeField] private float rightClawMaxRotation;

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

    //Sets the slider min and max according to the values set through Unity.
    void SliderValuesInitilize()
    {
        baseSlider.minValue = baseMinRotation;
        baseSlider.maxValue = baseMaxRotation;

        upperArmSlider.minValue = upperArmMinRotation;
        upperArmSlider.maxValue = upperArmMaxRotation;

        lowerArmSlider.minValue = lowerArmMinRotation;
        lowerArmSlider.maxValue = lowerArmMaxRotation;

        clawSlider.minValue = rightClawMinRotation;
        clawSlider.maxValue = rightClawMaxRotation;
    }

        //Rotates the base of the robot according to the slider value.
        public void RotateBaseRotator(float value)
        {
            //baseZRotation = value * turnRate * Time.deltaTime;
            //baseZRotation = Mathf.Clamp(baseZRotation, baseZRotMin, baseZRotMax);
            float remapedBaseValue = math.remap(-80,80,45,135,value);
            baseTransform.localEulerAngles = new Vector3(baseTransform.localEulerAngles.x, baseTransform.localEulerAngles.y, -value);
            SerialCOMSliders.Instance.baseValue = remapedBaseValue;
            SerialCOMSliders.Instance.WriteSerial();
        }

        //Rotates the Upper Arm of the robot according to the slider value.
        public void RotateUpperArmRotator(float value)
        {
            float remapedUpperArmValue = math.remap(0,-70,0,80,value);
            upperArmTransform.localEulerAngles = new Vector3(value, upperArmTransform.localEulerAngles.y, upperArmTransform.localEulerAngles.z);
            SerialCOMSliders.Instance.upperArmValue = remapedUpperArmValue;
            SerialCOMSliders.Instance.WriteSerial();
        }

        //Rotates the Lower Arm of the robot according to the slider value.
        public void RotateLowerArmRotator(float value)
        {
            float remapedLowerArmValue = math.remap(-80,30,35,145,value);
            lowerArmTransform.localEulerAngles = new Vector3(value, lowerArmTransform.localEulerAngles.y, lowerArmTransform.localEulerAngles.z);
            SerialCOMSliders.Instance.lowerArmValue = remapedLowerArmValue;
            SerialCOMSliders.Instance.WriteSerial();
        }

        //Opens and closes the pinchers
        public void RotatePincherArmRotator(float value)
        {
            float remapedClawValue = math.remap(0,50,0,115,value);
            leftClawTransform.localEulerAngles = new Vector3(
                leftClawTransform.localEulerAngles.x,
                leftClawTransform.localEulerAngles.y,
                -value
            );

            rightClawTransform.localEulerAngles = new Vector3(
                rightClawTransform.localEulerAngles.x,
                rightClawTransform.localEulerAngles.y,
                value
            );

            SerialCOMSliders.Instance.clawValue = remapedClawValue;
            SerialCOMSliders.Instance.WriteSerial();
        }

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
        PlayerPrefs.SetFloat("Position" + positionIndex + "_BaseSliderValue", baseSlider.value);
        PlayerPrefs.SetFloat("Position" + positionIndex + "_UpperArmSliderValue", upperArmSlider.value);
        PlayerPrefs.SetFloat("Position" + positionIndex + "_LowerArmSliderValue", lowerArmSlider.value);
        PlayerPrefs.SetFloat("Position" + positionIndex + "_ClawSliderValue", clawSlider.value);

        PlayerPrefs.SetFloat("Base_Rotation" + positionIndex, baseRotationSpeed);
        PlayerPrefs.SetFloat("UpperArm_Rotation" + positionIndex, upperArmRotationSpeed);
        PlayerPrefs.SetFloat("LowerArm_Rotation" + positionIndex, lowerArmRotationSpeed);
        PlayerPrefs.SetFloat("Claw_LeftRotation" + positionIndex, leftClawRotationSpeed);
        PlayerPrefs.SetFloat("Claw_RightRotation" + positionIndex, rightClawRotationSpeed);

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
        float baseSliderStartValue = baseSlider.value;
        float upperArmSliderStartValue = upperArmSlider.value;
        float lowerArmSliderStartValue = lowerArmSlider.value;
        float clawSliderStartValue = clawSlider.value;

        while (elapsedTime < lerpTime)
        {
            baseSlider.value = Mathf.Lerp(baseSliderStartValue, baseEndPoint, elapsedTime / lerpTime);
            upperArmSlider.value = Mathf.Lerp(upperArmSliderStartValue, upperArmEndPoint, elapsedTime / lerpTime);
            lowerArmSlider.value = Mathf.Lerp(lowerArmSliderStartValue, lowerArmEndPoint, elapsedTime / lerpTime);
            clawSlider.value = Mathf.Lerp(clawSliderStartValue, clawEndPoint, elapsedTime / lerpTime);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        m_isLerping = false;
    }

    //Resets the Robot on the default position.
        public void ResetTransformRotation()
        {
            baseSlider.value = 0;
            upperArmSlider.value = 0;
            lowerArmSlider.value = 0;
            clawSlider.value = 0;

            StartCoroutine(LerpRobotRotation(baseRotationSpeed, 0, baseTransform, lerpTime));
            StartCoroutine(LerpRobotRotation(upperArmRotationSpeed, 0, upperArmTransform, lerpTime));
            StartCoroutine(LerpRobotRotation(lowerArmRotationSpeed, 0, lowerArmTransform, lerpTime));
            StartCoroutine(LerpRobotRotation(leftClawRotationSpeed, 0, leftClawTransform, lerpTime));
            StartCoroutine(LerpRobotRotation(rightClawRotationSpeed, 0, rightClawTransform, lerpTime));
        }

        IEnumerator LerpRobotRotation(float rotation, float target, Transform part, float time)
        {
            float elapsedTime = 0;
            while (elapsedTime < time)
            {
                part.localEulerAngles = new Vector3(part.localEulerAngles.x, part.localEulerAngles.y, Mathf.Lerp(rotation, target, elapsedTime / time));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            part.localEulerAngles = new Vector3(part.localEulerAngles.x, part.localEulerAngles.y, target);
        }
}