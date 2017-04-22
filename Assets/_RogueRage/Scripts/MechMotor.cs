using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechMotor : MonoBehaviour {
    public float BikeVelocity;
    public MechDisplays MechDisplays;
    public float MaxForwardVelocity;
    public float MaxStrafeVelocity;
    public float BikeMaxLean;
    public Transform BikeTransform;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {



    }


    public void HandleForwardMotion(float forwardVelocity) {
        transform.Translate(Vector3.forward * Time.deltaTime * forwardVelocity);
        BikeVelocity = forwardVelocity;


    }
    public void HandleStrafeMotion(float leanPercent) {
        float scaleFactor = (BikeVelocity / MaxForwardVelocity);
        BikeTransform.localEulerAngles = new Vector3(BikeTransform.localEulerAngles.x, BikeTransform.localEulerAngles.y, leanPercent * BikeMaxLean *scaleFactor);


        float strafeTranslateAmount = Time.deltaTime * leanPercent * MaxStrafeVelocity * scaleFactor;

        if (Mathf.Abs(transform.position.x - strafeTranslateAmount) < 6f) {
            transform.Translate(Vector3.left * strafeTranslateAmount);
        }
    }

}
