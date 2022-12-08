using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ButtonManagment : MonoBehaviour
{
    public GameObject SaveButton;
    public GameObject LoadButton;
    public void DisableButton()
    {
        SaveButton.SetActive(false);
        LoadButton.SetActive(true);
    }

    public void EnableOtherButton()
    {
        LoadButton.SetActive(false);
        SaveButton.SetActive(true);
    }
}
