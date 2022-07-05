using UnityEngine;
using System;
using System.Collections;
using System.IO.Ports;

public class SerialCOM : MonoBehaviour
{
    #region Serial Port Communication Initializer
        //variable decleration field
        //Port name
        public string port = "COM3";
        //Port speed in bps
        public int baudrate = 9600;

        //Serial Port Decleration
        private SerialPort sp;

        //boolean for value reading
        bool isStreaming;

        //Geometry to modify
        public GameObject Robot;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        Open();
    }

    // Update is called once per frame
    void Update()
    {
        if (isStreaming)
        {
            string value = ReadSerialPort();
            if ((value != null) && (float.Parse(value) >= 1.0f))
            {
                Debug.Log(value);
            }
        }
    }

    //Opens Serial Port and set the program to read values from it.
    public void Open()
    {
        isStreaming = true;
        sp = new SerialPort(port, baudrate);
        sp.ReadTimeout = 100;
        sp.Open(); //Opens the Serial Port
        Debug.Log("Port connection was established!");
    }

    //Closes Serial Port
    public void Close()
    {
        sp.Close();
    }

    public string ReadSerialPort(int timeout = 50)
    {
        string message;

        sp.ReadTimeout = timeout;
        //attempt to read values from serial port
        try
        {
            message = sp.ReadLine();
            return message;
        }
        catch(TimeoutException)
        {
            return null;
        }
    }
}