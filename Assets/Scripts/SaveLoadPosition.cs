using UnityEngine;

public class SaveLoadPosition : MonoBehaviour
{
    public Transform BaseTransform, LowJointTransform, UpperJointTransform, PincherLeftTransform, PincherRightTransform;
    private Quaternion oldBasePos, oldLowJointPos, oldUpperJointPos, oldPincherLeftPos, oldPincherRightPos,
                        oldBasePos2, oldLowJointPos2, oldUpperJointPos2, oldPincherLeftPos2, oldPincherRightPos2,
                        oldBasePos3, oldLowJointPos3, oldUpperJointPos3, oldPincherLeftPos3, oldPincherRightPos3;

    public void SavePositionButton1()
    {
        oldBasePos = BaseTransform.transform.rotation;
        oldLowJointPos = LowJointTransform.rotation;
        oldUpperJointPos = UpperJointTransform.rotation;
        oldPincherLeftPos = PincherLeftTransform.rotation;
        oldPincherRightPos = PincherRightTransform.rotation;
    }

    public void LoadPositionButton1()
    {
        BaseTransform.transform.rotation = oldBasePos;
        LowJointTransform.transform.rotation = oldLowJointPos;
        UpperJointTransform.transform.rotation = oldUpperJointPos;
        PincherLeftTransform.transform.rotation = oldPincherLeftPos;
        PincherRightTransform.transform.rotation = oldPincherRightPos;
    }

    public void SavePositionButton2()
    {
        oldBasePos2 = BaseTransform.transform.rotation;
        oldLowJointPos2 = LowJointTransform.rotation;
        oldUpperJointPos2 = UpperJointTransform.rotation;
        oldPincherLeftPos2 = PincherLeftTransform.rotation;
        oldPincherRightPos2 = PincherRightTransform.rotation;
    }

    public void LoadPositionButton2()
    {
        BaseTransform.transform.rotation = oldBasePos2;
        LowJointTransform.transform.rotation = oldLowJointPos2;
        UpperJointTransform.transform.rotation = oldUpperJointPos2;
        PincherLeftTransform.transform.rotation = oldPincherLeftPos2;
        PincherRightTransform.transform.rotation = oldPincherRightPos2;
    }

    public void SavePositionButton3()
    {
        oldBasePos3 = BaseTransform.transform.rotation;
        oldLowJointPos3 = LowJointTransform.rotation;
        oldUpperJointPos3 = UpperJointTransform.rotation;
        oldPincherLeftPos3 = PincherLeftTransform.rotation;
        oldPincherRightPos3 = PincherRightTransform.rotation;
    }

    public void LoadPositionButton3()
    {
        BaseTransform.transform.rotation = oldBasePos3;
        LowJointTransform.transform.rotation = oldLowJointPos3;
        UpperJointTransform.transform.rotation = oldUpperJointPos3;
        PincherLeftTransform.transform.rotation = oldPincherLeftPos3;
        PincherRightTransform.transform.rotation = oldPincherRightPos3;
    }
}