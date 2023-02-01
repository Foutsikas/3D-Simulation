using UnityEngine;
using System.IO.Ports;
using System.Threading;
using System.Text;
using System;

public class SerialCOM : MonoBehaviour
{
    public int baudrate = 9600;
    public int S1, S2, S3, S4;

    private static SerialCOM instance;
    private SerialPort sp;
    private Thread readThread;
    private bool isStreaming;
    private string incomingValue = null;

    private void Awake()
    {
        instance = this;
        sp = new SerialPort("COM4", 9600);
    }

    public void Open()
    {
        sp.Open();
        readThread = new Thread(Read);
        readThread.Start();
        isStreaming = true;
    }

    public void Close()
    {
        isStreaming = false;
        sp.Close();
        readThread.Join();
    }

    private void OnDestroy()
    {
        isStreaming = false;
        readThread.Join();
        sp.Close();
    }

    private static void Read()
    {
        while (instance.isStreaming)
        {
            try
            {
                string temp = instance.incomingValue;
                instance.incomingValue = instance.sp.ReadLine();
                if (temp != instance.incomingValue)
                {
                    instance.StringConvert(instance.incomingValue);
                }
            }
            catch (TimeoutException) { }
        }
    }

    private void StringConvert(string value)
    {
        if (string.IsNullOrEmpty(value)) return;

        string[] values = new string[4];
        int i = 0;
        StringBuilder sb = new StringBuilder();

        foreach (char c in value)
        {
            if (c == '$') continue;
            if (c == '#')
            {
                values[i++] = sb.ToString();
                sb.Clear();
                continue;
            }

            sb.Append(c);
        }

        S1 = int.Parse(values[0]);
        S2 = int.Parse(values[1]);
        S3 = int.Parse(values[2]);
        S4 = int.Parse(values[3]);
    }
}