using UnityEngine;
using System;
using System.Text;
using System.IO.Ports;
using System.Threading;
using System.Management;
using Microsoft.Win32;
using System.Linq;

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

    void Awake()
    {
        // Find the COM port of the Arduino device
        string arduinoPort = FindArduinoPort();
        if (arduinoPort == null)
        {
            Debug.LogError("Could not find an Arduino device.");
            return;
        }
        // Connect to the Arduino port
        sp = new SerialPort(arduinoPort, baudrate);
        sp = new SerialPort();

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
        ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2",
            "SELECT * FROM Win32_PnPEntity WHERE Caption like '%(COM%'",
                new EnumerationOptions(
                null, System.TimeSpan.MaxValue,
                1, true, false, true,
                true, false, true, true));
        if (searcher.Get() != null)
        {
            try
            {
                foreach (ManagementObject port in searcher.Get())
                {
                    if (port == null)
                    {
                        //Debug.Log("Port is Null");
                        Debug.Log("Port is Null");
                        continue;
                    }
                    object deviceID = port["DeviceID"];
                    Debug.Log("Device ID "+ deviceID);

                    String s_RegPath = "HKEY_LOCAL_MACHINE\\System\\CurrentControlSet\\Enum\\" + deviceID + "\\Device Parameters";
                    String s_PortName = Registry.GetValue(s_RegPath, "PortName", "").ToString();

                    Debug.Log("Port name " + s_PortName);
                    //if (deviceID?.ToString().Contains("USB\\VID_10C4&PID_EA60\\0001") == true)
                    //{
                    //    arduinoPort = port["DeviceID"].ToString();
                    //    Console.WriteLine(arduinoPort);
                    //    break;
                    //}

                    //Console.WriteLine(port);

                }
            }
            catch (ManagementException e)
            {
                Debug.Log("Error!" + e);
            }
        }
        return arduinoPort;
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