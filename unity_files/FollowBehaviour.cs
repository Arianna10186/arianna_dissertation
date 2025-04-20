using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowBehaviour : MonoBehaviour
{
    private GretaController mainControl;
    private GretaVision gretaVision;
    private AdvancedWalking walkingScript;
    private ExcitedBehaviour excitedScript;
    private ManualControl manualControl;
    // private QLearning qLearning;

    public Transform user;
    public ArticulationBody spineJoint;

    public float stopDistance = 2f;

    public bool highGait;

    private bool isFollowing = false;

    // private Coroutine follow;

    private PidController rotationPID;
    public bool isWalking = false;
    private int frameCount = 0;
    void Start()
    {
        //Debug.Log("Following...");
        isFollowing = false;
        walkingScript = GetComponent<AdvancedWalking>();
        mainControl = GetComponent<GretaController>();
        gretaVision = GetComponent<GretaVision>();
        excitedScript = GetComponent<ExcitedBehaviour>();
        // qLearning = GetComponent<QLearning>();
        manualControl = GetComponent<ManualControl>();

        ConfigureJointDrive(spineJoint);

        rotationPID = new PidController(8f, 0f, 1f);
    }
    void FixedUpdate()
    {
        if (((mainControl.IsPersonDetected() || gretaVision.CamPersonDetected()) && !isFollowing) || (manualControl.revertControl == true && manualControl.followN == true) || (manualControl.revertControl == true && manualControl.followH == true))
        {
            mainControl.StopBehaviours(); // stop any other behaviours
            // Debug.Log("Person detected. Starting Follow Behaviour.");
            // Debug.Log("person detected: " + mainControl.IsPersonDetected() + ", cam person detected: " + gretaVision.CamPersonDetected());
            
            float deltaTime = Time.fixedDeltaTime;
            Vector3 directionToUser = (user.position - transform.position).normalized;
            float distanceToUser = Vector3.Distance(spineJoint.transform.position, user.position);

            //Debug.Log("User position" + user.position + ", Transform position: " + transform.position + ", direction to user: " + directionToUser);

            // Rotate to face user
            float targetRot = Mathf.Atan2(directionToUser.x, directionToUser.z) * Mathf.Rad2Deg * -1;
            float currentRot = spineJoint.transform.eulerAngles.y;

            if (currentRot > 180) currentRot -= 360;
            if (targetRot < 180) targetRot += 90;
            targetRot = targetRot * -1;

            float angleError = targetRot - currentRot;

            if (angleError > 180) angleError -= 360;
            if (angleError < -180) angleError +=360;

            float rotation = rotationPID.FindError(0, angleError, deltaTime);
            rotation = rotation * -1;

            Debug.Log("Target rotation = " + targetRot + ", Current Rotation = " + currentRot + ", Error = "+ angleError);

            ApplyRotation(rotation);

            // Robot walk towards user
                if (distanceToUser > stopDistance) // ensure a bit of distance between user and robot
            {
                frameCount++;
                if (frameCount > 800)
                {
                    Debug.Log("Following... Distance from user = " + distanceToUser);
                }
                if (!isWalking)
                {
                    // different variations based on action selected from q-learning
                    if (mainControl.chosenAction == 0 || manualControl.followN == true) // following
                    {
                        walkingScript.StartWalking(highGait=false);
                        isWalking = true;
                    } 
                    else if (mainControl.chosenAction == 1 || highGait==true || manualControl.followH == true) // following high gait
                    {
                        walkingScript.StartWalking(highGait=true);
                        isWalking = true;
                    }
                    else if (mainControl.chosenAction == 2) // jumping and following
                    {
                        // excitedScript.StartExcitement();
                        walkingScript.StartWalking(highGait=false);
                        isWalking = true;
                    }
                    
                }
            }
            else
            {
                if (isWalking || !gretaVision.CamPersonDetected())
                {
                    walkingScript.StopWalking();                    
                    isWalking = false;
                    isFollowing = true;
                }
            }
        } 
    }
    void ApplyRotation(float command)
    {
        ArticulationDrive drive = spineJoint.xDrive;

        drive.targetVelocity = command; // Rotate towards player
        spineJoint.xDrive = drive;
    }
    public void StopFollowing()
    {
        Debug.Log("Stopping Follow Behaviour");
        isFollowing = false;

        walkingScript.StopWalking();
        ApplyRotation(0); // don't know if this will work
    }
    public bool IsFollowing()
    {
        return isFollowing;
    }
    void ConfigureJointDrive(ArticulationBody joint)
    {
        var drive = joint.xDrive;
        drive.stiffness = 1000f;
        drive.damping = 1000f;
        drive.forceLimit = 1000f;
        joint.xDrive = drive;
    }
   }
