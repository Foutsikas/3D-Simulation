using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.IO.Ports;
using System.Threading;
using System;

public class SerialCOMSliders : MonoBehaviour
{
    public static SerialCOMSliders i;
    public static ControlledBySlider cbs;

    #region Serial Port Communication Initializer
    //variable decleration field

    //Port speed in bps
    public int baudrate = 9600;

    //Serial Port Decleration
    static private SerialPort sp;

    //Thread init
    Thread readThread;

    //boolean for value reading
    static bool isStreaming;

    string incomingValue = null;

    #endregion

    #region UI Slider Declarations
        public Slider S1Slider;
        public Slider S2Slider;
        public Slider S3Slider;
        public Slider S4Slider;
    #endregion

    #region Slider Values
        private float baseValue = cbs.baseRotation;
        private float upperArmRotation = cbs.upperArmRotation;
        private float lowerArmRotation = cbs.lowerArmRotation;
        private float ClawRotLeft = cbs.ClawRotLeft;
        private float ClawRotRight = cbs.ClawRotRight;
    #endregion

    private void Start()
    {
        //Open serial port and start a read thread
        sp = new SerialPort("COM4", baudrate);
        sp.Open();
        sp.ReadTimeout = 1;
        isStreaming = true;
        readThread = new Thread(StreamIn);
        readThread.Start();
    }

    private void StreamIn()
    {
        while (isStreaming)
        {
            try
            {
                incomingValue = sp.ReadLine();
                // Debug.Log("Serial input: " + incomingValue);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e.Message);
            }
        }
    }

    private void Update()
    {
        baseValue = cbs.baseRotation;
        upperArmRotation = cbs.upperArmRotation;
        lowerArmRotation = cbs.lowerArmRotation;
        ClawRotLeft = cbs.ClawRotLeft;
        ClawRotRight = cbs.ClawRotRight;

    if (sp?.IsOpen == true)
    {
        //Send the slider values as 4 different strings
        string baseValueString = baseValue.ToString();
        string upperArmRotationString = upperArmRotation.ToString();
        string lowerArmRotationString = lowerArmRotation.ToString();
        string ClawRotString = (ClawRotLeft + ClawRotRight).ToString();

        //Write the values to the serial port
        sp.WriteLine(baseValueString);
        sp.WriteLine(upperArmRotationString);
        sp.WriteLine(lowerArmRotationString);
        sp.WriteLine(ClawRotString);
    }
    }

    private void OnDestroy()
    {
        isStreaming = false;
        readThread.Join();
        sp.Close();
    }
}