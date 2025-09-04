using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Utility class for managing robot settings and saved positions
/// Attach to a settings panel or manager GameObject
/// </summary>
public class RobotSettingsManager : MonoBehaviour
{
    [Header("Settings UI")]
    public Button clearAllPositionsButton;
    public Button resetToFactoryButton;
    public Button exportSettingsButton;
    public Button importSettingsButton;

    [Header("Position Management")]
    public Text[] positionStatusTexts = new Text[4]; // For 4 save slots
    public Button[] clearPositionButtons = new Button[4];

    [Header("Home Position Settings")]
    public Slider homeBaseSlider;
    public Slider homeUpperArmSlider;
    public Slider homeLowerArmSlider;
    public Slider homeClawSlider;
    public Button setHomeButton;
    public Button loadHomeButton;

    [Header("References")]
    public ControlledBySlider robotController;

    private void Start()
    {
        SetupButtons();
        UpdatePositionStatus();
        LoadHomePositionSettings();
    }

    private void SetupButtons()
    {
        if (clearAllPositionsButton != null)
            clearAllPositionsButton.onClick.AddListener(ClearAllPositions);

        if (resetToFactoryButton != null)
            resetToFactoryButton.onClick.AddListener(ResetToFactoryDefaults);

        if (exportSettingsButton != null)
            exportSettingsButton.onClick.AddListener(ExportSettings);

        if (importSettingsButton != null)
            importSettingsButton.onClick.AddListener(ImportSettings);

        if (setHomeButton != null)
            setHomeButton.onClick.AddListener(SetCustomHome);

        if (loadHomeButton != null)
            loadHomeButton.onClick.AddListener(LoadHomePosition);

        // Setup individual position clear buttons
        for (int i = 0; i < clearPositionButtons.Length; i++)
        {
            if (clearPositionButtons[i] != null)
            {
                int index = i + 1; // Position indices are 1-based
                clearPositionButtons[i].onClick.AddListener(() => ClearPosition(index));
            }
        }
    }

    private void UpdatePositionStatus()
    {
        for (int i = 0; i < positionStatusTexts.Length; i++)
        {
            if (positionStatusTexts[i] != null)
            {
                int positionIndex = i + 1; // Positions are 1-based
                bool hasPosition = PlayerPrefs.HasKey($"Position{positionIndex}_BaseSliderValue");

                if (hasPosition)
                {
                    float baseValue = PlayerPrefs.GetFloat($"Position{positionIndex}_BaseSliderValue");
                    float upperValue = PlayerPrefs.GetFloat($"Position{positionIndex}_UpperArmSliderValue");
                    float lowerValue = PlayerPrefs.GetFloat($"Position{positionIndex}_LowerArmSliderValue");
                    float clawValue = PlayerPrefs.GetFloat($"Position{positionIndex}_ClawSliderValue");

                    positionStatusTexts[i].text = $"Slot {positionIndex}: Saved\nB:{baseValue:F1} U:{upperValue:F1} L:{lowerValue:F1} C:{clawValue:F1}";
                    positionStatusTexts[i].color = Color.green;
                }
                else
                {
                    positionStatusTexts[i].text = $"Slot {positionIndex}: Empty";
                    positionStatusTexts[i].color = Color.gray;
                }
            }
        }
    }

    private void LoadHomePositionSettings()
    {
        if (homeBaseSlider != null)
            homeBaseSlider.value = PlayerPrefs.GetFloat("HomeBase", 0f);

        if (homeUpperArmSlider != null)
            homeUpperArmSlider.value = PlayerPrefs.GetFloat("HomeUpperArm", -20f);

        if (homeLowerArmSlider != null)
            homeLowerArmSlider.value = PlayerPrefs.GetFloat("HomeLowerArm", 10f);

        if (homeClawSlider != null)
            homeClawSlider.value = PlayerPrefs.GetFloat("HomeClaw", 25f);
    }

    public void ClearPosition(int positionIndex)
    {
        PlayerPrefs.DeleteKey($"Position{positionIndex}_BaseSliderValue");
        PlayerPrefs.DeleteKey($"Position{positionIndex}_UpperArmSliderValue");
        PlayerPrefs.DeleteKey($"Position{positionIndex}_LowerArmSliderValue");
        PlayerPrefs.DeleteKey($"Position{positionIndex}_ClawSliderValue");
        PlayerPrefs.DeleteKey($"Base_Rotation{positionIndex}");
        PlayerPrefs.DeleteKey($"UpperArm_Rotation{positionIndex}");
        PlayerPrefs.DeleteKey($"LowerArm_Rotation{positionIndex}");
        PlayerPrefs.DeleteKey($"Claw_LeftRotation{positionIndex}");
        PlayerPrefs.DeleteKey($"Claw_RightRotation{positionIndex}");

        PlayerPrefs.Save();
        UpdatePositionStatus();

        Debug.Log($"Position {positionIndex} cleared.");
    }

    public void ClearAllPositions()
    {
        for (int i = 1; i <= 4; i++)
        {
            ClearPosition(i);
        }

        Debug.Log("All positions cleared.");
    }

