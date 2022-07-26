using UnityEngine;

public class Arduino_Controller : MonoBehaviour
{
  public SerialCOM sc;
  private int S1, S2, S3, S4;
  private readonly float desiredDuration = 2.5f;
  private float elapsedTime;

  #region SetUp Variables
    #region Base Variables
      private Quaternion BaseStartRotation;
      private Quaternion BaseEndRotation;
    #endregion

    #region UpperJoint
      private Quaternion UpperJointStartRotation;
      private Quaternion UpperJointEndRotation;
    #endregion
  #endregion

  #region SetUp Parts
  // These slots are where you will plug in the appropriate arm parts into the inspector.
     public Transform robotBase;
    public Transform UpperJoint;
    public Transform LowerJoint;
    public Transform ClawPincherLeft;
    public Transform ClawPincherRight;
  #endregion

  // void Awake()
  // {
  //     BaseStartRotation = robotBase.transform.rotation;
  // }
  void Start()
  {
    _ValueAssignment();
  }
  void Update()
  {
    _VariableAssignment();
    _ValueAssignment();
    _ArmMovement();
  }

  #region Methods
    void _VariableAssignment()
    {
      S1 = sc.S1;
      S2 = sc.S2;
      S3 = sc.S3;
      S4 = sc.S4;
    }
    void _ValueAssignment()
    {

      BaseEndRotation = new Quaternion (0, S1 + 80, 0, 0);//(S1 - 0)/(180-0)*100
      UpperJointEndRotation = new Quaternion (S2, 0, 0, 0);
    }
    void _ArmMovement()
    {
      #region TIME Calculation
        elapsedTime += Time.deltaTime;
        float percentageComplete = elapsedTime / desiredDuration;
      #endregion

      #region Base Movement
        BaseStartRotation = robotBase.localRotation;
        robotBase.transform.rotation = Quaternion.Lerp(BaseStartRotation, BaseEndRotation, percentageComplete);
      #endregion

      #region Upper Arm Movement
        UpperJointStartRotation = UpperJoint.localRotation;
        Debug.Log("Upper Joint Start: " + UpperJointStartRotation + "\\n" + "Upper Joint End: " + UpperJointEndRotation);
        UpperJoint.transform.rotation = Quaternion.Lerp(UpperJointStartRotation, UpperJointEndRotation, percentageComplete);
      #endregion
    }
  #endregion
}