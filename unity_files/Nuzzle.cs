using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nuzzle : MonoBehaviour
{
    public ArticulationBody neckJoint;
    public ArticulationBody headJoint;
    public float speed = 1f;
    // angle of movement range
    public float minAngle = -30f;
    public float maxAngle = 30f;
    public float minAngle2 = -20f;
    public float maxAngle2 = 20f;

    // private float moveDuration = 2f;
    private float timeElapsed;
    private Coroutine nuzzleAction;
    void Start()
    {
        ConfigureJointDrive(neckJoint);
        ConfigureJointDrive(headJoint);
    }
    public void StartNuzzle(float duration, float minAngle, float maxAngle, float minAngle2, float maxAngle2)
    {
        if (nuzzleAction != null)
        {
            StopCoroutine(nuzzleAction);
        }
        nuzzleAction = StartCoroutine(Nuzzling(duration, minAngle, maxAngle, minAngle2, maxAngle2));
    }
    IEnumerator Nuzzling(float moveDuration, float minAngle, float maxAngle, float minAngle2, float maxAngle2)
    {
        while (true)
        {
            timeElapsed += Time.deltaTime;
            float cycleTime = timeElapsed % moveDuration;
            float phaseDuration = moveDuration / 2f;

            float t = cycleTime / phaseDuration;
            float angle = Mathf.Lerp(minAngle, maxAngle, Mathf.Sin(t * Mathf.PI));
            float angle2 = Mathf.Lerp(minAngle2, maxAngle2, Mathf.Sin(t * Mathf.PI));

            SetJointSpeed(neckJoint, speed);
            SetJointAngle(neckJoint, angle);

            yield return null;
        }
    }
    public void StopNuzzle()
    {
        if (nuzzleAction != null){
            StopCoroutine(nuzzleAction);
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
