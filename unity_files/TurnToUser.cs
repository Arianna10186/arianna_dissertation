using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnToUser : MonoBehaviour
{
    public GameObject user;
    public ArticulationBody head;
    public ArticulationBody spine;

    private float headRotationY;
    private float spineRotationY;
    private float headAngle;

    private bool hasMoved = false;

    // private Coroutine turnToUser;
    void Start()
    {
        ConfigureJointDrive(spine);
        ConfigureJointDrive(head);
    }
    public void StartTurn()
    {
        if (hasMoved) return; // prevent from turning repeatedly

        hasMoved = true;

        // get head and spine roatation
        Quaternion headRotation = head.transform.rotation;
        Quaternion spineRotation = spine.transform.rotation;

        // rotation in z in degrees
        headRotationY = (headRotation.eulerAngles.y - 180) * -1;
        float totalRotation = headRotationY;
        spineRotationY = spineRotation.eulerAngles.y;

        if (spineRotationY > 180)
        {
            spineRotationY = (spineRotationY - 360) * -1;
        }
        else {
            spineRotationY = spineRotationY * -1;
        }
        // head angle
        headAngle = totalRotation - spineRotationY;
        Debug.Log("Spine Rotation: "+ spineRotationY +" Head Rotation: "+ headAngle +" Total Rotation: "+ totalRotation);        

        MoveSpine();

        // wait for x seconds before turning again
        // yield return new WaitForSeconds(UnityEngine.Random.Range(1, 3));
    }        
    void MoveSpine()
    {
        Vector3 headPosition = head.transform.position; // get coordinate of robot head
        Vector3 userPosition = user.transform.position; // get coordinate of the user
        Vector3 spinePosition = spine.transform.position; // get coordinate of robot spine

        // calculate distance between head and user
        float a = Vector3.Distance(headPosition, userPosition);
        // Debug.Log("Distance between head and user: " + distance);

        // calculate distance between spine and user
        float b = Vector3.Distance(spinePosition, userPosition);
        // Debug.Log("Distance between spine and user: " + distance);
        
        float beta = 180 - headAngle;
        float betaRad = beta * Mathf.Deg2Rad;

        // find angle from spine to user
        float alphaRad = Mathf.Asin(a / b * Mathf.Sin(betaRad));
        float alpha = alphaRad * Mathf.Rad2Deg;

        float targetAngle = 0; // default angle

        // angle the spine needs to rotate depends on if rotation is positive or negative
    if (spineRotationY < 0 && headAngle < 0) // move by alpha anticlockwise
    {
        targetAngle = alpha + spineRotationY; // should both be a minus
    }
    else if (spineRotationY > 0 && headAngle > 0) // move by alpha clockwise
    {
        targetAngle = alpha + spineRotationY; // should both be a positive
    } 
    else if (spineRotationY < 0 && headAngle > 0) // move by alpha - spineAngle clockwise
    {
        targetAngle = (spineRotationY * -1) + alpha;
    } 
    else if (spineRotationY > 0 && headAngle < 0) // move by alpha anticlockwise
    {
        targetAngle = (spineRotationY * -1) + alpha;
    }
    // rotate the spine
    RotateToUser(spine, targetAngle);
    // rotate head to origin
    RotateToUser(head, 0);

    Debug.Log("Spine moves to " + targetAngle + " degrees");
    }
    public void ResetTurn() // if user is not detected
    {
        hasMoved = false;
    }

    void RotateToUser(ArticulationBody joint, float targetAngle)
    {
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
