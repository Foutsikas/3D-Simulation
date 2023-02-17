using UnityEngine;
using UnityEngine.UI;
using System.IO.Ports;
using System.Threading;
using System;
using System.Text;

public class SerialCOMSliders : MonoBehaviour
{
    private static SerialCOMSliders instance;
    public static SerialCOMSliders Instance { get { return instance; } }

    #region Serial Port Communication Initializer
        private SerialPort serialPort;
        // private readonly object lockObject = new object();
        private Thread readThread;
        //private bool isStreaming;

        private struct SerialPortConfig
        {
            public string PortName;
            public int BaudRate;
        }
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
        SerialPortConfig portConfig = new SerialPortConfig
        {
            PortName = "COM4",
            BaudRate = 9600
        };

        OpenSerialPort(portConfig);
    }

    private void OpenSerialPort(SerialPortConfig config)
    {
        serialPort = new SerialPort(config.PortName, config.BaudRate);
        serialPort.ReadTimeout = 1;
        serialPort.Open();
        var x = serialPort.IsOpen;
        Debug.Log("Serial Port Is Open: " + x);
    }

    // private void ReadSerialPort()
    // {
    //     while (serialPort.IsOpen)
    //     {
    //         try
    //         {
    //             if (serialPort.BytesToRead > 0)
    //             {
    //                 byte[] incomingValue = new byte[serialPort.BytesToRead];
    //                 serialPort.Read(incomingValue, 0, incomingValue.Length);
    //                 Debug.Log($"Serial input: {Encoding.ASCII.GetString(incomingValue)}");
    //             }
    //         }
    //         catch (Exception ex)
    //         {
    //             Debug.LogWarning(ex.Message);
    //         }
    //     }
    // }

    public void WriteSerial()
    {
        //Checks whether the serialPort is null.
        if (serialPort == null)
        {
            return;
        }

        if (serialPort.IsOpen)
        {
            string dataString = $"B{baseValue:F0}U{upperArmValue:F0}L{lowerArmValue:F0}C{clawValue:F0}!";
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

    private void OnDestroy()
    {
        // isStreaming = false;
        readThread.Join();
        if (serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }
}