using System;
using System.IO.Ports;
using System.Text;
using UnityEngine;

public class SendToServo : MonoBehaviour
{
    private SerialPort serialPort;
    public ArticulationBody headJoint;
    private float headRot;

    private float lastSentAngle = -1f;  // Stores the last sent rotation
    private float sendThreshold = 1f;   // Minimum change in degrees to trigger a send
    private float sendInterval = 0.1f;  // Minimum time between sends (100ms)
    private float lastSendTime = 0f;    // Tracks the last send time

    private string portName = "COM4";
    private int baudRate = 115200;

    void Start()
    {
        // get head rotation
        headRot = headJoint.transform.rotation.eulerAngles.y;

        serialPort = new SerialPort(portName, baudRate);
        serialPort.Open();
        System.Threading.Thread.Sleep(500);
        // serialPort.DtrEnable = true; // may not need

        Debug.Log("UART client started");
        
    }
    void Update()
    {
        headRot = headJoint.transform.rotation.eulerAngles.y;

        // convert to correct format
        if (headRot > 180)
        {
            headRot = headRot - 360;
        }
        else
        {
            headRot = headRot * -1;
        }
        
        // send if rotation > thresh & certain time has passed
        if (Mathf.Abs(headRot - lastSentAngle) >= sendThreshold && Time.time - lastSendTime >= sendInterval)
        {
            SendData(headRot);
            lastSentAngle = headRot;
            lastSendTime = Time.time;
        }
    }
    private void SendData(float data)
    {
        try
        {
            if (serialPort.IsOpen)
            {
                string message = data.ToString("F2") + "\n";
                serialPort.Write(message);
                Debug.Log($"Sent: {message}");
            }
        
        }
        catch (Exception e)
        {
            Debug.LogError($"Send Error: {e.Message}");
        }
    }
    // void OnApplicationQuit()
    // {
    //     serialPort.Close();  // Close UDP client when application quits
    // }
    }
