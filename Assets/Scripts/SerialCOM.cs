using UnityEngine;
using System;
using System.Text;
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

        #region Servo Values
            public int S1, S2, S3, S4;
        #endregion

    #endregion

    // // Start is called before the first frame update
    // void Start()
    // {
    //     Open();
    // }

    // Update is called once per frame
    void Update()
    {
        if (isStreaming)
        {
            string value = _ReadSerialPort();
            Debug.Log(value);
            _StringConvert();
        }
    }

    //Opens Serial Port and set the program to read values from it.
    public void Open()
    {
        isStreaming = true;
        sp = new SerialPort(port, baudrate);
        sp.ReadTimeout = 100;
        sp.Open(); //Opens the Serial Port
        Debug.Log("Port connection has been established!");
    }

    //Closes Serial Port
    public void Close()
    {
        sp.Close();
        Debug.Log("Port connection has been disabled !");
    }

    public string _ReadSerialPort(int timeout = 50)
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

    public void _StringConvert()
    {
        string _IncomingValue = _ReadSerialPort();
        StringBuilder[] sb = new StringBuilder[4];
        // string[] _converted = new string[4];
        int i = 0;
        for (int x = 0; x < _IncomingValue.Length; x++)
        {
            if (_IncomingValue[x] == '$')
            {
                continue;
            }

            if (_IncomingValue[x] == '#')
            {
                i++;
                continue;
            }
            if (sb[i] == null)
                sb[i] = new StringBuilder();
            sb[i].Append(_IncomingValue[x]);
        }

        S1 = int.Parse(sb[0].ToString());
        S2 = int.Parse(sb[1].ToString());
        S3 = int.Parse(sb[2].ToString());
        S4 = int.Parse(sb[3].ToString());
    }
}