using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailWag : MonoBehaviour
{
    public ArticulationBody tailJoint;
    public float duration = 0.5f;

    private Coroutine tailWag;
    void Start()
    {
        ConfigureJointDrive(tailJoint);
    }

    public void StartWagging(float minAngle, float maxAngle)
    {
        tailWag = StartCoroutine(WagTail(minAngle, maxAngle));
    }
    IEnumerator WagTail(float minAngle, float maxAngle)
    {
        yield return TailMove(minAngle, duration);
        yield return TailMove(maxAngle, duration);
    }

    IEnumerator TailMove(float angle, float duration)
    {
        var drive = tailJoint.xDrive;
        drive.target = angle;
        tailJoint.xDrive = drive;
        yield return new WaitForSeconds(duration);
    }
    public void StopWagging()
    {
        if (tailWag != null){
            Debug.Log("Stop wagging tail");
            StopCoroutine(tailWag);
        }
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
