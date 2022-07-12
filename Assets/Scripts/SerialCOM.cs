using UnityEngine;
using System;
using System.Collections;
using System.Text;
using System.IO.Ports;
using Unity.Jobs;

public class SerialCOM : MonoBehaviour
{
    #region Serial Port Communication Initializer
        //variable decleration field
        //Port name
        public string port = "COM3";
        //Port speed in bps
        public int baudrate = 9600;

        //Serial Port Decleration
        private SerialPort sp = new SerialPort ("COM3", 9600);

        //boolean for value reading
        bool isStreaming;

        string value;

        #region Servo Values
            public int S1, S2, S3, S4;
        #endregion

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
            StartCoroutine(WaitForFeedback());
            // value = ReadSerialPort();
            _StringConvert();
            // if (!String.IsNullOrWhiteSpace(value))
            // {
            //     Debug.Log(value);
            //     _StringConvert();
            // }
        }
    }

    //Opens Serial Port and set the program to read values from it.
    public void Open()
    {
        isStreaming = true;
        sp = new SerialPort(port, baudrate);
        sp.ReadTimeout = 5000;
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
        sp.ReadTimeout = timeout;
        //attempt to read values from serial port
        try
        {
           value = sp.ReadLine();
            return value;
        }
        catch(TimeoutException)
        {
            return null;
        }
    }

    public void _StringConvert()
    {
        if (value == null)
        {
            Debug.Log("String is Null");
            return;
        }

        StringBuilder[] sb = new StringBuilder[4];
        // string[] _converted = new string[4];
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
            if (sb[i] == null)
                sb[i] = new StringBuilder();
            sb[i].Append(value[x]);
        }

        S1 = int.Parse(sb[0].ToString());
        S2 = int.Parse(sb[1].ToString());
        S3 = int.Parse(sb[2].ToString());
        S4 = int.Parse(sb[3].ToString());

        Debug.Log("S1: " + S1 + " ,S2: " + S2 + " ,S3: " + S3 + " ,S4: " + S4);
    }


    private IEnumerator WaitForFeedback()
    {
        yield return new WaitForSeconds(5f);
        ReadSerialPort();
    }

    public struct MessageReceiver: IJob
    {
        
        public void Execute()
        {

        }
    }
}
