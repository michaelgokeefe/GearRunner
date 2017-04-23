using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechLegMotor : MonoBehaviour {
    public Animator Anim;
    public float RunSpeedScale;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Anim.speed = RunSpeedScale;
	}
}
