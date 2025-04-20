using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomHeadMovement : MonoBehaviour
{
    public ArticulationBody headJoint;

    // random speed of movement
    public float minSpeed = 0.1f;
    public float maxSpeed = 1.0f;

    // random angle of movement
    public float minAngle = -45f;
    public float maxAngle = 45f;

    // random duration of sleep between movement
    public float minSleep = 1f;
    public float maxSleep = 7.0f;

    private Coroutine randomHeadMovement;

    void Start()
    {
        ConfigureJointDrive(headJoint);
    }

    public void RandHeadTurn()
    {
        if (randomHeadMovement != null)
        {
            StopCoroutine(randomHeadMovement);
        }
        randomHeadMovement = StartCoroutine(HeadMove());
    }

    IEnumerator HeadMove()
    {
        while (true)
        {
            // generate random parameters
            float speed = Random.Range(minSpeed, maxSpeed);
            float headAngle = Random.Range(minAngle, maxAngle);
            float sleepTime = Random.Range(minSleep, maxSleep);

            // perform random movement
            SetJointSpeed(headJoint, speed);
            SetJointAngle(headJoint, headAngle);

            // wait for a random amount of time
            yield return new WaitForSeconds(sleepTime);
        }
    }
    public void StopHeadTurn()
    {
        if (randomHeadMovement != null){
            StopCoroutine(randomHeadMovement);
            SetJointAngle(headJoint, 0f);
        }
    }
    void SetJointSpeed(ArticulationBody joint, float speed)
    {        
        var drive = joint.xDrive;
        drive.targetVelocity = speed;
        joint.xDrive = drive;        
    }
    void SetJointAngle(ArticulationBody joint, float targetAngle)
    {
        // set  joint angle
        var drive = joint.xDrive;
        drive.target = targetAngle;
        joint.xDrive = drive;
    }
    void ConfigureJointDrive(ArticulationBody joint)
    {
        var drive = joint.xDrive;
        drive.stiffness = 1000f;
        drive.damping = 1000f;
        drive.forceLimit = 100f;
        joint.xDrive = drive;
    }
}
