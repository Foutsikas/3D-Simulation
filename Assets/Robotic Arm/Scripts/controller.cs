using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class controller : MonoBehaviour
{
    public Slider baseSlider;
    public Slider armUpSlider;
    public Slider armLowSlider;
    public Slider clawSlider;
    public Slider cOpClSlider; //Claw Open Close Slider

    // slider value for base platform that goes from -1 to 1.
    public float baseSliderValue = 0.0f;

    // slider value for upper & lower arm that goes from -1 to 1.
    public float upperArmSliderValue = 0.0f;
    public float lowerArmSliderValue = 0.0f;

    // slider value for the claw component from -1 to 1.
    public float clawSliderValue = 0.0f;

    // slider value for the left and right pinchers from -1 to 1.
    public float cOpClSliderValue = 0.0f;

    // These slots are where you will plug in the appropriate arm parts into the inspector.
    public Transform robotBase;
    public Transform upperArm;
    public Transform lowerArm;
    public Transform clawPart;
    public Transform cOpenCloseLeft;
    public Transform cOpenCloseRight;

    // Allow us to have numbers to adjust in the inspector the speed of each part's rotation.
    public float baseTurnRate = 1.0f;
    public float upperArmTurnRate = 1.0f;
    public float lowerArmTurnRate = 1.0f;
    public float clawPartTurnRate = 1.0f;
    public float clawOpClTurnRate = 1.0f;

    private float baseYRot = 0.0f;
    public float baseYRotMin = -45.0f;
    public float baseYRotMax = 45.0f;

    private float upperArmXRot = 0.0f;
    public float upperArmXRotMin = -45.0f;
    public float upperArmXRotMax = 45.0f;

    private float lowerArmXRot = 0.0f;
    public float lowerArmXRotMin = -45.0f;
    public float lowerArmXRotMax = 45.0f;

    private float clawPartXRot = 0.0f;
    public float clawPartXRotMin = -45.0f;
    public float clawPartXRotMax = 45.0f;

    private float cOpClYRotLeft = 0.0f;
    public float cOpClYRotMinLeft = -45.0f;
    public float cOpClYRotMaxLeft = 45.0f;

    private float cOpClYRotRight = 0.0f;
    public float cOpClYRotMinRight = -45.0f;
    public float cOpClYRotMaxRight = 45.0f;

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
        //rotating our base of the robot here around the Y axis and multiplying
        //the rotation by the slider's value and the turn rate for the base.
        baseYRot += baseSliderValue * baseTurnRate;
        baseYRot = Mathf.Clamp(baseYRot, baseYRotMin, baseYRotMax);
        robotBase.localEulerAngles = new Vector3(robotBase.localEulerAngles.x, baseYRot, robotBase.localEulerAngles.z);

        //rotating our upper arm of the robot here around the X axis and multiplying
        //the rotation by the slider's value and the turn rate for the upper arm.
        upperArmXRot += upperArmSliderValue * upperArmTurnRate;
        upperArmXRot = Mathf.Clamp(upperArmXRot, upperArmXRotMin, upperArmXRotMax);
        upperArm.localEulerAngles = new Vector3(upperArmXRot, upperArm.localEulerAngles.y, upperArm.localEulerAngles.z);

        //rotating our lower arm of the robot here around the X axis and multiplying
        //the rotation by the slider's value and the turn rate for the lower arm.
        lowerArmXRot += lowerArmSliderValue * lowerArmTurnRate;
        lowerArmXRot = Mathf.Clamp(lowerArmXRot, lowerArmXRotMin, lowerArmXRotMax);
        lowerArm.localEulerAngles = new Vector3(lowerArmXRot, lowerArm.localEulerAngles.y, lowerArm.localEulerAngles.z);

        //rotating our claw of the robot here around the X axis and multiplying
        //the rotation by the slider's value and the turn rate for the claw component.
        clawPartXRot += clawSliderValue * clawPartTurnRate;
        clawPartXRot = Mathf.Clamp(clawPartXRot, clawPartXRotMin, clawPartXRotMax);
        clawPart.localEulerAngles = new Vector3(clawPartXRot, clawPart.localEulerAngles.y, clawPart.localEulerAngles.z);


        //rotating our left claw pincher of the robot on the Y axis and multiplying
        //the rotation by the slider's value and the turn rate for the claw pincher.
        cOpClYRotLeft += cOpClSliderValue * clawOpClTurnRate;
        cOpClYRotLeft = Mathf.Clamp(cOpClYRotLeft, cOpClYRotMinLeft, cOpClYRotMaxLeft);
        cOpenCloseLeft.localEulerAngles = new Vector3(cOpenCloseLeft.localEulerAngles.x, cOpClYRotLeft, cOpenCloseLeft.localEulerAngles.z);

        //rotating our right claw pincher of the robot on the Y axis and multiplying
        //the rotation by the slider's value and the turn rate for the claw pincher.
        cOpClYRotRight += cOpClSliderValue * clawOpClTurnRate;
        cOpClYRotRight = Mathf.Clamp(cOpClYRotRight, cOpClYRotMinRight, cOpClYRotMaxRight);
        cOpenCloseRight.localEulerAngles = new Vector3(cOpenCloseRight.localEulerAngles.x, -cOpClYRotRight, cOpenCloseRight.localEulerAngles.z);


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

    void Update()
    {
        CheckInput();
        ProcessMovement();
    }
}
