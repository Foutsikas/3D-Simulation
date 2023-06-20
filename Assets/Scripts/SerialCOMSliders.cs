using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;

public class SerialCOMSliders : MonoBehaviour
{
    private static SerialCOMSliders instance;
    public static SerialCOMSliders Instance { get { return instance; } }

    #region Serial Port Communication Initializer
    private SerialPort serialPort;
    private readonly Thread readThread;
    #endregion

    #region UI Slider Declarations
    public float baseValue;
    public float upperArmValue;
    public float lowerArmValue;
    public float clawValue;
    #endregion

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        OpenSerialPort();
    }

    /// Compile an array of COM port names associated with given VID and PID
    List<string> ComPortNames(string VID, string PID)
    {
        string pattern = $"^VID_{VID}.PID_{PID}";
        Regex _rx = new(pattern, RegexOptions.IgnoreCase);
        List<string> comports = new();
        RegistryKey rk1 = Registry.LocalMachine;
        RegistryKey rk2 = rk1.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum");
        foreach (string s3 in rk2.GetSubKeyNames())
        {
            RegistryKey rk3 = rk2.OpenSubKey(s3);
            foreach (string s in rk3.GetSubKeyNames())
            {
                if (_rx.Match(s).Success)
                {
                    RegistryKey rk4 = rk3.OpenSubKey(s);
                    foreach (string s2 in rk4.GetSubKeyNames())
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

    void OpenSerialPort()
    {
        // Read configuration from config.txt in the StreamingAssets folder
        string configPath = Path.Combine(Application.dataPath, "StreamingAssets/config.txt");
        string[] configLines = File.ReadAllLines(configPath);

        string pid = null;
        string vid = null;

        // Extract PID and VID from config.txt
        foreach (string line in configLines)
        {
            if (line.StartsWith("VID"))
            {
                vid = line.Split('=')[1].Trim();
            }
            else if (line.StartsWith("PID"))
            {
                pid = line.Split('=')[1].Trim();
            }
        }

        if (pid == null || vid == null)
        {
            Debug.LogWarning("Could not find PID or VID in config.txt.");
            return;
        }

        List<string> comPorts = ComPortNames(vid, pid);

        if (comPorts.Count > 0)
        {
            serialPort = new SerialPort(comPorts[0], 9600)
            {
                ReadTimeout = 1
            };
            serialPort.Open();
            Debug.Log("Serial Port Is Open: " + serialPort.IsOpen);
        }
        else
        {
            Debug.LogWarning("No COM port found for VID " + vid + " and PID " + pid);
        }
    }


    public void WriteSerial()
    {
        //Checks whether the serialPort is null.
        if (serialPort == null)
        {
            return;
        }

        if (serialPort.IsOpen)
        {
            string dataString = $"{baseValue:F0}@{upperArmValue:F0}@{lowerArmValue:F0}@{clawValue:F0}!";
            //$"B{baseValue:F0}U{upperArmValue:F0}L{lowerArmValue:F0}C{clawValue:F0}!";
            // Debug.Log("Data String: " + dataString);

            byte[] data = Encoding.ASCII.GetBytes(dataString);
            try
            {
                serialPort.Write(data, 0, data.Length);
                serialPort.BaseStream.Flush();
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex.Message);
            }
        }
    }

    public void ClosePort()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
            readThread.Join();
        }
    }

    private void OnDestroy()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }
}
