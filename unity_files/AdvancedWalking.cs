using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedWalking : MonoBehaviour
{
    // set knee and hip joints
    public ArticulationBody[] hipJoints;
    public ArticulationBody[] kneeJoints;
    public ArticulationBody spineJoint;
    public Transform greta;

    private float[] hipStanceAngles;
    private float[] hipSwingAngles;
    private float[] kneeStanceAngles;
    private float[] kneeSwingAngles;

    private float moveDuration = 3f;
    float lastCycleTime = -1f;
    private bool hasJumped = false;
    private float timeElapsed;

    // drive parameters
    public float stiffness = 1000f;
    public float damping = 1000f;
    public float forceLimit = 1000f;
    private Coroutine walkingCoroutine;
    private FollowBehaviour followScript;
    private ManualControl manualControl;
    private GretaController mainControl;

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
        ConfigureJointDrive(spineJoint);
    }

    public void StartWalking(bool highGait)
    {
        followScript = GetComponent<FollowBehaviour>();
        manualControl = GetComponent<ManualControl>();
        mainControl = GetComponent<GretaController>();
        if (walkingCoroutine != null)
        {
            StopCoroutine(walkingCoroutine);
        }
        walkingCoroutine = StartCoroutine(Walking());
    }
    IEnumerator Walking()
    {
        while (true)
        {
            if (followScript.highGait)
            {
                hipSwingAngles = new float[] { -55f, -55f, -90f, -90f }; 
                hipStanceAngles = new float[] { 0f, 0f, 0f, 0f }; 
                kneeSwingAngles = new float[] { 60f, 60f, 120f, 120f }; 
                kneeStanceAngles = new float[] { 5f, 5f, 25f, 25f }; 
                moveDuration = 1.5f;
            }
            else
            {
                hipSwingAngles = new float[] { -70f, -70f, -100f, -100f }; 
                hipStanceAngles = new float[] { 0f, 0f, 0f, 0f }; 
                kneeSwingAngles = new float[] { 60f, 60f, 120f, 120f }; 
                kneeStanceAngles = new float[] { 0f, 0f, 20f, 20f }; 
                moveDuration = 3f;
            }
            // get rotation of the spine joint
            float spineAngle = spineJoint.transform.eulerAngles.y;
            float globalAngle = greta.transform.eulerAngles.y;

            // convert spine into correct format
            if (spineAngle > 180)
            {
                spineAngle = (spineAngle - 360) * -1;
            }
            else
            {
                spineAngle = spineAngle * -1;
            }
            // Debug.Log("Spine angle: " + spineAngle + " Global angle: " + globalAngle + " Difference: " + (globalAngle + spineAngle));

            float rotationFactor = spineAngle / 30f;

            // adapt hip and knee swing angles based on the rotation of the spine joint
            // if (globalAngle + spineAngle > 30 && globalAngle + spineAngle < 90 && increase == false)
            // {
            //     hipSwingAngles[1] = hipSwingAngles[1] * 1.1f; // fl leg
            //     hipSwingAngles[2] = hipSwingAngles[2] * 1.1f; // bl leg
            //     kneeSwingAngles[1] = kneeSwingAngles[1] * 1.1f; 
            //     kneeSwingAngles[2] = kneeSwingAngles[2] * 1.1f; 

            //     increase = true;
            //     decrease = false;
            // } 
            // else if (globalAngle + spineAngle < -30 && globalAngle + spineAngle > -90 && decrease == false)
            // {
            //     hipSwingAngles[0] = hipSwingAngles[0] * 1.1f; // fr leg
            //     hipSwingAngles[3] = hipSwingAngles[3] * 1.1f; // br leg
            //     kneeSwingAngles[0] = kneeSwingAngles[0] * 1.1f; 
            //     kneeSwingAngles[3] = kneeSwingAngles[3] * 1.1f;

            //     increase = false;
            //     decrease = true;
            // }
            timeElapsed += Time.deltaTime;
            float cycleTime = timeElapsed % moveDuration;
            float phaseDuration = moveDuration / 2f;
            int currentPhase = (cycleTime < phaseDuration ? 0 : 1);
            if (currentPhase != lastCycleTime)
            {
                hasJumped = false;
                lastCycleTime = currentPhase;
            }

            for (int i = 0; i < hipJoints.Length; i++)
            {
                int pairedIndex = (i + 2) % hipJoints.Length; // e.g pair 1st and 3rd leg

                float legPhaseStart = (i % 2) * phaseDuration;
                float legPhaseEnd = legPhaseStart + phaseDuration;

                if (cycleTime >= legPhaseStart && cycleTime < legPhaseEnd)
                {
                    // swing phase for this leg
                    float t = (cycleTime - legPhaseStart) / phaseDuration;

                    float swingAdjustment = (i % 2 == 1 ? -rotationFactor : rotationFactor) * 10f;

                    float hipAngle = Mathf.Lerp(hipStanceAngles[i], hipSwingAngles[i], Mathf.Sin(t * Mathf.PI));
                    float kneeAngle = Mathf.Lerp(kneeStanceAngles[i], kneeSwingAngles[i], Mathf.Sin(t * Mathf.PI));

                    SetJointAngle(hipJoints[i], hipAngle);
                    SetJointAngle(kneeJoints[i], kneeAngle);
                    if (followScript.highGait || manualControl.followH || mainControl.chosenAction == 1)
                    {
                        if (!hasJumped)
                        {
                            hasJumped = true;
                            spineJoint.AddForce(Vector3.up * 25f, ForceMode.Impulse);
                        }
                    }
                }
        }
            // rotate spine joint slightly to simulate forward motion
            // float spineAngle = spineRotationAngle * Mathf.Sin((timeElapsed / moveDuration) * Mathf.PI * 2);
            // SetJointAngle(spineJoint, 0f);

            yield return null;
        }
    }
    public void StopWalking()
    {
        if (walkingCoroutine != null){
            StopCoroutine(walkingCoroutine);
            // set joints to stance angles
            for (int i = 0; i < hipJoints.Length; i++)
            {
                SetJointAngle(hipJoints[i], hipStanceAngles[i]);
                SetJointAngle(kneeJoints[i], kneeStanceAngles[i]);
            }
        }
    }
    void SetJointAngle(ArticulationBody joint, float targetAngle)
    {
        // set  joint angle
        var drive = joint.xDrive;
        drive.target = targetAngle;
        // drive.target = 0f;
        joint.xDrive = drive;

        // set spine y angle to 0
        // var ydrive = joint.yDrive;
        // ydrive.target = 0f;
        // joint.yDrive = ydrive;

        // // set spine z angle to 0
        // var zdrive = joint.zDrive;
        // zdrive.target = 0f;
        // joint.zDrive = zdrive;
    }
    void ConfigureJointDrive(ArticulationBody joint)
    {
        var drive = joint.xDrive;
        drive.stiffness = stiffness;
        drive.damping = damping;
        drive.forceLimit = forceLimit;
        joint.xDrive = drive;

        var ydrive = joint.yDrive;
        ydrive.stiffness = stiffness;
        ydrive.damping = damping;
        ydrive.forceLimit = forceLimit;
        joint.yDrive = ydrive;

        var zdrive = joint.zDrive;
        zdrive.stiffness = stiffness;
        zdrive.damping = damping;
        zdrive.forceLimit = forceLimit;
        // joint.zDrive = zdrive;
    }
}

