using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MechMovementTypes { Stopped, Walking, Jogging, Running };
public class MechMotor : MonoBehaviour {
    public float BikeVelocity;
    public float MechVelocity;
    public MechDisplays MechDisplays;
    public float MaxForwardVelocity;
    public float MaxStrafeVelocity;
    public float BikeMaxLean;
    public MechMovementTypes MechMovementType;
    //public Transform BikeTransform;

    public float RunningThreshold;
    public float JoggingThreshold;
    public float WalkingThreshold;

    public float RunningVelocity;
    public float JoggingVelocity;
    public float WalkingVelocity;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {



    }


    public void HandleForwardMotion(float forwardVelocity) {
        //Debug.Log(forwardVelocity);
        MechMovementTypes cachedMovementType = MechMovementType;
        if (forwardVelocity < WalkingThreshold) {
            MechMovementType = MechMovementTypes.Stopped;
        }
        else if (forwardVelocity < JoggingThreshold) {
            MechMovementType = MechMovementTypes.Walking;
        }
        else if (forwardVelocity < RunningThreshold) {
            MechMovementType = MechMovementTypes.Jogging;
        }
        else {
            MechMovementType = MechMovementTypes.Running;
        }
        if (cachedMovementType != MechMovementType) {
            ChangeVelocity();
        }


        transform.Translate(Vector3.forward * Time.deltaTime * MechVelocity);

    }


    public void ChangeVelocity() {
        switch (MechMovementType) {
            case MechMovementTypes.Stopped:
                MechVelocity = 0;
                break;
            case MechMovementTypes.Walking:
                MechVelocity = WalkingVelocity;
                break;
            case MechMovementTypes.Jogging:
                MechVelocity = JoggingVelocity;
                break;
            case MechMovementTypes.Running:
                MechVelocity = RunningVelocity;
                break;
        }
    }
    public void HandleStrafeMotion(float leanPercent) {
        //float scaleFactor = (BikeVelocity / MaxForwardVelocity);
        ////BikeTransform.localEulerAngles = new Vector3(BikeTransform.localEulerAngles.x, BikeTransform.localEulerAngles.y, leanPercent * BikeMaxLean *scaleFactor);


        //float strafeTranslateAmount = Time.deltaTime * leanPercent * MaxStrafeVelocity * scaleFactor;

        //if (Mathf.Abs(transform.position.x) <= 6f) {
        //    transform.Translate(Vector3.right * strafeTranslateAmount);
        //    transform.position = new Vector3(Mathf.Clamp(transform.position.x, -6, 6), transform.position.y, transform.position.z);
        //}
    }

}
