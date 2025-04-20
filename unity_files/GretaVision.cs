using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class GretaVision : MonoBehaviour
{
    private UdpClient udpClient;
    private IPEndPoint endPoint;

    // Declare UDP variables
    Thread receiveThread;
    UdpClient client;
    int port;

    // private gretaController mainControl;
    public GameObject user;
    // public ArticulationBody spineJoint;
    public Transform greta;
    private UnityEngine.Vector3 gretaPosition;
    private float gretaRot;

    private UnityEngine.Vector3 UserPosition;
    private bool positionUpdated = false;
    private bool personDetected = false;
    private bool updateDetection = false;

    private float timeout = 1f;
    private float lastRecievedTime = 0f;
    public float positionThreshold = 10f;
    private UnityEngine.Vector3 previousPosition;
    void Start()
    {
        port = 10000;
        InitUDP();

        // mainControl = GetComponent<gretaController>();

        // get spine joint position
        gretaPosition = greta.transform.position;
        gretaRot = greta.transform.eulerAngles.y;
    }
    private void InitUDP()
    {
        print ("UDP Initialized");

        client = new UdpClient(port);

        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }
    void Update()
    {        
        if (positionUpdated)
        {
            // float smoothSpeed = 5f;
            user.transform.position = UnityEngine.Vector3.Lerp(user.transform.position, UserPosition, 1f);
            // user.transform.position = UnityEngine.Vector3.Lerp(user.transform.position, UserPosition, Time.deltaTime * smoothSpeed); // dont think it caused an error but didnt seem to work
            positionUpdated = false; 
        }
        if (updateDetection)
        {
            lastRecievedTime = Time.time;
            personDetected = true;
            updateDetection = false;
        }
        if (Time.time - lastRecievedTime > timeout)
        {
            personDetected = false;
        }
    }
    private void ReceiveData()
    {
        while (true)
        {
            try
            {
                float gretaX = gretaPosition.x;
                float gretaY = gretaPosition.z;

                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, port);

                byte[] data = client.Receive(ref anyIP);

                float x = BitConverter.ToSingle(data, 0);
                float y = BitConverter.ToSingle(data, 4);
                float depth = BitConverter.ToSingle(data, 8);

                // Dummy data
                // float y = 1;
                // float x = 100; // centre of camera
                // float depth = 0.7f; // closeset distance

                // Debug.Log($"Received coordinates: X={x}, Y={y}, Depth={depth}");

                // Convert camera coordinates to world coordinates
                float worldX = depth * 20f; // 20
                float worldY = (x / -32) + 10;
                float worldZ = 2.5f;

                // taking into account the rotation of the robot
                UnityEngine.Vector2 rotatedPos = RotatePoint(new UnityEngine.Vector2(worldX, worldY), gretaRot);
                float userPosX = gretaX + rotatedPos.x;
                float userPosY = gretaY + rotatedPos.y;
                Debug.Log($"Received coordinates: X={userPosX}, Y={userPosY}, Z={worldZ}");

                UnityEngine.Vector3 currentPosition = new UnityEngine.Vector3(userPosX, worldZ, userPosY);
                // check if position change is greater than threshold
                if (UnityEngine.Vector3.Distance(currentPosition, previousPosition) > positionThreshold)
                {
                    // assign coordinates to user position relative  to robot
                    UserPosition = currentPosition;
                    positionUpdated = true;
                    previousPosition = currentPosition;
                }
                updateDetection = true;

                personDetected = true;
                Debug.Log($"Person detected: {personDetected}");

                // Send data
                SendData(userPosX, userPosY, worldZ);

            }
            catch(Exception e)
            {
                print(e.ToString());
            }
        }
    }
    private UnityEngine.Vector2 RotatePoint(UnityEngine.Vector2 point, float angle)
    {
        float angleRad = -angle * Mathf.Deg2Rad;
        float cosTheta = Mathf.Cos(angleRad);
        float sinTheta = Mathf.Sin(angleRad);

        float rotatedX = point.x * cosTheta - point.y * sinTheta;
        float rotatedY = point.x * sinTheta + point.y * cosTheta;

        return new UnityEngine.Vector2(rotatedX, rotatedY);
    }
    private UnityEngine.Vector3 SendData(float x, float y, float z)
    {
        return new UnityEngine.Vector3(x, y, z);
    }
    public bool CamPersonDetected()
    {
        return personDetected;
    }
    // private void OnApplicationQuit()
    // {
    //     StopThreadAndClient();
    // }
    // private void StopThreadAndClient()
    // {
    //     // receiveThread.Abort();
    //     client.Close();
    // }
}
