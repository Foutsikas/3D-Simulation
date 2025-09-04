using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

/// <summary>
/// Simple popup that reminds users to hit reset before controlling the robot
/// Shows after robot initialization is complete
/// </summary>
public class ResetReminder : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject popupPanel;
    public TMP_Text messageText;
    public Button gotItButton;
    public Button resetNowButton; // Optional - directly calls reset

    [Header("References")]
    public ControlledBySlider robotController;

    [Header("Settings")]
    [TextArea(3, 5)]
    public string reminderMessage = "IMPORTANT: For safety, always hit the RESET \r\nbutton first. This will:\r\n- Move robot to home position\r\n- Ensure safe starting point\r\n- Prevent unexpected movements\r\n\r\nReady to control after reset!";

    public bool showOnlyOnce = false; // Show popup only once per session
    public bool autoCloseAfterReset = true; // Auto close when reset is pressed

    private bool hasShownThisSession = false;
    private bool isWaitingForInitialization = false;

    void Start()
    {
        SetupButtons();

        if (popupPanel != null)
            popupPanel.SetActive(false);

        if (messageText != null)
            messageText.text = reminderMessage;

        // Start monitoring for initialization completion
        if (robotController != null)
        {
            StartCoroutine(WaitForInitializationComplete());
        }
    }

    void Update()
    {
        // Monitor if reset was pressed to auto-close popup
        if (autoCloseAfterReset && popupPanel != null && popupPanel.activeSelf)
        {
            if (robotController != null && robotController.IsLerping)
            {
                // Reset is being executed, close the popup
                ClosePopup();
            }
        }
    }

    private void SetupButtons()
    {
        if (gotItButton != null)
            gotItButton.onClick.AddListener(ClosePopup);

        if (resetNowButton != null && robotController != null)
            resetNowButton.onClick.AddListener(() => {
                robotController.ResetTransformRotation();
                ClosePopup();
            });
    }

    private IEnumerator WaitForInitializationComplete()
    {
        isWaitingForInitialization = true;

        // Wait for robot to be initialized and not lerping
        while (robotController != null)
        {
            // Check if robot is initialized (using reflection to access private field)
            var isInitializedField = robotController.GetType().GetField("isInitialized",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            bool isInitialized = isInitializedField?.GetValue(robotController) as bool? ?? false;

            if (isInitialized && !robotController.IsLerping)
            {
                // Robot initialization is complete
                ShowPopup();
                break;
            }

            yield return new WaitForSeconds(0.2f);
        }

        isWaitingForInitialization = false;
    }

    public void ShowPopup()
    {
        // Check if we should show the popup
        if (showOnlyOnce && hasShownThisSession)
            return;

        if (popupPanel != null)
        {
            popupPanel.SetActive(true);
            hasShownThisSession = true;

            Debug.Log("Reset reminder popup shown - Robot ready for control");
        }
    }

    public void ClosePopup()
    {
        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
            Debug.Log("Reset reminder popup closed");
        }
    }

    // Public method to manually show popup (if needed)
    public void ManualShowPopup()
    {
        ShowPopup();
    }

    // Call this method if you want to reset the "show only once" flag
    public void ResetShowOnceFlag()
    {
        hasShownThisSession = false;
    }
}