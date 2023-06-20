using UnityEngine;

public class ButtonManagment : MonoBehaviour
{
    public GameObject SaveButton;
    public GameObject LoadButton;
    public ControlledBySlider cbs;
    public void DisableButton()
    {
        //Locks the button during Lerping.
        if (cbs.IsLerping)
        {
            return;
        }
        SaveButton.SetActive(false);
        LoadButton.SetActive(true);
    }

    public void EnableOtherButton()
    {
        //Locks the button during Lerping.
        if (cbs.IsLerping)
        {
            return;
        }
        LoadButton.SetActive(false);
        SaveButton.SetActive(true);
    }
}
