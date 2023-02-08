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

    
}