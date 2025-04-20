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
using System.Text;

public class ExpressionClient : MonoBehaviour
{
    private UdpClient udpClient2;
    private IPEndPoint endPoint2;
    private GretaController gretaController;

    // Declare UDP variables
    Thread receiveThread2;
    UdpClient client2;
    int port2;
    bool classification;
    void Start()
    {
        port2 = 6000;
        InitExpressionUDP();
        gretaController = GetComponent<GretaController>();

    }
    public void InitExpressionUDP()
    {
        print ("Expression UDP Initialized");

        client2 = new UdpClient(port2);

        receiveThread2 = new Thread(new ThreadStart(ReceiveData));
        receiveThread2.IsBackground = true;
        receiveThread2.Start();
    }
    void Update()
    {
        
    }
    private void ReceiveData()
    {
        while (true)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, port2);

                byte[] data = client2.Receive(ref anyIP);
                // string expression = Encoding.UTF8.GetString(data);
                string expression = Encoding.UTF8.GetString(data).Trim('[', '"', ']');


                // Dummy data
                // string expression = "happy";
                string gesture = "neutral";

                //Debug.Log($"Received expression: {expression}, received gesture: {gesture}");

                classification = Classification(expression, gesture);
            }
            catch(Exception e)
            {
                print(e.ToString());
            }
            
        }
    }
    private bool Classification(string expression, string gesture)
    {
        float exp_score = 0.0f;
        float ges_score = 0.0f;
        
        string[] exp_classes = {"neutral", "angry", "disgust", "fear", "happy", "sad", "surprise"};
        float[] exp_class_scores = {0.0f, -1.0f, 0.0f, -3.0f, 3.0f, -1.0f, 1.0f};

        string[] ges_classes = {"neutral", "nodding", "stop"};
        float[] ges_class_scores = {0.0f, 1.0f, -3.0f};

        for (int i = 0; i < exp_classes.Length; i++)
        {
            if (exp_classes[i] == expression)
            {
                exp_score = exp_class_scores[i];
            }
        }
        for (int i = 0; i < ges_classes.Length; i++)
        {
            if (ges_classes[i] == gesture)
            {
                ges_score = ges_class_scores[i];
            }
        }
        float total_score = exp_score + ges_score;

        // when person is detected
        if (gretaController.IsPersonDetected())
        {
            Debug.Log($"Expression: {expression}, Gesture: {gesture}");
            Debug.Log($"Expression score: {exp_score}, Gesture score: {ges_score}, Total score: {total_score}");

            if (total_score < 0.0f)
            {
                Debug.Log("Negative reaction");
            } else
            {
                Debug.Log("Positive reaction");
            }
        }
        if (total_score < 0.0f)
            {
                return false;
            } else
            {
                return true;
            }
        
    }
    public float GetReward()
    {
        float reward = 0.0f;
        if (classification)
        {
            reward = 1.0f;
        } else
        {
            reward = -1.0f;
        }
        return reward;
    }
}
