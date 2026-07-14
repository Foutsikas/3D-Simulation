using UnityEngine;

public class ControlledByArduino : MonoBehaviour
{
    public SerialCOM sc;
    private int S1, S2, S3, S4;

    #region Robot Components' Transforms
    public Transform robotBase;
    public Transform UpperJoint;
    public Transform LowerJoint;
    public Transform ClawPincherLeft;
    public Transform ClawPincherRight;
    #endregion

    #region Calibration (τιμές ευθυγραμμισμένες με το R3_Simulation v2 firmware)
    [Header("Firmware rest angles (pos*_initial)")]
    [Tooltip("pos1_initial στο firmware")] public int baseRestAngle = 90;
    [Tooltip("pos3_initial στο firmware")] public int lowerRestAngle = 80;

    [Header("Visual limits (μοίρες απόκλισης από τη στάση ηρεμίας)")]
    // Firmware: pos1 45..135, rest 90  ->  -45..+45
    public float baseMin = -45f, baseMax = 45f;
    // Firmware: pos2 0..80, rest 0  ->  το μοντέλο γέρνει προς τα αρνητικά
    public float upperMin = -80f, upperMax = 0f;
    // Firmware: pos3 35..145, rest 80  ->  -45..+65
    public float lowerMin = -45f, lowerMax = 65f;
    // Firmware: pos4 0..107 -> οπτικό άνοιγμα δαγκάνας 0..50 μοιρών
    public float clawVisualMax = 50f;
    private const float clawFirmwareMax = 107f;
    #endregion

    private readonly float lerpTime = 1.5f;

    // Αρχική τοπική στάση κάθε μέλους (από το prefab), ως βάση αναφοράς.
    private Quaternion baseRest, upperRest, lowerRest, leftClawRest, rightClawRest;

    void Start()
    {
        baseRest = robotBase.localRotation;
        upperRest = UpperJoint.localRotation;
        lowerRest = LowerJoint.localRotation;
        leftClawRest = ClawPincherLeft.localRotation;
        rightClawRest = ClawPincherRight.localRotation;
    }

    void Update()
    {
        ValueAssignment();
        Movement();
    }

    void ValueAssignment()
    {
        S1 = sc.S1;
        S2 = sc.S2;
        S3 = sc.S3;
        S4 = sc.S4;
    }

    void Movement()
    {
        float t = Time.deltaTime * lerpTime;

        // Βάση: περιστροφή γύρω από τον τοπικό άξονα Z.
        // Στο rest (S1 = 90) η απόκλιση είναι 0 και το μοντέλο ταυτίζεται
        // με τη φυσική στάση εκκίνησης του ρομπότ.
        float baseDelta = Mathf.Clamp(-(S1 - baseRestAngle), baseMin, baseMax);
        robotBase.localRotation = Quaternion.Slerp(
            robotBase.localRotation,
            baseRest * Quaternion.Euler(0f, 0f, baseDelta),
            t);

        // Πάνω βραχίονας: τοπικός άξονας X. Rest στο S2 = 0.
        float upperDelta = Mathf.Clamp(-S2, upperMin, upperMax);
        UpperJoint.localRotation = Quaternion.Slerp(
            UpperJoint.localRotation,
            upperRest * Quaternion.Euler(upperDelta, 0f, 0f),
            t);

        // Κάτω βραχίονας: τοπικός άξονας X. Rest στο S3 = 80
        // (το παλιό "S3 - 129" ήταν βαθμονομημένο στο καταργημένο
        // pos3_initial = 129 και κολλούσε το μπράτσο στο clamp στο boot).
        float lowerDelta = Mathf.Clamp(S3 - lowerRestAngle, lowerMin, lowerMax);
        LowerJoint.localRotation = Quaternion.Slerp(
            LowerJoint.localRotation,
            lowerRest * Quaternion.Euler(lowerDelta, 0f, 0f),
            t);

        // Δαγκάνα: το φυσικό εύρος 0..107 απεικονίζεται γραμμικά στο
        // οπτικό 0..50, ώστε το πλήρες άνοιγμα του servo να αντιστοιχεί
        // στο πλήρες οπτικό άνοιγμα (πριν, κάθε τι πάνω από 50 χανόταν).
        float clawDelta = Mathf.Clamp(S4 * (clawVisualMax / clawFirmwareMax), 0f, clawVisualMax);
        ClawPincherLeft.localRotation = Quaternion.Slerp(
            ClawPincherLeft.localRotation,
            leftClawRest * Quaternion.Euler(0f, 0f, -clawDelta),
            t);
        ClawPincherRight.localRotation = Quaternion.Slerp(
            ClawPincherRight.localRotation,
            rightClawRest * Quaternion.Euler(0f, 0f, clawDelta),
            t);
    }
}