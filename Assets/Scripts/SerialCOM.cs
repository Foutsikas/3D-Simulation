using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;
using TMPro;

public class SerialCOM : MonoBehaviour
{
    public static SerialCOM i;

    #region Serial Port Communication Initializer
    //variable decleration field

    //Port speed in bps
    public int baudrate = 9600;

    //Serial Port Decleration
    static private SerialPort sp;

    //Thread init
    Thread readThread; //= new Thread(Read);

    //boolean for value reading
    static bool isStreaming;

    string incomingValue = null;

    #region Servo Values
    public int S1, S2, S3, S4;
    #endregion

    #endregion

    public TMP_Text statusText;
    public float fadeDuration = 2f; // Duration for fading the status text
    public float displayDuration = 3f; // Duration for displaying the status text

    void Awake()
    {
        // Find the COM port of the Arduino device
        string arduinoPort = FindArduinoPort();
        if (arduinoPort == null)
        {
            Debug.LogError("Could not find an Arduino device.");
            SetStatusText("Device Failed To Connect");
            return;
        }
        // Connect to the Arduino port
        sp = new SerialPort(arduinoPort, baudrate);

        if (i == null)
        {
            i = this;
        }
        else
        {
            Destroy(this);
        }
    }

    // Finds the COM port of the Arduino device based on its name string
    string FindArduinoPort()
    {
        string arduinoPort = null;

        // Read the VID and PID from the config.txt file in the StreamingAssets folder
        string configPath = Path.Combine(Application.dataPath, "StreamingAssets/config.txt");
        string[] configLines = File.ReadAllLines(configPath);
        {
            string VID = null;
            string PID = null;

            // Extract PID and VID from config.txt
            foreach (string line in configLines)
            {
                if (line.StartsWith("VID"))
                {
                    VID = line.Split('=')[1].Trim();
                }
                else if (line.StartsWith("PID"))
                {
                    PID = line.Split('=')[1].Trim();
                }
            }

            if (PID == null || VID == null)
            {
                Debug.LogWarning("Could not find PID or VID in config.txt.");
                SetStatusText("Device Failed To Connect");
            }

            // Compile an array of COM port names associated with the given VID and PID
            List<string> comports = ComPortNames(VID, PID);

            // Use the first COM port from the compiled list
            if (comports.Count > 0)
            {
                arduinoPort = comports[0];
                SetStatusText("Device Connected");
            }
            else
            {
                SetStatusText("Device Failed To Connect");
            }
        }

        if (arduinoPort == null)
        {
            Debug.LogError("Could not find an Arduino device.");
            SetStatusText("Device Failed To Connect");
            return null;
        }

        // Connect to the Arduino port
        sp = new SerialPort(arduinoPort, baudrate);

        return arduinoPort;
    }

    List<string> ComPortNames(String VID, String PID)
    {
        String pattern = String.Format("^VID_{0}.PID_{1}", VID, PID);
        Regex _rx = new(pattern, RegexOptions.IgnoreCase);
        List<string> comports = new();
        RegistryKey rk1 = Registry.LocalMachine;
        RegistryKey rk2 = rk1.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum");
        foreach (String s3 in rk2.GetSubKeyNames())
        {
            RegistryKey rk3 = rk2.OpenSubKey(s3);
            foreach (String s in rk3.GetSubKeyNames())
            {
                if (_rx.Match(s).Success)
                {
                    RegistryKey rk4 = rk3.OpenSubKey(s);
                    foreach (String s2 in rk4.GetSubKeyNames())
                    {
                        RegistryKey rk5 = rk4.OpenSubKey(s2);
                        RegistryKey rk6 = rk5.OpenSubKey("Device Parameters");
                        comports.Add((string)rk6.GetValue("PortName"));
                    }
                }
            }
        }
        return comports;
    }

    void SetStatusText(string message)
    {
        statusText.text = message;
        StartCoroutine(FadeStatusText());
    }

    System.Collections.IEnumerator FadeStatusText()
    {
        yield return new WaitForSeconds(displayDuration);

        float elapsedTime = 0f;
        Color initialColor = statusText.color;

        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            statusText.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        statusText.gameObject.SetActive(false);
    }

    void Start()
    {
        Open();
    }

    //Opens Serial Port and set the program to read values from it.
    public void Open()
    {
        sp.Open();
        readThread = new Thread(Read);
        readThread.Start();
        isStreaming = true;
    }

    //Closes Serial Port
    public void Close()
    {
        isStreaming = false;
        sp.Close();
        readThread.Join();
        Debug.Log("Port was Closed!");
    }

    //If the program terminates unexpectedly, closes the port and switch back to the main thread.
    void OnDestroy()
    {
        isStreaming = false;
        readThread.Join();
        sp.Close();
    }

    public static void Read()
    {
        while (isStreaming)
        {
            try
            {
                string temp = i.incomingValue;
                i.incomingValue = sp.ReadLine();
                if (temp != i.incomingValue)
                {
                    i.StringConvert(i.incomingValue);
                }
            }
            catch (TimeoutException) { }
        }
    }

    //The StringConvert takes the incoming arduino data string,
    //removes the symbols, keeps the arithmetic values, parses them from int to string
    //and saves the converted value to the corresponding variable S1-S4.
    public void StringConvert(string value)
    {
        if (value == null)
        {
            return;
        }

        StringBuilder[] sb = new StringBuilder[4];
        int i = 0;
        for (int x = 0; x < value.Length; x++)
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

            (sb[i] ?? (sb[i] = new StringBuilder())).Append(value[x]);
        }

        S1 = int.Parse(sb[0].ToString());
        S2 = int.Parse(sb[1].ToString());
        S3 = int.Parse(sb[2].ToString());
        S4 = int.Parse(sb[3].ToString());
    }
}
