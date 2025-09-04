using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine;

public class SerialCOMSliders : MonoBehaviour
{
    private static SerialCOMSliders instance;
    public static SerialCOMSliders Instance => instance;

    #region Serial Port Communication
    private SerialPort serialPort;
    private string currentPortName = null;
    #endregion

    #region UI Slider Values
    [Header("Servo Values")]
    public float baseValue = 90f;
    public float upperArmValue = 90f;
    public float lowerArmValue = 90f;
    public float clawValue = 90f;
    #endregion

    [Header("UI Components")]
    public TMP_Text statusText;
    public float fadeDuration = 2f;
    public float displayDuration = 3f;

    [Header("Connection Settings")]
    public int baudRate = 9600;
    public int writeTimeout = 1000;
    [Tooltip("Time in seconds between connection retry attempts")]
    public float retryInterval = 5f;
    private bool isRetryingConnection = false;
    private bool isInitialized = false;

    [Header("Data Transmission")]
    [Tooltip("Minimum time between serial writes in seconds")]
    public float writeThrottleTime = 0.1f;
    private float lastWriteTime = 0f;
    private bool hasPendingWrite = false;

    private void Awake()
    {
        // Remove any existing instances when entering this scene
        if (instance != null && instance != this)
        {
            instance.CleanupConnection();
            Destroy(instance.gameObject);
        }

        instance = this;
        // DON'T use DontDestroyOnLoad - this should be scene-specific
    }

    private void Start()
    {
        InitializeConnection();
    }

    void OnEnable()
    {
        if (isInitialized && serialPort != null && !serialPort.IsOpen)
        {
            OpenSerialPort(currentPortName);
        }
    }

    void OnDisable()
    {
        CleanupConnection();
    }

    private void Update()
    {
        // Handle throttled writes
        if (hasPendingWrite && Time.time - lastWriteTime >= writeThrottleTime)
        {
            PerformWrite();
            hasPendingWrite = false;
        }
    }

    private void InitializeConnection()
    {
        if (isInitialized) return;

        try
        {
            string arduinoPort = ArduinoPortManager.FindArduinoPort();
            if (string.IsNullOrEmpty(arduinoPort))
            {
                Debug.LogWarning("Could not find an Arduino device for sliders. Will retry connection.");
                SetStatusText("Device Not Found - Retrying...");
                StartRetryConnection();
                return;
            }

            currentPortName = arduinoPort;
            OpenSerialPort(arduinoPort);
            SetStatusText("Device Connected");
            isInitialized = true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to initialize slider connection: {ex.Message}");
            SetStatusText("Device Failed To Connect");
            StartRetryConnection();
        }
    }

    private void StartRetryConnection()
    {
        if (!isRetryingConnection)
        {
            isRetryingConnection = true;
            InvokeRepeating(nameof(RetryConnection), retryInterval, retryInterval);
        }
    }

    private void RetryConnection()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            isRetryingConnection = false;
            CancelInvoke(nameof(RetryConnection));
            return;
        }

        Debug.Log("Retrying Arduino connection for sliders...");
        isInitialized = false;
        InitializeConnection();
    }

    void OpenSerialPort(string portName)
    {
        try
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                serialPort.Close();
                serialPort = null;
            }

            if (string.IsNullOrEmpty(portName))
            {
                Debug.LogWarning("Port name is null or empty. Cannot open connection.");
                return;
            }

            // Test the port first
            if (!ArduinoPortManager.TestPortConnection(portName))
            {
                Debug.LogWarning($"Port {portName} is not accessible. Will retry...");
                StartRetryConnection();
                return;
            }

            serialPort = new SerialPort(portName, ArduinoPortManager.GetBaudRate())
            {
                WriteTimeout = ArduinoPortManager.GetWriteTimeout(),
                DtrEnable = false,
                RtsEnable = false
            };

            serialPort.Open();

            if (isRetryingConnection)
            {
                isRetryingConnection = false;
                CancelInvoke(nameof(RetryConnection));
            }

            Debug.Log($"Serial Port Is Open: {serialPort.IsOpen} on port {portName}");
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Failed to open the serial port {portName}: {ex.Message}");
            SetStatusText("Device Failed to Connect");
            StartRetryConnection();
        }
    }

    void SetStatusText(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
            statusText.gameObject.SetActive(true);
            StartCoroutine(FadeStatusText());
        }
    }

    IEnumerator FadeStatusText()
    {
        yield return new WaitForSeconds(displayDuration);

        if (statusText == null) yield break;

        float elapsedTime = 0f;
        Color initialColor = statusText.color;

        while (elapsedTime < fadeDuration && statusText != null)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            statusText.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (statusText != null)
            statusText.gameObject.SetActive(false);
    }

    public void WriteSerial()
    {
        // Ensure instance is available and connection exists
        if (instance == null)
        {
            Debug.LogWarning("SerialCOMSliders instance is null!");
            return;
        }

        if (serialPort == null || !serialPort.IsOpen)
        {
            Debug.LogWarning("Serial port is not available. Attempting to reconnect...");
            if (!isRetryingConnection)
            {
                StartRetryConnection();
            }
            return;
        }

        // Throttle writes to prevent overwhelming the Arduino
        if (Time.time - lastWriteTime < writeThrottleTime)
        {
            hasPendingWrite = true;
            return;
        }

        PerformWrite();
    }

    private void PerformWrite()
    {
        if (serialPort == null || !serialPort.IsOpen)
        {
            return;
        }

        try
        {
            string dataString = $"{baseValue:F0}@{upperArmValue:F0}@{lowerArmValue:F0}@{clawValue:F0}!";

            byte[] data = Encoding.ASCII.GetBytes(dataString);
            serialPort.Write(data, 0, data.Length);
            serialPort.BaseStream.Flush();

            lastWriteTime = Time.time;
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Serial write error: {ex.Message}");
            SetStatusText("Communication Error");

            // Try to reconnect on communication error
            if (!isRetryingConnection)
            {
                StartRetryConnection();
            }
        }
    }

    public void ClosePort()
    {
        try
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                serialPort.Close();
                Debug.Log("Serial port closed successfully.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Error closing serial port: {ex.Message}");
        }
    }

    private void CleanupConnection()
    {
        ClosePort();
        CancelInvoke();
        if (instance == this) instance = null;
        serialPort = null;
    }

    private void OnDestroy()
    {
        CleanupConnection();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            ClosePort();
        }
        else if (serialPort != null && !serialPort.IsOpen)
        {
            InitializeConnection();
        }
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            ClosePort();
        }
        else if (serialPort != null && !serialPort.IsOpen && isInitialized)
        {
            InitializeConnection();
        }
    }

    // Public method to check if the connection is ready
    public bool IsConnected()
    {
        return serialPort != null && serialPort.IsOpen && isInitialized;
    }

    // Public method to get connection status
    public string GetConnectionStatus()
    {
        if (serialPort != null && serialPort.IsOpen)
            return $"Connected to {currentPortName}";
        else if (isRetryingConnection)
            return "Connecting...";
        else
            return "Disconnected";
    }
}