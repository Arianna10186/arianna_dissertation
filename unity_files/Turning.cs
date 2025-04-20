using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turning : MonoBehaviour
{
    public ArticulationBody spineJoint;

    private Coroutine turning;
    void Start()
    {
        ConfigureJointDrive(spineJoint);
    }

    public void Turn(float minAngle, float maxAngle, float waitTimeMin, float waitTimeMax)
    {
        if (turning != null)
        {
            StopCoroutine(turning);
        }
        turning = StartCoroutine(SpineMove(minAngle, maxAngle, waitTimeMin, waitTimeMax));
    }

    IEnumerator SpineMove(float minAngle, float maxAngle, float waitTimeMin, float waitTimeMax)
    {
        while (true)
        {
            
            float targetAngle = Random.Range(minAngle, maxAngle);
            
            float duration = UnityEngine.Random.Range(4.0f, 7.0f);
            float elapsedTime = 0f;
            float startAngle = spineJoint.xDrive.target;

            while (elapsedTime < duration)
            {
                float newAngle = Mathf.Lerp(startAngle, targetAngle, elapsedTime / duration);
                SetJointAngle(spineJoint, newAngle);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            SetJointAngle(spineJoint, targetAngle);

            // wait for x seconds before turning again
            yield return new WaitForSeconds(UnityEngine.Random.Range(waitTimeMin, waitTimeMax));
        }
        
    }
    void SetJointAngle(ArticulationBody joint, float targetAngle)
    {
        var drive = joint.xDrive;
        drive.target = targetAngle;
        joint.xDrive = drive;
    }
    public void StopTurning()
    {
        //Debug.Log("Stop turning");
        StopCoroutine(turning);
        SetJointAngle(spineJoint, 0f);
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
