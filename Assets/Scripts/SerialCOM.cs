using UnityEngine;
using System;
using System.Text;
using System.IO.Ports;
using System.Threading;

public class SerialCOM : MonoBehaviour
{
    public static SerialCOM i;

    #region Serial Port Communication Initializer
        //variable decleration field
        //Port name
        public string port = "COM3";
        //Port speed in bps
        public int baudrate = 9600;

        //Serial Port Decleration
        static private SerialPort sp = new SerialPort("COM3", 9600);

        //Thread init
         Thread readThread = new Thread(Read);

        //boolean for value reading
        static bool isStreaming;

        string incomingValue = null;

        #region Servo Values
            public int S1, S2, S3, S4;
        #endregion

    #endregion

    void Awake()
    {
        if (i == null)
        {
            i = this;
        }
        else
        {
            Destroy(this);
        }
    }

    //Opens Serial Port and set the program to read values from it.
    public void Open()
    {
        isStreaming = true;
        sp.Open(); //Opens the Serial Port
        readThread.Start();
        Debug.Log("Port connection was established!");
    }

    //Closes Serial Port
    public void Close()
    {
        isStreaming = false;
        readThread.Join();
        sp.Close();
        Debug.Log("Port was Closed!");
    }

    //If the program terminates unexpectedly, closes the port and switch back to the main thread.
    void OnDestroy()
    {
        isStreaming = false;
        readThread.Join();
        sp.Close();
        Debug.Log("Unexpected Termination, Connection Destroyed");
    }

    void Update()
    {
        Debug.Log("MAINTHREAD_S1: " + S1 + " ,S2: " + S2 + " ,S3: " + S3 + " ,S4: " + S4);
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
                    // Debug.Log(i.incomingValue);
                }
            }
            catch (TimeoutException) { }
        }
    }

    public void StringConvert(string value)
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

            (sb[i] ?? (sb[i] = new StringBuilder())).Append(value[x]);

            // ^ same thing as below
            // if (sb[i] == null)
            //     sb[i] = new StringBuilder();
            // sb[i].Append(value[x]);
        }

        S1 = int.Parse(sb[0].ToString());
        S2 = int.Parse(sb[1].ToString());
        S3 = int.Parse(sb[2].ToString());
        S4 = int.Parse(sb[3].ToString());

        // Debug.Log("S1: " + S1 + " ,S2: " + S2 + " ,S3: " + S3 + " ,S4: " + S4);
    }
}