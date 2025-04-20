// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Numerics;
// using UnityEngine;

public class PidController
{
    public float P;
    public float I;
    public float D;

    private float previousError = 0f;
    private float integral = 0f;

    public PidController(float P, float I, float D)
    {
        this.P = P;
        this.I = I;
        this.D = D;
    }

    public float FindError(float target, float current, float deltaTime)
    {
        float error = target - current;
        integral += error * deltaTime;
        float derivative = (error - previousError) / deltaTime;
        previousError = error;

        return P * error + I * integral * D * derivative;

    }
}
