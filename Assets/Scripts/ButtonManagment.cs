using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ButtonManagment : MonoBehaviour
{
    public GameObject SaveButton;
    public GameObject LoadButton;
    public ControlledBySlider cbs;
    public void DisableButton()
    {
        //Locks the button during Lerping.
        if (cbs.isLerping)
        {
            return ;
        }
        SaveButton.SetActive(false);
        LoadButton.SetActive(true);
    }

    public void EnableOtherButton()
    {
        //Locks the button during Lerping.
        if (cbs.isLerping)
        {
            return ;
        }
        LoadButton.SetActive(false);
        SaveButton.SetActive(true);
    }
}
