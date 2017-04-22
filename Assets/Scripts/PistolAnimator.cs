using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PistolAnimator : MonoBehaviour {
    public Animator AnimPistol;
    public Animator AnimSlide;
    public Animator AnimTrigger;

    public void PullTrigger() {
        AnimTrigger.Play("TriggerPull");
    }
    public void FirePistol() {
        AnimPistol.Play("FirePistol");
    }
    public void SlideShot() {
        AnimSlide.Play("SlideShot");
    }
    public void SlideStick() {
        AnimSlide.Play("SlideStick");
    }
    public void SlideRelease() {
        AnimSlide.Play("SlideReset");
    }
    public void ReleaseTrigger() {
        AnimTrigger.Play("TriggerRelease");
    }
}
