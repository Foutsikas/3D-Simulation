using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.IO.Ports;
using System.Threading;
using System;

public class SerialCOMSliders : MonoBehaviour
{
    private static SerialCOMSliders instance;
    public static SerialCOMSliders Instance { get { return instance; } }

    #region Serial Port Communication Initializer
    private SerialPort sp;
    private readonly object lockObject = new object();
    private Thread readThread;
    private bool isStreaming;

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
        sp = new SerialPort(config.PortName, config.BaudRate);
        sp.Open();
        sp.ReadTimeout = 1;
        readThread = new Thread(StreamIn);
        readThread.Start();
        isStreaming = true;
    }

    private void StreamIn()
    {
        while (isStreaming)
        {
            try
            {
                if (sp.BytesToRead > 0)
                {
                    byte[] incomingValue;
                    lock (lockObject)
                    {
                        incomingValue = new byte[sp.BytesToRead];
                        sp.Read(incomingValue, 0, incomingValue.Length);
                    }
                    Debug.Log("Serial input: " + Encoding.ASCII.GetString(incomingValue));
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
        float baseSliderValue = baseSlider.value;
        float upperArmSliderValue = upperArmSlider.value;
        float lowerArmSliderValue = lowerArmSlider.value;
        float clawSliderValue = clawSlider.value;
        string dataString = "^" + baseSliderValue + "@" + upperArmSliderValue +
                            "@" + lowerArmSliderValue + "@" + clawSliderValue + "^";

        byte[] data = Encoding.Default.GetBytes(dataString);

        sp.Write(data, 0, data.Length);
    }

    private void WriteSerial(string data)
    {
        if (sp.IsOpen)
        {
            try
            {
                sp.WriteLine(data);
                sp.BaseStream.Flush();
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
        isStreaming = false;
        readThread.Join();
        if (sp.IsOpen)
        {
            sp.Close();
        }
    }
}