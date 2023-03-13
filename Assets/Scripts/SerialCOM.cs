using UnityEngine;
using System;
using System.Text;
using System.IO.Ports;
using System.Threading;
using System.Management;

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

    // private AutoDetectPORTs DetectPorts;

    #region Servo Values
    public int S1, S2, S3, S4;
    #endregion

    #endregion

    void Awake()
    {
        string arduinoPort = DetectArduinoPort();
        if (arduinoPort == null)
        {
            Debug.LogError("Could not find an Arduino device.");
            return;
        }

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

    void Start()
    {
        Open();
    }

    private string DetectArduinoPort()
    {
        // Query the WMI registry for devices matching the specified name
        string query = "SELECT * FROM Win32_PnPEntity WHERE Name LIKE '%(COM%)' AND Name LIKE '%Arduino%'";
        ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
        ManagementObjectCollection devices = searcher.Get();

        foreach (ManagementObject device in devices)
        {
            string deviceName = (string)device.GetPropertyValue("Name");
            string devicePort = null;

            int startIndex = deviceName.IndexOf("(COM");
            if (startIndex >= 0)
            {
                int endIndex = deviceName.IndexOf(")", startIndex);
                if (endIndex >= 0)
                {
                    devicePort = deviceName.Substring(startIndex + 1, endIndex - startIndex - 1);
                }
            }

            if (devicePort != null)
            {
                return "COM" + devicePort;
            }
        }

        return null;
    }

    //Opens Serial Port and set the program to read values from it.
    public void Open()
    {
        sp.Open();
        readThread = new Thread(Read);
        readThread.Start();
        isStreaming = true;
        // Debug.Log("Is Alive: " + readThread.IsAlive);
        // Debug.Log("Thread State: " + readThread.ThreadState);
        // Debug.Log("Port State: " + isStreaming);
        // Debug.Log("Port connection was established!");
    }

    //Closes Serial Port
    public void Close()
    {
        isStreaming = false;
        sp.Close();
        readThread.Join();
        // Debug.Log("Thread State: " + readThread.ThreadState);
        // Debug.Log("Port State: " + isStreaming);
        Debug.Log("Port was Closed!");
    }

    //If the program terminates unexpectedly, closes the port and switch back to the main thread.
    void OnDestroy()
    {
        isStreaming = false;
        readThread.Join();
        sp.Close();
        // readThread.Abort();
        // Debug.Log("Unexpected Termination, Connection Destroyed");
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
                    // Debug.Log(i.incomingValue);
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
            // Debug.Log("String is Null");
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