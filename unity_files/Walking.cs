using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walking : MonoBehaviour
{
    // set knee and hip joints
    public ArticulationBody[] hipJoints;
    public ArticulationBody[] kneeJoints;

    public float[] hipStanceAngles;
    public float[] hipSwingAngles;
    public float[] kneeStanceAngles;
    public float[] kneeSwingAngles;

    public float moveDuration = 2f;
    private float timeElapsed;

    // drive parameters
    public float stiffness = 100f;
    public float damping = 100f;
    public float forceLimit = 100f;

    void Start()
    {
        for (int i = 0; i < hipJoints.Length; i++)
        {
            ConfigureJointDrive(hipJoints[i]);
        }

        for (int i = 0; i < kneeJoints.Length; i++)
        {
            ConfigureJointDrive(kneeJoints[i]);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;
        float cycleTime = timeElapsed % moveDuration;
        float phaseDuration = moveDuration / 4f;

        for (int i = 0; i < hipJoints.Length; i++)
        {
            float legPhaseStart = i * phaseDuration;
            float legPhaseEnd = (i + 1) * phaseDuration;

            if (cycleTime >= legPhaseStart && cycleTime < legPhaseEnd)
            {
                // swing phase for this leg
                float t = (cycleTime - legPhaseStart) / phaseDuration;
                float hipAngle = Mathf.Lerp(hipStanceAngles[i], hipSwingAngles[i], Mathf.Sin(t * Mathf.PI));
                float kneeAngle = Mathf.Lerp(kneeStanceAngles[i], kneeSwingAngles[i], Mathf.Sin(t * Mathf.PI));
                SetJointAngle(hipJoints[i], hipAngle);
                SetJointAngle(kneeJoints[i], kneeAngle);
            } 
    }
    }
    void SetJointAngle(ArticulationBody joint, float targetAngle)
    {
        // set hip joint angle
        // var hipDrive = hipJoint.xDrive;
        // hipDrive.target = hipTarget;
        // hipJoint.xDrive = hipDrive;

        // set knee joint angle
        var drive = joint.xDrive;
        drive.target = targetAngle;
        joint.xDrive = drive;
    }
    void ConfigureJointDrive(ArticulationBody joint)
    {
        var drive = joint.xDrive;
        drive.stiffness = stiffness;
        drive.damping = damping;
        drive.forceLimit = forceLimit;
        joint.xDrive = drive;

        //joint.automaticInertiaTensor = false;
        //joint.inertiaTensor = Vector3.one * 0.01f;
    }
}

