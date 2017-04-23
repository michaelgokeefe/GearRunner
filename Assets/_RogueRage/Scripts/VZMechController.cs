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

    public float NeckHeight;
    public float NeckForwardOffset;

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
    void Awake() {




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
        //VZController.AttachPlayer(MechTorso, HeadCamera);


    }
    void Start() {
        //_MotionInput.OnMenu = false;

        // Transition canvas
        //VZController.TransitionCanvas().GetComponent<Canvas>().enabled = true;

        //// Offset raycast for terrain
        //Transform raycastOrigin = transform.Find("RaycastOrigin");
        //if (raycastOrigin != null) {
        //    mRaycastOffset = raycastOrigin.localPosition;
        //}





        // Lookup release screen
        //Transform release = VZController.TransitionCanvas().transform.Find("Release");

        //if (release != null) {
        //    _TransitionRelease = release.GetComponent<Text>();
        //}


        //IsCalibrated = false;
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
    void Update() {
        HandleLeaning();
        HandleThrottle();
        HandleInput();
    }

    void FixedUpdate() {

        if (Motor.MechDisplays) {
            SpeedometerTicks++;
            if (SpeedometerTicks > TicksUntilSpeedometerRefresh) {
                SpeedometerTicks = 0;
                Motor.MechDisplays.UpdateVelocityDisplay(Motor.BikeVelocity);
            }
        }

        InputSpeeds.Add(VZController.InputSpeed);
        if (InputSpeeds.Count >= InputSpeedsToAverage) {
            float sumSpeed = 0;
            for (int i = 0; i < InputSpeeds.Count; i++) {
                sumSpeed += InputSpeeds[i];
            }
            AverageInputSpeed = (sumSpeed / InputSpeeds.Count);
            Debug.LogWarning("Avg Speed: " + AverageInputSpeed);
            InputSpeeds.Clear();
            //Motor.HandleForwardMotion(AverageInputSpeed);

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
        if (Mathf.Abs(HeadCamera.transform.localPosition.x) > MinDistanceToLean) {
            leanPercent = Mathf.Min((Mathf.Abs(HeadCamera.transform.localPosition.x) - MinDistanceToLean), MaxLeanDelta) / MaxLeanDelta;
            if (HeadCamera.transform.localPosition.x < 0) {
                leanPercent *= -1;
            }
        }
        else {
            leanPercent = 0;
        }

        Motor.HandleStrafeMotion(leanPercent);
    }

    void HandleThrottle() {

        Motor.HandleForwardMotion(AverageInputSpeed);
    }
}