    public void ResetToFactoryDefaults()
    {
        // Clear all saved positions
        ClearAllPositions();

        // Reset home positions
        PlayerPrefs.DeleteKey("HomeBase");
        PlayerPrefs.DeleteKey("HomeUpperArm");
        PlayerPrefs.DeleteKey("HomeLowerArm");
        PlayerPrefs.DeleteKey("HomeClaw");

        // Reset last known positions
        PlayerPrefs.DeleteKey("LastKnownBase");
        PlayerPrefs.DeleteKey("LastKnownUpperArm");
        PlayerPrefs.DeleteKey("LastKnownLowerArm");
        PlayerPrefs.DeleteKey("LastKnownClaw");

        PlayerPrefs.Save();
        LoadHomePositionSettings();

        Debug.Log("Factory defaults restored.");
    }

    public void SetCustomHome()
    {
        if (homeBaseSlider == null || homeUpperArmSlider == null ||
            homeLowerArmSlider == null || homeClawSlider == null)
        {
            Debug.LogWarning("Home position sliders not assigned.");
            return;
        }

        PlayerPrefs.SetFloat("HomeBase", homeBaseSlider.value);
        PlayerPrefs.SetFloat("HomeUpperArm", homeUpperArmSlider.value);
        PlayerPrefs.SetFloat("HomeLowerArm", homeLowerArmSlider.value);
        PlayerPrefs.SetFloat("HomeClaw", homeClawSlider.value);
        PlayerPrefs.Save();

        Debug.Log("Custom home position set.");
    }

    public void LoadHomePosition()
    {
        if (robotController != null)
        {
            // This would trigger the robot to move to home position
            robotController.ResetTransformRotation();
        }
    }

    public void ExportSettings()
    {
        // Create a simple settings export string
        string settings = "";
        settings += $"HomeBase={PlayerPrefs.GetFloat("HomeBase", 0f)}\n";
        settings += $"HomeUpperArm={PlayerPrefs.GetFloat("HomeUpperArm", -20f)}\n";
        settings += $"HomeLowerArm={PlayerPrefs.GetFloat("HomeLowerArm", 10f)}\n";
        settings += $"HomeClaw={PlayerPrefs.GetFloat("HomeClaw", 25f)}\n";

        for (int i = 1; i <= 4; i++)
        {
            if (PlayerPrefs.HasKey($"Position{i}_BaseSliderValue"))
            {
                settings += $"Position{i}_Base={PlayerPrefs.GetFloat($"Position{i}_BaseSliderValue")}\n";
                settings += $"Position{i}_UpperArm={PlayerPrefs.GetFloat($"Position{i}_UpperArmSliderValue")}\n";
                settings += $"Position{i}_LowerArm={PlayerPrefs.GetFloat($"Position{i}_LowerArmSliderValue")}\n";
                settings += $"Position{i}_Claw={PlayerPrefs.GetFloat($"Position{i}_ClawSliderValue")}\n";
            }
        }

        // In a real application, you'd save this to a file
        // For now, we'll just copy to clipboard (requires TextMeshPro)
        GUIUtility.systemCopyBuffer = settings;
        Debug.Log("Settings exported to clipboard:\n" + settings);
    }

    public void ImportSettings()
    {
        // In a real application, you'd load from a file
        // For now, we'll read from clipboard
        string settings = GUIUtility.systemCopyBuffer;

        if (string.IsNullOrEmpty(settings))
        {
            Debug.LogWarning("Clipboard is empty. Cannot import settings.");
            return;
        }

        try
        {
            string[] lines = settings.Split('\n');
            foreach (string line in lines)
            {
                if (line.Contains("="))
                {
                    string[] parts = line.Split('=');
                    if (parts.Length == 2)
                    {
                        string key = parts[0].Trim();
                        if (float.TryParse(parts[1].Trim(), out float value))
                        {
                            PlayerPrefs.SetFloat(key, value);
                        }
                    }
                }
            }

            PlayerPrefs.Save();
            UpdatePositionStatus();
            LoadHomePositionSettings();

            Debug.Log("Settings imported successfully.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to import settings: {ex.Message}");
        }
    }

    // Call this when positions are saved/loaded to update UI
    public void RefreshUI()
    {
        UpdatePositionStatus();
    }

    // Utility method to check storage usage
    public void ShowStorageInfo()
    {
        int savedPositions = 0;
        for (int i = 1; i <= 4; i++)
        {
            if (PlayerPrefs.HasKey($"Position{i}_BaseSliderValue"))
                savedPositions++;
        }

        bool hasCustomHome = PlayerPrefs.HasKey("HomeBase");
        bool hasLastKnown = PlayerPrefs.HasKey("LastKnownBase");

        string info = $"Storage Info:\n";
        info += $"Saved Positions: {savedPositions}/4\n";
        info += $"Custom Home: {(hasCustomHome ? "Yes" : "No")}\n";
        info += $"Last Known Position: {(hasLastKnown ? "Yes" : "No")}";

        Debug.Log(info);
    }
}