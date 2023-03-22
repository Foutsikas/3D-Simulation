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

        try
        {
            System.Diagnostics.Process process = new();
            process.StartInfo.FileName = "SystemManagement.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            string fullPath = Application.dataPath + "/SystemManagement.exe";
            System.Diagnostics.ProcessStartInfo startInfo = new(fullPath);
            System.Diagnostics.Process.Start(startInfo);

            string output = process.StandardOutput.ReadToEnd();

            process.WaitForExit();

            string[] lines = output.Split('\n');

            foreach (string line in lines)
            {
                if (line.StartsWith("Port name"))
                {
                    arduinoPort = line.Substring(line.IndexOf("COM"));
                    break;
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("Error: " + e.Message);
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