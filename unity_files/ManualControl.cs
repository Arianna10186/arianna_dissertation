using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualControl : MonoBehaviour
{
    public bool revertControl = false;
    public bool hasReverted = false;
    private string currentBehaviour = "";
    // behaviours
    public bool Search = false;
    public bool followN = false;
    public bool followH = false;
    public bool Excited = false;
    public bool Nuzzle = false;
    public bool Headbutt = false;

    // scripts to call
    private GretaController mainControl;
    private SearchingBehaviour searchBehaviour;
    private FollowBehaviour followBehaviour;
    private ExcitedBehaviour excitedBehaviour;
    private Nuzzle nuzzleAction;
    private AdvancedWalking walkingScript;
    private Turning turningScript;
    private SearchingBehaviour searchingScript;
    void Start()
    {
        mainControl = GetComponent<GretaController>();
        searchBehaviour = GetComponent<SearchingBehaviour>();
        followBehaviour = GetComponent<FollowBehaviour>();
        excitedBehaviour = GetComponent<ExcitedBehaviour>();
        nuzzleAction = GetComponent<Nuzzle>();

        walkingScript = GetComponent<AdvancedWalking>();
        turningScript = GetComponent<Turning>();
        searchingScript = GetComponent<SearchingBehaviour>();
    }
    void Update()
    {
        if (revertControl == true)
        {
            if (hasReverted == false)
            {
                Debug.Log("Reverting to manual control");

                // stop all behaviours
                searchBehaviour.StopSearch();
                walkingScript.StopWalking();
                followBehaviour.StopFollowing();
                excitedBehaviour.StopExcitement();
                //mainControl.StopBehaviours();
                hasReverted = true;
            }
            // enable walking and turning using the arrow keys
            if (Input.GetKey(KeyCode.UpArrow))
            {
                Debug.Log("Walking Forward");
                walkingScript.StartWalking(false); // walk forward with normal gait
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                Debug.Log("Stop Walking");
                turningScript.StopTurning();
                walkingScript.StopWalking(); // stop walking
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                Debug.Log("Turning Left");
                turningScript.Turn(-20f, -20f, 0.5f, 0.5f);
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                Debug.Log("Turning Right");
                turningScript.Turn(20f, 20f, 0.5f, 0.5f);
            }
            // activate behaviours
            if (Nuzzle == true && currentBehaviour != "Nuzzle")
            {
                StopCurrentBehaviour();
                currentBehaviour = "Nuzzle";
                nuzzleAction.StartNuzzle(3f, 30f, 60f, -20f, 20f);
            }
            else if (Headbutt == true && currentBehaviour != "Headbutt")
            {
                StopCurrentBehaviour();
                currentBehaviour = "Headbutt";
                nuzzleAction.StartNuzzle(2f, 0f, 60f, 0f, 0f);
            }
            else if (Excited == true && currentBehaviour != "Excited")
            {
                StopCurrentBehaviour();
                currentBehaviour = "Excited";
                excitedBehaviour.StartExcitement();
            }
            else if (Search == true && currentBehaviour != "Search")
            {
                StopCurrentBehaviour();
                currentBehaviour = "Search";
                searchBehaviour.StartSearch();
            }
        }
        else
        {
            hasReverted = false;
            currentBehaviour = "";
        }
    }

    void StopCurrentBehaviour()
    {
        if (currentBehaviour == "Nuzzle" || currentBehaviour == "Headbutt")
        {
            nuzzleAction.StopNuzzle();
        } else if (currentBehaviour == "Excited")
        {
            excitedBehaviour.StopExcitement();
        }
        else if (currentBehaviour == "Search")
        {
            searchBehaviour.StopSearch();
        }
        Nuzzle = false;
        Headbutt = false;
        Excited = false;
        Search = false;
    }

}
