using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchingBehaviour : MonoBehaviour
{
    private AdvancedWalking walkingScript;
    private RandomHeadMovement headScript;
    private Turning turnScript;
    private GretaVision gretaVision;
    private FollowBehaviour followScript;
    private ManualControl manualControl;

    private bool searchRunning;
    private Coroutine search;
    void Start()
    {
        searchRunning = true;
    }
    public void StartSearch()
    {
        Debug.Log("Searching...");
        walkingScript = GetComponent<AdvancedWalking>();
        headScript = GetComponent<RandomHeadMovement>();
        turnScript = GetComponent<Turning>();
        gretaVision = GetComponent<GretaVision>();
        followScript = GetComponent<FollowBehaviour>();
        manualControl = GetComponent<ManualControl>();

        searchRunning = true;
        search = StartCoroutine(SearchBehaviour());
    }
    IEnumerator SearchBehaviour()
    {
        while (true)
        {
            // Debug.Log("search running: " + searchRunning);
            if (!searchRunning){
                //Debug.Log("Search Behaviour Stopped");
                yield return new WaitUntil(() => searchRunning);
            }
            if (followScript.highGait == true)
            {
                walkingScript.StartWalking(true);
            } else {
                walkingScript.StartWalking(false);
            }
            headScript.RandHeadTurn();
            turnScript.Turn(-30f, 30f, 6f, 15f); // turn randomly between -20 and 20 degrees, wait 10 seconds

            // Wait for a short delay to prevent infinite loop crash
            yield return new WaitForSeconds(UnityEngine.Random.Range(2f, 5f));
    }
    }
    public void StopSearch()
    {
        Debug.Log("Stopping Search Behaviour");
        searchRunning = false;
        //Debug.Log(searchRunning);

        headScript.StopHeadTurn();
        // walkingScript.StopWalking();
        turnScript.StopTurning();
    }
    public void ResumeSearch()
    {
        if (searchRunning == false && gretaVision.CamPersonDetected() == false && manualControl.revertControl == false){
            Debug.Log("Resume Search Behaviour"); 
            searchRunning = true;
        }
    }
    public bool IsSearchRunning()
    {
        return searchRunning;
    }
}
