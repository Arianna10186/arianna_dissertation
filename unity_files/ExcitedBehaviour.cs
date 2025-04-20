using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExcitedBehaviour : MonoBehaviour
{
    private Jumping jumpScript;
    private TailWag tailScript;
    private bool excitementRunning = false;
    // private Coroutine excitementCoroutine;
    void Start()
    {
        jumpScript = GetComponent<Jumping>();
        tailScript = GetComponent<TailWag>();
    }
    public void StartExcitement()
    {
        if (!excitementRunning)
        {
            excitementRunning = true;
            // Debug.Log("excitement running: " + excitementRunning);
            jumpScript.StartJumping(); 
            tailScript.StartWagging(-30, 30); 
        }

        //yield return new WaitForSeconds(Random.Range(1f, 3f));
    }
    public void StopExcitement()
    {
        if (excitementRunning)
        {
            excitementRunning = false;
            // Debug.Log("excitement running: " + excitementRunning);
            jumpScript.StopJumping(); 
            tailScript.StopWagging(); 
        }
    }

    public bool IsExcitementRunning()
    {
        return excitementRunning;
    }
}
