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
    public Slider baseSlider;
    public Slider upperArmSlider;
    public Slider lowerArmSlider;
    public Slider clawSlider;
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
        readThread = new Thread(ReadSerialPort);
        readThread.Start();
        //isStreaming = true;
        var x = serialPort.IsOpen;
        Debug.Log("Serial Port Is Open: " + x);
    }

    private void ReadSerialPort()
    {
        while (serialPort.IsOpen)
        {
            try
            {
                if (serialPort.BytesToRead > 0)
                {
                    byte[] incomingValue = new byte[serialPort.BytesToRead];
                    serialPort.Read(incomingValue, 0, incomingValue.Length);
                    Debug.Log($"Serial input: {Encoding.ASCII.GetString(incomingValue)}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex.Message);
            }
        }
    }

    #region UI Slider Controls
    private void Update()
    {
        //Checks whether the serialPort is null.
        if (serialPort == null)
        {
            return;
        }

        float baseSliderValue = baseSlider.value;
        float upperArmSliderValue = upperArmSlider.value;
        float lowerArmSliderValue = lowerArmSlider.value;
        float clawSliderValue = clawSlider.value;
        string dataString = $"^{baseSliderValue}@{upperArmSliderValue}@{lowerArmSliderValue}@{clawSliderValue}^";
        Debug.Log("Data String: " + dataString);
        Debug.Log("Base Rotation: " + Mathf.Clamp(baseSliderValue, -80, 80));

        byte[] data = Encoding.Default.GetBytes(dataString);

        serialPort.Write(data, 0, data.Length);
    }

    private void WriteSerial(string data)
    {
        if (serialPort.IsOpen)
        {
            try
            {
                serialPort.WriteLine(data);
                serialPort.BaseStream.Flush();
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex.Message);
            }
        }
    }
    #endregion

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