using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

//  Edit>ProjectSettings>Player>ApiCompatibilityLevel change to .NET 2.0 (not subset)

public class ArduinoInputs : MonoBehaviour
{

    // The serial port
    SerialPort sp;

    [SerializeField]
    int HIDInput = 0;

    // Use this for initialization
    void Start()
    {

        foreach (string str in SerialPort.GetPortNames())
        {
            sp = new SerialPort("\\\\.\\" + str, 9600);
            Debug.Log(string.Format("Existing COM port: {0}", str));
        }

        if (sp != null)
        {
            if (!sp.IsOpen)
            {
                // Setting up the serial port
                sp.Open();
                sp.ReadTimeout = 1;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        InputManager();
        Debug.Log(HIDInput);
    }

    void InputManager()
    {
        if (sp != null)
        {
            // See if its open
            if (sp.IsOpen)
            {
                // Do a try and catch
                try
                {
                    // Print the serial port
                    //print(sp.ReadByte());
                    HIDInput = sp.ReadByte();
                }
                catch (System.Exception)
                {
                }
            }
        }
    }

    public int GetHIDInput()
    {
        return HIDInput;
    }

    private void OnDestroy()
    {
        sp.Close();
    }
}
