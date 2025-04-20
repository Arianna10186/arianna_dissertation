using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControlModeText : MonoBehaviour
{
    public TextMeshProUGUI controlModeText;
    public Toggle toggle;
    public ManualControl manualControl;
    // set buttons
    public void SetSearch() => SetBehaviour("Search");
    public void SetFollowN() => SetBehaviour("FollowN");
    public void SetFollowH() => SetBehaviour("FollowH");
    public void SetExcited() => SetBehaviour("Excited");
    public void SetNuzzle() => SetBehaviour("Nuzzle");
    public void SetHeadbutt() => SetBehaviour("Headbutt");
    void Update()
    {
        if (manualControl == null) return;
        if (manualControl.revertControl == true)
        {
            controlModeText.text = "Control Mode: Manual";
        }
        else
        {
            controlModeText.text = "Control Mode: Automatic";
        }
        // on toggle, activate manual control
        if (toggle.isOn == true)
        {
            manualControl.revertControl = true;
        }
        else
        {
            manualControl.revertControl = false;
        }
    }
    private void SetBehaviour(string behaviour)
    {
        // reset
        manualControl.Search = false;
        manualControl.followN = false;
        manualControl.followH = false;
        manualControl.Excited = false;
        manualControl.Nuzzle = false;
        manualControl.Headbutt = false;

        // set selected behaviour
        switch (behaviour)
        {
            case "Search":
                manualControl.Search = true;
                break;
            case "FollowN":
                manualControl.followN = true;
                break;
            case "FollowH":
                manualControl.followH = true;
                break;
            case "Excited":
                manualControl.Excited = true;
                break;
            case "Nuzzle":
                manualControl.Nuzzle = true;
                break;
            case "Headbutt":
                manualControl.Headbutt = true;
                break;
        }
    }
}
