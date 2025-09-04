using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class ControlledBySlider : MonoBehaviour
{
    // Slider Variables
    public Slider baseSlider;
    public Slider upperArmSlider;
    public Slider lowerArmSlider;
    public Slider clawSlider;

    // Robot Parts Variables
    [SerializeField] private Transform baseTransform;
    [SerializeField] private Transform upperArmTransform;
    [SerializeField] private Transform lowerArmTransform;
    [SerializeField] private Transform leftClawTransform;
    [SerializeField] private Transform rightClawTransform;

    // Slider Values
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
    private bool m_isLerping;
    public bool IsLerping => m_isLerping;

    [Header("Robot Initialization")]
    [SerializeField] private bool useHomePositionOnStart = true;
    [SerializeField] private float homeInitializationTime = 3f;
    [SerializeField] private float baseHomePosition = 0f;
    [SerializeField] private float upperArmHomePosition = -20f;
    [SerializeField] private float lowerArmHomePosition = 10f;
    [SerializeField] private float clawHomePosition = 25f;

    [Header("Connection Monitoring")]
    [SerializeField] private float connectionCheckInterval = 1f;
    private float lastConnectionCheck = 0f;

    [Header("Optimization Settings")]
    [SerializeField] private bool onlySendChangedValues = true;
    [SerializeField] private float valueChangeThreshold = 0.5f; // Minimum change to trigger update

    // State tracking
    private bool isInitialized = false;
    private bool slidersEnabled = false;
    private float lastBaseValue = float.MinValue;
    private float lastUpperArmValue = float.MinValue;
    private float lastLowerArmValue = float.MinValue;
    private float lastClawValue = float.MinValue;

    // UI Feedback
    [Header("UI Feedback")]
    public UnityEngine.UI.Text initializationStatusText; // Optional status display
    public UnityEngine.UI.Button resetButton; // Optional reset button

    private void Start()
    {
        SliderValuesInitialize();
        DisableSliders(); // Prevent user interaction during initialization
        StartCoroutine(InitializationSequence());
    }

    private void Update()
    {
        // Periodically check connection status
        if (Time.time - lastConnectionCheck >= connectionCheckInterval)
        {
            CheckSerialConnection();
            lastConnectionCheck = Time.time;
        }
    }

    // Complete initialization sequence
    private IEnumerator InitializationSequence()
    {
        UpdateInitializationStatus("Waiting for connection...");

        // Step 1: Wait for SerialCOMSliders to be ready
        yield return StartCoroutine(WaitForSerialConnection());

        // Step 2: Initialize robot to home position
        if (useHomePositionOnStart)
        {
            yield return StartCoroutine(InitializeToHomePosition());
        }
        else
        {
            // Alternative: Set sliders to last known positions or safe defaults
            yield return StartCoroutine(InitializeToSafePosition());
        }

        // Step 3: Enable sliders for user control
        EnableSliders();
        isInitialized = true;
        UpdateInitializationStatus("Ready for control!");

        Debug.Log("Robot initialization complete - ready for manual control!");
    }

    // Wait for SerialCOMSliders to be initialized
    private IEnumerator WaitForSerialConnection()
    {
        while (SerialCOMSliders.Instance == null)
        {
            Debug.Log("Waiting for SerialCOMSliders to initialize...");
            yield return new WaitForSeconds(0.5f);
        }

        while (!SerialCOMSliders.Instance.IsConnected())
        {
            Debug.Log("Waiting for serial connection...");
            yield return new WaitForSeconds(0.5f);
        }

        Debug.Log("Serial connection established for sliders!");
    }

    // Smoothly move robot to home position
    private IEnumerator InitializeToHomePosition()
    {
        UpdateInitializationStatus("Moving to home position...");

        // Set target positions but don't enable slider interaction yet
        float elapsedTime = 0;
        float startBase = baseSlider.value;
        float startUpperArm = upperArmSlider.value;
        float startLowerArm = lowerArmSlider.value;
        float startClaw = clawSlider.value;

        while (elapsedTime < homeInitializationTime)
        {
            float t = elapsedTime / homeInitializationTime;

            // Smoothly interpolate to home positions
            SetSliderValueQuietly(baseSlider, Mathf.Lerp(startBase, baseHomePosition, t));
            SetSliderValueQuietly(upperArmSlider, Mathf.Lerp(startUpperArm, upperArmHomePosition, t));
            SetSliderValueQuietly(lowerArmSlider, Mathf.Lerp(startLowerArm, lowerArmHomePosition, t));
            SetSliderValueQuietly(clawSlider, Mathf.Lerp(startClaw, clawHomePosition, t));

            // Update robot transforms
            UpdateTransforms();

            // Send to robot (all values at once since we're initializing)
            SendAllValuesToRobot();

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure final positions
        SetSliderValueQuietly(baseSlider, baseHomePosition);
        SetSliderValueQuietly(upperArmSlider, upperArmHomePosition);
        SetSliderValueQuietly(lowerArmSlider, lowerArmHomePosition);
        SetSliderValueQuietly(clawSlider, clawHomePosition);

        UpdateTransforms();
        SendAllValuesToRobot();

        // Store current values as "last known" values
        StoreCurrentValues();

        yield return new WaitForSeconds(0.5f); // Brief pause
    }

    // Alternative: Initialize to safe/stored positions
    private IEnumerator InitializeToSafePosition()
    {
        UpdateInitializationStatus("Initializing to safe position...");

        // Try to load last known good positions
        if (PlayerPrefs.HasKey("LastKnownBase"))
        {
            SetSliderValueQuietly(baseSlider, PlayerPrefs.GetFloat("LastKnownBase", baseHomePosition));
            SetSliderValueQuietly(upperArmSlider, PlayerPrefs.GetFloat("LastKnownUpperArm", upperArmHomePosition));
            SetSliderValueQuietly(lowerArmSlider, PlayerPrefs.GetFloat("LastKnownLowerArm", lowerArmHomePosition));
            SetSliderValueQuietly(clawSlider, PlayerPrefs.GetFloat("LastKnownClaw", clawHomePosition));
        }
        else
        {
            // Use home positions as fallback
            SetSliderValueQuietly(baseSlider, baseHomePosition);
            SetSliderValueQuietly(upperArmSlider, upperArmHomePosition);
            SetSliderValueQuietly(lowerArmSlider, lowerArmHomePosition);
            SetSliderValueQuietly(clawSlider, clawHomePosition);
        }

        UpdateTransforms();
        SendAllValuesToRobot();
        StoreCurrentValues();

        yield return new WaitForSeconds(1f);
    }

    // Set slider value without triggering OnValueChanged events
    private void SetSliderValueQuietly(Slider slider, float value)
    {
        if (slider == null) return;

        // Temporarily disable the slider to prevent events
        bool wasInteractable = slider.interactable;
        slider.interactable = false;
        slider.value = value;
        slider.interactable = wasInteractable;
    }

    private void UpdateTransforms()
    {
        if (baseTransform != null)
            baseTransform.localEulerAngles = new Vector3(baseTransform.localEulerAngles.x, baseTransform.localEulerAngles.y, -baseSlider.value);

        if (upperArmTransform != null)
            upperArmTransform.localEulerAngles = new Vector3(upperArmSlider.value, upperArmTransform.localEulerAngles.y, upperArmTransform.localEulerAngles.z);

        if (lowerArmTransform != null)
            lowerArmTransform.localEulerAngles = new Vector3(lowerArmSlider.value, lowerArmTransform.localEulerAngles.y, lowerArmTransform.localEulerAngles.z);

        if (leftClawTransform != null)
            leftClawTransform.localEulerAngles = new Vector3(leftClawTransform.localEulerAngles.x, leftClawTransform.localEulerAngles.y, -clawSlider.value);

        if (rightClawTransform != null)
            rightClawTransform.localEulerAngles = new Vector3(rightClawTransform.localEulerAngles.x, rightClawTransform.localEulerAngles.y, clawSlider.value);
    }

    private void SendAllValuesToRobot()
    {
        TryUseSerial(() =>
        {
            // Convert all slider values to robot values
            SerialCOMSliders.Instance.baseValue = math.remap(-80, 80, 45, 135, baseSlider.value);
            SerialCOMSliders.Instance.upperArmValue = math.remap(0, -70, 0, 65, upperArmSlider.value);
            SerialCOMSliders.Instance.lowerArmValue = math.remap(-40, 30, 45, 145, lowerArmSlider.value);
            SerialCOMSliders.Instance.clawValue = math.remap(0, 50, 0, 115, clawSlider.value);
            SerialCOMSliders.Instance.WriteSerial();
        });
    }

    private void StoreCurrentValues()
    {
        lastBaseValue = baseSlider.value;
        lastUpperArmValue = upperArmSlider.value;
        lastLowerArmValue = lowerArmSlider.value;
        lastClawValue = clawSlider.value;
    }

    private void DisableSliders()
    {
        slidersEnabled = false;
        if (baseSlider != null) baseSlider.interactable = false;
        if (upperArmSlider != null) upperArmSlider.interactable = false;
        if (lowerArmSlider != null) lowerArmSlider.interactable = false;
        if (clawSlider != null) clawSlider.interactable = false;
    }

    private void EnableSliders()
    {
        slidersEnabled = true;
        if (baseSlider != null) baseSlider.interactable = true;
        if (upperArmSlider != null) upperArmSlider.interactable = true;
        if (lowerArmSlider != null) lowerArmSlider.interactable = true;
        if (clawSlider != null) clawSlider.interactable = true;
    }

    private void UpdateInitializationStatus(string message)
    {
        if (initializationStatusText != null)
        {
            initializationStatusText.text = message;
        }
        Debug.Log($"[Robot Initialization] {message}");
    }

    private void CheckSerialConnection()
    {
        if (SerialCOMSliders.Instance == null)
        {
            Debug.LogWarning("SerialCOMSliders instance is null!");
            return;
        }

        if (!SerialCOMSliders.Instance.IsConnected())
        {
            Debug.LogWarning("Serial connection lost. Status: " + SerialCOMSliders.Instance.GetConnectionStatus());
            DisableSliders();
        }
        else if (!slidersEnabled && isInitialized)
        {
            EnableSliders();
        }
    }

    // Helper method to safely use SerialCOMSliders
    private bool TryUseSerial(System.Action action)
    {
        if (!isInitialized)
        {
            return false; // Don't send commands during initialization
        }

        if (SerialCOMSliders.Instance == null)
        {
            Debug.LogWarning("SerialCOMSliders.Instance is null. Cannot send command to robot.");
            return false;
        }

        if (!SerialCOMSliders.Instance.IsConnected())
        {
            Debug.LogWarning("SerialCOMSliders is not connected. Cannot send command to robot.");
            return false;
        }

        try
        {
            action.Invoke();
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error using SerialCOMSliders: {ex.Message}");
            return false;
        }
    }

    // Optimized individual slider methods - only send if value changed significantly
    private bool ShouldUpdateValue(float currentValue, float lastValue)
    {
        return onlySendChangedValues ?
            Mathf.Abs(currentValue - lastValue) >= valueChangeThreshold :
            true;
    }

    // Sets the slider min and max according to the values set through Unity.
    private void SliderValuesInitialize()
    {
        if (baseSlider != null)
        {
            baseSlider.minValue = baseMinRotation;
            baseSlider.maxValue = baseMaxRotation;
        }

        if (upperArmSlider != null)
        {
            upperArmSlider.minValue = upperArmMinRotation;
            upperArmSlider.maxValue = upperArmMaxRotation;
        }

        if (lowerArmSlider != null)
        {
            lowerArmSlider.minValue = lowerArmMinRotation;
            lowerArmSlider.maxValue = lowerArmMaxRotation;
        }

        if (clawSlider != null)
        {
            clawSlider.minValue = rightClawMinRotation;
            clawSlider.maxValue = rightClawMaxRotation;
        }
    }

    // Rotates the base of the robot according to the slider value.
    public void RotateBaseRotator(float value)
    {
        if (!isInitialized) return;

        if (baseTransform != null)
        {
            baseTransform.localEulerAngles = new Vector3(baseTransform.localEulerAngles.x, baseTransform.localEulerAngles.y, -value);
        }

        // Only send if value changed significantly
        if (ShouldUpdateValue(value, lastBaseValue))
        {
            TryUseSerial(() =>
            {
                float remappedBaseValue = math.remap(-80, 80, 45, 135, value);
                SerialCOMSliders.Instance.baseValue = remappedBaseValue;
                SerialCOMSliders.Instance.WriteSerial();
            });
            lastBaseValue = value;
        }
    }

    // Rotates the Upper Arm of the robot according to the slider value.
    public void RotateUpperArmRotator(float value)
    {
        if (!isInitialized) return;

        if (upperArmTransform != null)
        {
            upperArmTransform.localEulerAngles = new Vector3(value, upperArmTransform.localEulerAngles.y, upperArmTransform.localEulerAngles.z);
        }

        if (ShouldUpdateValue(value, lastUpperArmValue))
        {
            TryUseSerial(() =>
            {
                float remappedUpperArmValue = math.remap(0, -70, 0, 65, value);
                SerialCOMSliders.Instance.upperArmValue = remappedUpperArmValue;
                SerialCOMSliders.Instance.WriteSerial();
            });
            lastUpperArmValue = value;
        }
    }

    // Rotates the Lower Arm of the robot according to the slider value.
    public void RotateLowerArmRotator(float value)
    {
        if (!isInitialized) return;

        if (lowerArmTransform != null)
        {
            lowerArmTransform.localEulerAngles = new Vector3(value, lowerArmTransform.localEulerAngles.y, lowerArmTransform.localEulerAngles.z);
        }

        if (ShouldUpdateValue(value, lastLowerArmValue))
        {
            TryUseSerial(() =>
            {
                float remappedLowerArmValue = math.remap(-40, 30, 45, 145, value);
                SerialCOMSliders.Instance.lowerArmValue = remappedLowerArmValue;
                SerialCOMSliders.Instance.WriteSerial();
            });
            lastLowerArmValue = value;
        }
    }

    // Opens and closes the pinchers
    public void RotatePincherArmRotator(float value)
    {
        if (!isInitialized) return;

        if (leftClawTransform != null)
        {
            leftClawTransform.localEulerAngles = new Vector3(
                leftClawTransform.localEulerAngles.x,
                leftClawTransform.localEulerAngles.y,
                -value
            );
        }

        if (rightClawTransform != null)
        {
            rightClawTransform.localEulerAngles = new Vector3(
                rightClawTransform.localEulerAngles.x,
                rightClawTransform.localEulerAngles.y,
                value
            );
        }

        if (ShouldUpdateValue(value, lastClawValue))
        {
            TryUseSerial(() =>
            {
                float remappedClawValue = math.remap(0, 50, 0, 115, value);
                SerialCOMSliders.Instance.clawValue = remappedClawValue;
                SerialCOMSliders.Instance.WriteSerial();
            });
            lastClawValue = value;
        }
    }

    // Public method to reinitialize robot (for reset button)
    public void ReinitializeRobot()
    {
        if (m_isLerping) return;

        isInitialized = false;
        DisableSliders();
        StartCoroutine(InitializationSequence());
    }

    // Save current position as home position
    public void SaveCurrentAsHome()
    {
        baseHomePosition = baseSlider.value;
        upperArmHomePosition = upperArmSlider.value;
        lowerArmHomePosition = lowerArmSlider.value;
        clawHomePosition = clawSlider.value;

        // Also save to PlayerPrefs for persistence
        PlayerPrefs.SetFloat("HomeBase", baseHomePosition);
        PlayerPrefs.SetFloat("HomeUpperArm", upperArmHomePosition);
        PlayerPrefs.SetFloat("HomeLowerArm", lowerArmHomePosition);
        PlayerPrefs.SetFloat("HomeClaw", clawHomePosition);
        PlayerPrefs.Save();

        Debug.Log("Current position saved as new home position!");
    }

    // Save Servo Position (existing functionality)
    public void SaveServoPosition(int positionIndex)
    {
        if (!isInitialized) return;

        if (baseSlider == null || upperArmSlider == null || lowerArmSlider == null || clawSlider == null)
        {
            Debug.LogWarning("One or more sliders are null. Cannot save position.");
            return;
        }

        PlayerPrefs.SetFloat($"Position{positionIndex}_BaseSliderValue", baseSlider.value);
        PlayerPrefs.SetFloat($"Position{positionIndex}_UpperArmSliderValue", upperArmSlider.value);
        PlayerPrefs.SetFloat($"Position{positionIndex}_LowerArmSliderValue", lowerArmSlider.value);
        PlayerPrefs.SetFloat($"Position{positionIndex}_ClawSliderValue", clawSlider.value);

        PlayerPrefs.SetFloat($"Base_Rotation{positionIndex}", baseRotationSpeed);
        PlayerPrefs.SetFloat($"UpperArm_Rotation{positionIndex}", upperArmRotationSpeed);
        PlayerPrefs.SetFloat($"LowerArm_Rotation{positionIndex}", lowerArmRotationSpeed);
        PlayerPrefs.SetFloat($"Claw_LeftRotation{positionIndex}", leftClawRotationSpeed);
        PlayerPrefs.SetFloat($"Claw_RightRotation{positionIndex}", rightClawRotationSpeed);

        // Also save current positions as "last known good"
        PlayerPrefs.SetFloat("LastKnownBase", baseSlider.value);
        PlayerPrefs.SetFloat("LastKnownUpperArm", upperArmSlider.value);
        PlayerPrefs.SetFloat("LastKnownLowerArm", lowerArmSlider.value);
        PlayerPrefs.SetFloat("LastKnownClaw", clawSlider.value);

        PlayerPrefs.Save();
        Debug.Log($"Position {positionIndex} saved successfully.");
    }

    // Load Servo Position (existing functionality)
    public void LoadServoPosition(int positionIndex)
    {
        if (!isInitialized || m_isLerping)
        {
            Debug.Log("Robot not ready or currently lerping, ignoring load command.");
            return;
        }

        if (!PlayerPrefs.HasKey($"Position{positionIndex}_BaseSliderValue"))
        {
            Debug.LogWarning($"No saved position found for index {positionIndex}");
            return;
        }

        if (baseSlider == null || upperArmSlider == null || lowerArmSlider == null || clawSlider == null)
        {
            Debug.LogWarning("One or more sliders are null. Cannot load position.");
            return;
        }

        m_isLerping = true;
        DisableSliders();

        Base_SliderValue = PlayerPrefs.GetFloat($"Position{positionIndex}_BaseSliderValue");
        UpperArm_SliderValue = PlayerPrefs.GetFloat($"Position{positionIndex}_UpperArmSliderValue");
        LowerArm_SliderValue = PlayerPrefs.GetFloat($"Position{positionIndex}_LowerArmSliderValue");
        Claw_SliderValue = PlayerPrefs.GetFloat($"Position{positionIndex}_ClawSliderValue");

        StartCoroutine(LerpSliders(Base_SliderValue, UpperArm_SliderValue, LowerArm_SliderValue, Claw_SliderValue));
        Debug.Log($"Position {positionIndex} loaded successfully.");
    }

    // Smoothly move the sliders to the saved positions over time.
    private IEnumerator LerpSliders(float baseEndPoint, float upperArmEndPoint, float lowerArmEndPoint, float clawEndPoint)
    {
        if (baseSlider == null || upperArmSlider == null || lowerArmSlider == null || clawSlider == null)
        {
            Debug.LogWarning("One or more sliders are null. Cannot lerp sliders.");
            m_isLerping = false;
            EnableSliders();
            yield break;
        }

        float elapsedTime = 0;
        float baseSliderStartValue = baseSlider.value;
        float upperArmSliderStartValue = upperArmSlider.value;
        float lowerArmSliderStartValue = lowerArmSlider.value;
        float clawSliderStartValue = clawSlider.value;

        while (elapsedTime < lerpTime)
        {
            float t = elapsedTime / lerpTime;

            SetSliderValueQuietly(baseSlider, Mathf.Lerp(baseSliderStartValue, baseEndPoint, t));
            SetSliderValueQuietly(upperArmSlider, Mathf.Lerp(upperArmSliderStartValue, upperArmEndPoint, t));
            SetSliderValueQuietly(lowerArmSlider, Mathf.Lerp(lowerArmSliderStartValue, lowerArmEndPoint, t));
            SetSliderValueQuietly(clawSlider, Mathf.Lerp(clawSliderStartValue, clawEndPoint, t));

            UpdateTransforms();
            SendAllValuesToRobot();

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure final values are set
        SetSliderValueQuietly(baseSlider, baseEndPoint);
        SetSliderValueQuietly(upperArmSlider, upperArmEndPoint);
        SetSliderValueQuietly(lowerArmSlider, lowerArmEndPoint);
        SetSliderValueQuietly(clawSlider, clawEndPoint);

        UpdateTransforms();
        SendAllValuesToRobot();
        StoreCurrentValues();

        m_isLerping = false;
        EnableSliders();
    }

    // Resets the Robot to the home position.
    public void ResetTransformRotation()
    {
        if (!isInitialized || m_isLerping)
        {
            Debug.Log("Robot not ready or currently lerping, ignoring reset command.");
            return;
        }

        m_isLerping = true;
        DisableSliders();

        StartCoroutine(LerpSliders(baseHomePosition, upperArmHomePosition, lowerArmHomePosition, clawHomePosition));
        Debug.Log("Robot reset to home position.");
    }

    private void OnDestroy()
    {
        // Save current positions when leaving scene
        if (isInitialized)
        {
            PlayerPrefs.SetFloat("LastKnownBase", baseSlider?.value ?? baseHomePosition);
            PlayerPrefs.SetFloat("LastKnownUpperArm", upperArmSlider?.value ?? upperArmHomePosition);
            PlayerPrefs.SetFloat("LastKnownLowerArm", lowerArmSlider?.value ?? lowerArmHomePosition);
            PlayerPrefs.SetFloat("LastKnownClaw", clawSlider?.value ?? clawHomePosition);
            PlayerPrefs.Save();
        }
    }
}