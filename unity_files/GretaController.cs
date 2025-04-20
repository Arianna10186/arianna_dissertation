using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Analytics;

public class GretaController : MonoBehaviour
{   
    // scripts to call
    private ObjectAvoidance objectAvoidance;
    private SearchingBehaviour searchBehaviour;
    private ExcitedBehaviour excitedBehaviour;
    private FollowBehaviour followBehaviour;
    private GretaVision gretaVision;
    private ExpressionClient expressionClient;
    private QLearning qLearning;
    private Nuzzle nuzzleAction;
    private ManualControl manualControl;

    public int chosenAction;

    public bool detectPerson = false;
    private bool behaviourRunning;
    private Coroutine start;

    void Start()
    {
        // Greta Vision
        gretaVision = GetComponent<GretaVision>();
        // Expression Client
        expressionClient = GetComponent<ExpressionClient>();
        // Learning preferences
        qLearning = GetComponent<QLearning>();

        // Object Avoidance
        objectAvoidance = GetComponent<ObjectAvoidance>();

        // Start behaviour
        searchBehaviour = GetComponent<SearchingBehaviour>();

        // Triggered if person is detected
        excitedBehaviour = GetComponent<ExcitedBehaviour>();
        followBehaviour = GetComponent<FollowBehaviour>();

        nuzzleAction = GetComponent<Nuzzle>();
        manualControl = GetComponent<ManualControl>();

        behaviourRunning = true; // if any behaviour is running

        start = StartCoroutine(StartBehaviour());
    }
    IEnumerator StartBehaviour()
    { 
        while (true)
        {
            if (detectPerson==true || gretaVision.CamPersonDetected()) // person detected manual trigger
            {
                // start classifying expressions
                // expressionClient.InitExpressionUDP();
                // Debug.Log("Person detected");
                searchBehaviour.StopSearch();
                // detectPerson = true;
                behaviourRunning = true;

                // start learning preferences ///////////
                chosenAction = qLearning.ChooseAction(); // q learning chooses action
                qLearning.ExecuteAction(chosenAction);
                Debug.Log("Chosen Action: " + chosenAction);

                // wait until robot is x distance away from person
                while (followBehaviour.isWalking == true)
                {
                    yield return new WaitUntil(() => !followBehaviour.isWalking);
                }
                
                // get feedback
                float reward = expressionClient.GetReward();

                // update Q-table
                qLearning.UpdateQTable(reward);
                if (qLearning.GoalStateIsReached(qLearning.state))
                {
                    break;
                }/////////////////////

            } else{
                if (followBehaviour.IsFollowing())
                {
                    followBehaviour.StopFollowing();
                }
                behaviourRunning = true;
                if (manualControl.revertControl == false && searchBehaviour.IsSearchRunning() == false && objectAvoidance.avoidanceRunning == false) 
                {
                    searchBehaviour.StartSearch();
                }
                
            }
            // To add: if search, follow, object avoid and interaction are not running the revert to manual control
            if (searchBehaviour.IsSearchRunning() == false && followBehaviour.IsFollowing() == false && objectAvoidance.avoidanceRunning == false && qLearning.interactionRunning == false)
            {
                manualControl.revertControl = true;
            }
            yield return new WaitForSeconds(UnityEngine.Random.Range(2f, 5f));
        }
    }
    public void StopBehaviours()
    {
        // Debug.Log("Stopping Other Behaviours");
        behaviourRunning = false;

        if (searchBehaviour.IsSearchRunning()) // stop searching behaviour if running
        {
            searchBehaviour.StopSearch();
        }
        if (followBehaviour.IsFollowing()) // stop following behaviour if running
        {
            followBehaviour.StopFollowing();
        }
        // excitedBehaviour.StopExcitement();
    }
    public bool AreBehavioursRunning()
    {
        if (searchBehaviour.IsSearchRunning() || excitedBehaviour.IsExcitementRunning() || followBehaviour.IsFollowing())
        {
            behaviourRunning = true;
        }
        else {
            behaviourRunning = false; // no behaviours are running
        }
        return behaviourRunning;
    }
    public bool IsPersonDetected()
    {
        return detectPerson;
    }
    public void ResumeDefaultBehaviour()
    {
        if (!detectPerson && !gretaVision.CamPersonDetected() && !searchBehaviour.IsSearchRunning() && manualControl.revertControl == false)
        {
            Debug.Log("Resume Search Behaviour");
            searchBehaviour.ResumeSearch();
        }        
    }
//     void OnApplicationQuit()
// {
//     if (receiveThread != null && receiveThread.IsAlive)
//     {
//         receiveThread.Abort();
//     }
//     client.Close();
// }
}
