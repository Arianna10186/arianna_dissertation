using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectAvoidance : MonoBehaviour
{
    private Turning turnScript;
    private AdvancedWalking walkScript;
    private GretaController mainControl;
    private float sensingDist = 10.0f;
    public ArticulationBody spineJoint;
    public bool avoidanceRunning = false;

    void Start()
    {
        turnScript = GetComponent<Turning>(); // replace with pid controller
        walkScript = GetComponent<AdvancedWalking>();
        mainControl = GetComponent<GretaController>();
    }
    void Update()
    {
        DetectObject(); // continuously looking for objects

    }
    void DetectObject()
    {
        Vector3 rayOrigin = spineJoint.worldCenterOfMass + spineJoint.transform.forward * 0.5f + spineJoint.transform.right * 1f;
        Vector3 rayDirection = spineJoint.transform.TransformDirection(Vector3.right);

        Debug.DrawRay(rayOrigin + Vector3.up * 0.5f, rayDirection * sensingDist, Color.red);

        RaycastHit hit;
        // if (Physics.SphereCast(rayOrigin, 10f, rayDirection, out hit, sensingDist))
        if (Physics.Raycast(rayOrigin + Vector3.up * 0.5f, rayDirection, out hit, sensingDist))
        {
            if (hit.transform.CompareTag("object") && mainControl.AreBehavioursRunning())
            {
                Debug.Log("Avoiding object");
                avoidanceRunning = true;
                // turnScript.Turn(-45f, -35f, 0.5f, 1f);
                mainControl.StopBehaviours(); // pause any other behaviour
                // if (mainControl.IsSearchRunning() == false)
                // {
                //     Debug.Log("Search has stopped");
                // }
                if (turnScript != null)
                {
                    // walkScript.StartWalking();
                    turnScript.Turn(45f, 35f, 0.5f, 1f);
                } 
            }
        }
        else
        {
            if (!mainControl.AreBehavioursRunning() && !mainControl.IsPersonDetected())
            {
                // Debug.Log("No object detected");
                mainControl.ResumeDefaultBehaviour();
            }
            

            // resume normal operation once no object is detected
            // if (mainControl.IsSearchRunning() == false)
            // {
            //     Debug.Log("Resume Search Behaviour");
            //     mainControl.StartSearch();
            // }
        }
    //     yield return null;
    }
}
