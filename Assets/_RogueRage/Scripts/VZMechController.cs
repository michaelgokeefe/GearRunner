using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VZMechController : MonoBehaviour {
    public MechMotor Motor;
    public MechPlayerInput PlayerInput;
    public Transform PlayerRoot;
    public Transform MechTorso;
    public Camera HeadCamera;
    public Transform CameraTransform;
    public Transform CameraRigPC;

    public AnimationCurve StrafeThresholds;
    public float TurningAngleThreshold;
    public float HeadBikeFactor;
    public float TargetBikeZ;
    public float ActualBikeZ;
    public float BikeZLerpSpeed;
    public float BikeLeanSpeed;
    public float LookForwardAngleThreshold;

    public float headZAngle;
    public float BikeVelocity;
    //public float MaxStrafeVelocity;
    //public float MaxVelocity;
    public float Acceleration;



    public float StrafeInput;
    public bool UsingHeadset;
    public float LeanSpeed; //Degrees per second
    public float LeanAngle;
    public GameObject VZControllerPrefab;
    //public float bikeZAngle;
    // Use this for initialization
    public static VZController VZController { get; private set; }
    public bool IsCalibrated;
    public float NeckHeight;

    List<float> InputSpeeds = new List<float>();
    public int InputSpeedsToAverage;
    public float AverageInputSpeed;
    public int TicksUntilSpeedometerRefresh;
    public int SpeedometerTicks;


    protected const string STATE_CALIBRATING = "Calibrating";
    protected const string STATE_NORMAL = "Normal";
    const string RELEASE_TEXT = "By continuing use of the VirZOOM bike controller you agree for yourself and (if applicable) members of your family to all terms and conditions of the License Agreement at virzoom.com/eula.htm";
    protected VZMotionInput _MotionInput = new VZMotionInput();
    VZMotionOutput _MotionOutput = new VZMotionOutput();
    string _State = "";
    protected string _PrevState = "";


    Text _TransitionRelease = null;

    public float MinDistanceToLean;
    public float MaxLeanDelta;
    void Awake () {




        // Init camera
        if (HeadCamera == null) {
            HeadCamera = UnityEngine.Camera.main;
        }

        // Instantiate controller prefab
        if (VZController == null) {
            GameObject go = Instantiate(VZControllerPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            VZController = go.GetComponent<VZController>();
            VZController.transform.localScale = Vector3.one;
            VZController.name = "VZController";
            DontDestroyOnLoad(VZController);
        }

        // Attach controller to us
        VZController.AttachPlayer(MechTorso, HeadCamera);


    }
    void Start() {
        _MotionInput.OnMenu = false;

        // Transition canvas
        VZController.TransitionCanvas().GetComponent<Canvas>().enabled = true;

        //// Offset raycast for terrain
        //Transform raycastOrigin = transform.Find("RaycastOrigin");
        //if (raycastOrigin != null) {
        //    mRaycastOffset = raycastOrigin.localPosition;
        //}

        // Init neck height
        VZController.Neck().localPosition = new Vector3(0, NeckHeight, 0);

        // Lookup release screen
        Transform release = VZController.TransitionCanvas().transform.Find("Release");

        if (release != null) {
            _TransitionRelease = release.GetComponent<Text>();
        }


        IsCalibrated = false;
    }

    void UpdateCalibrating() {
        // Try connecting to bike
        if (VZController.BikeState().Type < 0)
            VZController.TryConnectBike();

        // Update calibration msg
        if (_TransitionRelease != null) {
            _TransitionRelease.text = RELEASE_TEXT + "\n\n\n";

#if !UNITY_EDITOR
         // Require bike outside of editor
         if (VZController.BikeState().Type < 0)
            _TransitionRelease.text += "PLUG IN DONGLE";
         else if (!VZController.BikeState().Connected)
            _TransitionRelease.text += "POWER ON BIKE";
         else
#endif
            _TransitionRelease.text += "HOLD L+R TO BEGIN";
        }

        // Hold both buttons to calibrate
        if (VZController.LeftButton.Held(0.5f) && VZController.RightButton.Held(0.5f) && VZController.IsHeadTracked()) {
            VZController.LeftButton.Clear();
            VZController.RightButton.Clear();

            VZController.Recenter();

            Restart(true);
            Debug.Log(PlayerInput.transform.parent.name + " : " + PlayerInput.transform.parent.localEulerAngles.y);
            PlayerInput.YawOffset = PlayerInput.transform.parent.localEulerAngles.y - 360;
            PlayerInput.YawTransform = PlayerInput.transform;
        }
    }
    protected virtual void Restart(bool initLevel) {

        // Reset transition canvas alpha
        VZController.TransitionCanvas().GetComponent<CanvasGroup>().alpha = 1;

        // Reset controller
        VZController.Restart();

        //// Reset state
        //mSpeedMultiplier = 1.0f;

        // Reset initial position and rotation
        //transform.position = mInitialPos;
        //GetComponent<Rigidbody>().velocity = Vector3.zero;
        //mBodyRot = mInitialRot;

        VZPlugin.ResetMotion();
        IsCalibrated = true;
        SetState(STATE_NORMAL);

#if !VZ_GAME
        StartCoroutine(FadeDown(2));
#endif
    }

    protected string State() {
        return _State;
    }

    protected void SetState(string state) {
#if VZ_GAME
      VZUtl.Log("SetState: from " + mState + " to " + state);
#endif
        _PrevState = _State;
        _State = state;
    }

    protected virtual IEnumerator FadeDown(float fadeTime) {
        // Fade alpha down to zero
        CanvasGroup group = VZController.TransitionCanvas().GetComponent<CanvasGroup>();
        float time = 0;
        float alpha = group.alpha;

        while (time < fadeTime) {
            time += Time.deltaTime;
            group.alpha = Mathf.SmoothStep(alpha, 0.0f, time / fadeTime);
            yield return null;
        }

        // Deactivate and reset alpha
        VZController.TransitionCanvas().SetActive(false);
        group.alpha = 1.0f;
    }

    // Update is called once per frame
    void Update () {
        if (!IsCalibrated) {
            UpdateCalibrating();
            return;
        }
        HandleLeaning();
        HandleThrottle();
        HandleInput();
    }

    void FixedUpdate() {
        if (!IsCalibrated) {
            return;
        }
        SpeedometerTicks++;
        if (SpeedometerTicks > TicksUntilSpeedometerRefresh) {
            SpeedometerTicks = 0;
           Motor.MechDisplays.UpdateVelocityDisplay(Motor.BikeVelocity);
        }
        InputSpeeds.Add(VZController.InputSpeed);
        if (InputSpeeds.Count >= InputSpeedsToAverage) {
            float sumSpeed = 0;
            for (int i = 0; i< InputSpeeds.Count; i++) {
                sumSpeed += InputSpeeds[i];
            }
            AverageInputSpeed = (sumSpeed / InputSpeeds.Count) * 4;
            //Debug.LogWarning("Avg Speed: " + AverageInputSpeed);
            InputSpeeds.Clear();

        }
    }
    void HandleInput() {
        if (VZController.RightButton.Pressed()) {
            PlayerInput.MainInputDown();
        }
        if (VZController.RightButton.Released()) {
            PlayerInput.MainInputUp();
        }
    }
    void HandleLeaning() {
        float leanPercent;
        if (Mathf.Abs(VZController.HeadLean) > MinDistanceToLean) {
            leanPercent = Mathf.Min((Mathf.Abs(VZController.HeadLean) - MinDistanceToLean), MaxLeanDelta) / MaxLeanDelta;
            if (VZController.HeadLean <0) {
                leanPercent *= -1;
            }
        }
        else {
            leanPercent = 0;
        }
        Motor.HandleStrafeMotion(leanPercent);
    }

    void HandleThrottle() {
        if (BikeVelocity < AverageInputSpeed) {
            BikeVelocity += Time.deltaTime * Acceleration;
        }
        else {
            BikeVelocity -= Time.deltaTime * Acceleration;
        }
        BikeVelocity = Mathf.Clamp(BikeVelocity, 0, Motor.MaxForwardVelocity);
        Motor.HandleForwardMotion(BikeVelocity);
    }
}
//if (Mathf.Abs(ActualBikeZ) > 5) {
//    float translateAmount = Time.deltaTime * (ActualBikeZ / LeanAngle) * BikeMotor.MaxStrafeVelocity * (BikeVelocity / BikeMotor.MaxForwardVelocity);
//    BikeMotor.HandleStrafeMotion(translateAmount);
//}
//if (UsingHeadset) {
//    headZAngle = CameraTransform.localRotation.eulerAngles.z;
//    if (headZAngle > 180) {
//        headZAngle -= 360;


//        headZAngle = Mathf.Max(-40, headZAngle);
//    }

//    else {
//        headZAngle = Mathf.Min(40, headZAngle);
//    }
//    ActualBikeZ = headZAngle;
//}
//else {
//    if (StrafeInput != 0) {
//        //Debug.Log(CameraRigPC.localEulerAngles.y);
//        ActualBikeZ = Mathf.Lerp(ActualBikeZ, -StrafeInput * LeanAngle * StrafeThresholds.Evaluate(CameraRigPC.localEulerAngles.y), Time.deltaTime * 10);


//        //ActualBikeZ += StrafeInput * Time.deltaTime * -LeanSpeed;
//    }
//    else {
//        if (ActualBikeZ < 0) {
//            ActualBikeZ += Time.deltaTime * LeanSpeed;
//            if (ActualBikeZ > 0) {
//                ActualBikeZ = 0;
//            }
//        }
//        else if (ActualBikeZ > 0) {
//            ActualBikeZ -= Time.deltaTime * LeanSpeed;
//            if (ActualBikeZ < 0) {
//                ActualBikeZ = 0;
//            }
//        }
//    }

//    //ActualBikeZ = Mathf.Lerp(ActualBikeZ, -StrafeInput * LeanAngle, Time.deltaTime);
//    ActualBikeZ = Mathf.Clamp(ActualBikeZ, -LeanAngle, LeanAngle);

//}