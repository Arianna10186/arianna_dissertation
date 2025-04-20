using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Jumping : MonoBehaviour
{
    public ArticulationBody spineJoint;
    public ArticulationBody[] hipJoints;
    public ArticulationBody[] kneeJoints;

    //public float spineRotAngle;
    public float[] hipCrouchAngles;
    public float[] hipExtendAngles;
    public float[] kneeCrouchAngles;
    public float[] kneeExtendAngles;
    public float[] kneeTuckAngles;
    private float[] originalAngles = { 0f, 0f, 0f, 0f };

    public float crouchDuration = 0.3f;
    public float pushDuration = 0.2f;
    public float tuckDuration = 0.2f;
    public float landDuration = 0.3f;

    public float jumpForce = 5f;

    private int jumpCount = 0;
    private int maxJumps = 3;

    private Coroutine jumping;
    void Start()
    {
        ConfigureJointDrive(spineJoint);

        for (int i = 0; i < hipJoints.Length; i++)
        {
            ConfigureJointDrive(hipJoints[i]);
        }

        for (int i = 0; i < kneeJoints.Length; i++)
        {
            ConfigureJointDrive(kneeJoints[i]);
        }
    }
    public void StartJumping()
    {
        // if (jumping != null)
        // {
        //     StopCoroutine(jumping);
        // }
        jumpCount = 0;
        jumping = StartCoroutine(Jump());
        // if (jumping == null)
        // {
        //     jumpCount = 0;
        //     jumping = StartCoroutine(Jump());
        // }
    }
    IEnumerator Jump()
    {
        while (jumpCount < maxJumps)
        {
            jumpCount++;

            // crouch
            yield return MoveJoints(hipJoints, kneeJoints, hipCrouchAngles, kneeCrouchAngles, crouchDuration); // Crouch
            // push off
            yield return MoveJoints(hipJoints, kneeJoints, hipExtendAngles, kneeExtendAngles, pushDuration); // Push Off
            spineJoint.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            // tuck
            yield return MoveJoints(hipJoints, kneeJoints, hipExtendAngles, kneeTuckAngles, tuckDuration); // Mid-Air Tuck
            // land
            yield return MoveJoints(hipJoints, kneeJoints, hipCrouchAngles, kneeCrouchAngles, landDuration); // Landing Prep
            // set to original position
            yield return MoveJoints(hipJoints, kneeJoints, originalAngles, originalAngles, landDuration);
            yield return new WaitForSeconds(1f);
        }
        // Debug.Log("Jumping complete");
        //StopCoroutine(jumping);
        //jumping = null;
        
    }
    IEnumerator MoveJoints(ArticulationBody[] hips, ArticulationBody[] knees, float[] hipTargets, float[] kneeTargets, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            t = Mathf.Sin(t * Mathf.PI * 0.5f);

            for (int i = 0; i < hips.Length; i++)
            {
                SetJointAngleX(hips[i], Mathf.Lerp(hips[i].xDrive.target, hipTargets[i], t));
                SetJointAngleX(knees[i], Mathf.Lerp(knees[i].xDrive.target, kneeTargets[i], t));
            }
            yield return null;
        }   
    }
    public void StopJumping()
    {
        if (jumping != null){
            // Debug.Log("Stop jumping");
            StopCoroutine(jumping);
        }
    }
    void SetJointAngleX(ArticulationBody joint, float targetAngle)
    {
        // set  joint angle
        var drive = joint.xDrive;
        drive.target = targetAngle;
        joint.xDrive = drive;
    }
    void SetJointAngleY(ArticulationBody joint, float targetAngle)
    {
        // set  joint angle
        var drive = joint.yDrive;
        drive.target = targetAngle;
        joint.yDrive = drive;
    }
    void ConfigureJointDrive(ArticulationBody joint)
    {
        var drive = joint.xDrive;
        drive.stiffness = 1000f;
        drive.damping = 1000f;
        drive.forceLimit = 100f;
        joint.xDrive = drive;

        var ydrive = joint.yDrive;
        ydrive.stiffness = 1000f;
        ydrive.damping = 1000f;
        ydrive.forceLimit = 100f;
        joint.yDrive = ydrive;
    }
}
