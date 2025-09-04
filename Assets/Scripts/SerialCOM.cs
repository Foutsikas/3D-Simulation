using System;
using System.IO.Ports;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine;

public class SerialCOM : MonoBehaviour
{
    public static SerialCOM i;

    #region Serial Port Communication
    private SerialPort sp;
    private Thread readThread;
    private static bool isStreaming;
    private string incomingValue = null;
    private string currentPortName = null;

    #region Servo Values
    public int S1, S2, S3, S4;
    #endregion
    #endregion

    [Header("UI Components")]
    public TMP_Text statusText;
    public float fadeDuration = 2f;
    public float displayDuration = 3f;

    private bool isRetryingConnection = false;
    private bool isInitialized = false;

    void Awake()
    {
        // Remove any existing instances when entering this scene
        if (i != null && i != this)
        {
            i.CleanupConnection();
            Destroy(i.gameObject);
        }

        i = this;
        // DON'T use DontDestroyOnLoad - this should be scene-specific
    }

    void Start()
    {
        InitializeConnection();
    }

    void OnEnable()
    {
        if (isInitialized && sp != null && !sp.IsOpen)
        {
            Open();
        }
    }

    void OnDisable()
    {
        CleanupConnection();
    }

    private void InitializeConnection()
    {
        if (isInitialized) return;

        try
        {
            string arduinoPort = ArduinoPortManager.FindArduinoPort();
            if (string.IsNullOrEmpty(arduinoPort))
            {
                Debug.LogWarning("Could not find an Arduino device. Will retry connection.");
                SetStatusText("Device Not Found - Retrying...");
                StartRetryConnection();
                return;
            }

            currentPortName = arduinoPort;
            SetupSerialPort(arduinoPort);
            SetStatusText("Device Connected");
            isInitialized = true;
            Open();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to initialize connection: {ex.Message}");
            SetStatusText("Device Failed To Connect");
            StartRetryConnection();
        }
    }

    private void SetupSerialPort(string portName)
    {
        ClosePort(); // Close any existing connection

        sp = new SerialPort(portName, ArduinoPortManager.GetBaudRate())
        {
            ReadTimeout = ArduinoPortManager.GetReadTimeout(),
            WriteTimeout = ArduinoPortManager.GetWriteTimeout(),
            DtrEnable = false,
            RtsEnable = false
        };

        Debug.Log($"Arduino found on port: {portName}");
    }

    private void StartRetryConnection()
    {
        if (!isRetryingConnection)
        {
            isRetryingConnection = true;
            InvokeRepeating(nameof(RetryConnection), ArduinoPortManager.GetRetryInterval(), ArduinoPortManager.GetRetryInterval());
        }
    }

    private void RetryConnection()
    {
        if (sp != null && sp.IsOpen)
        {
            isRetryingConnection = false;
            CancelInvoke(nameof(RetryConnection));
            return;
        }

        Debug.Log("Retrying Arduino connection...");
        isInitialized = false;
        InitializeConnection();
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

    System.Collections.IEnumerator FadeStatusText()
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

    public void Open()
    {
        try
        {
            if (sp == null)
            {
                Debug.LogWarning("SerialPort is null. Cannot open connection.");
                return;
            }

            if (sp.IsOpen)
            {
                Debug.Log("Port is already open.");
                return;
            }

            sp.Open();
            readThread = new Thread(Read) { IsBackground = true };
            readThread.Start();
            isStreaming = true;

            if (isRetryingConnection)
            {
                isRetryingConnection = false;
                CancelInvoke(nameof(RetryConnection));
            }

            Debug.Log($"Successfully opened port: {currentPortName}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to open serial port: {ex.Message}");
            SetStatusText("Failed to Connect");
            StartRetryConnection();
        }
    }

    private void ClosePort()
    {
        try
        {
            isStreaming = false;

            if (readThread != null && readThread.IsAlive)
            {
                if (!readThread.Join(1000))
                {
                    readThread.Abort();
                }
                readThread = null;
            }

            if (sp != null && sp.IsOpen)
            {
                sp.Close();
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Error closing port: {ex.Message}");
        }
    }

    private void CleanupConnection()
    {
        ClosePort();
        CancelInvoke();
        if (i == this) i = null;
    }

    void OnDestroy()
    {
        CleanupConnection();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            ClosePort();
        }
        else if (sp != null && !sp.IsOpen)
        {
            Open();
        }
    }

    public static void Read()
    {
        while (isStreaming && i != null && i.sp != null)
        {
            try
            {
                if (!i.sp.IsOpen)
                {
                    Thread.Sleep(100);
                    continue;
                }

                string temp = i.incomingValue;
                i.incomingValue = i.sp.ReadLine();

                if (!string.IsNullOrEmpty(i.incomingValue) && temp != i.incomingValue)
                {
                    i.StringConvert(i.incomingValue);
                }
            }
            catch (TimeoutException)
            {
                // Normal timeout, continue reading
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Read error: {ex.Message}");
                Thread.Sleep(100);
            }
        }
    }

    public void StringConvert(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return;
        }

        try
        {
            StringBuilder[] sb = new StringBuilder[4];
            int i = 0;

            for (int x = 0; x < value.Length && i < 4; x++)
            {
                if (value[x] == '$')
                {
                    continue;
                }

                if (value[x] == '#')
                {
                    i++;
                    continue;
                }

                sb[i] ??= new StringBuilder();
                sb[i].Append(value[x]);
            }

            // Safely parse values with error checking
            if (sb[0] != null && int.TryParse(sb[0].ToString(), out int s1)) S1 = s1;
            if (sb[1] != null && int.TryParse(sb[1].ToString(), out int s2)) S2 = s2;
            if (sb[2] != null && int.TryParse(sb[2].ToString(), out int s3)) S3 = s3;
            if (sb[3] != null && int.TryParse(sb[3].ToString(), out int s4)) S4 = s4;
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"String conversion error: {ex.Message} - Input: {value}");
        }
    }
}