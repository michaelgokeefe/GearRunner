using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GunLogic : MonoBehaviour {
    public MechPlayerInput PlayerInput;
    public GameObject LaserRod;
    public GameObject GazeTarget;
    public bool Equipped;
    public bool IsAiming;
    public float LaserOffset;
    public PistolAnimator AnimationLogic;
    public Transform MagSlot;
    public GameObject MagPrefab;

    AudioSource _AudioSource;
    public AudioClip ReloadSound;
    public AudioClip ShotSound;
    public Transform BarrelPoint;
    public float Damage;
    public Text AmmoCountText;
    public Image ReloadIndicator;
    public int AmmoCount;
    public int MagSize;
    public bool ReloadTimer;
    public bool CanReload;
    bool TriggerPulled;
    bool MagSpinning;
    GameObject ReloadMag;
    float _ReloadMagAngle;
    public LayerMask LaserCollideLayers;

    // Use this for initialization
    void Start () {
        AmmoCount = MagSize;
        AmmoCountText.text = AmmoCount.ToString();
        //CanShoot = true;
        _AudioSource = GetComponent<AudioSource>();
        StopAiming();
        CanReload = true;
	}


    public void StartAiming() {
        IsAiming = true;
        LaserRod.SetActive(true);
        Equipped = true;
        GazeTarget.SetActive(true);
    }
    public void StopAiming() {
        IsAiming = false;
        LaserRod.SetActive(false);
        Equipped = false;
        GazeTarget.SetActive(false);
    }

    public void PullTriggerLogic() {
        TriggerPulled = true;
        AnimationLogic.PullTrigger();
        AmmoCount--;

        RaycastHit hit;
        if (Physics.Raycast(BarrelPoint.position, BarrelPoint.forward, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Drone"))) {
            hit.collider.GetComponent<IDamageable>().Damage(Damage);

        }
        _AudioSource.clip = ShotSound;
        _AudioSource.Play();

        if (AmmoCount == 0) {
            AmmoCountText.text = "--";
        }
        else {
            AmmoCountText.text = AmmoCount.ToString();
        }


        AnimationLogic.FirePistol();

        if (AmmoCount > 1) {
            AnimationLogic.SlideShot();
            //AnimSlide.Play("SlideShot");

        }
        else {
            AnimationLogic.SlideStick();
            //AnimSlide.Play("SlideStick");
        }
    }

    public void PopMag() {
        ReloadMag = GameObject.Instantiate(MagPrefab,PlayerInput.BikeTransform) as GameObject;
        CanReload = false;
        Invoke("ResetReload", 1.5f);
        MagSpinning = true;
    }

    public void ResetReload() {

        CanReload = true;
        Destroy(ReloadMag);
        MagSpinning = false;
    }

    public void CompleteReload() {
        Destroy(ReloadMag);
        CanReload = true;
        AmmoCount = MagSize;
        AmmoCountText.text = AmmoCount.ToString();
        ReloadIndicator.enabled = false;
        _AudioSource.clip = ReloadSound;
        _AudioSource.Play();
    }

    public void GunInputDown() {
        if (AmmoCount > 0) {
            PullTriggerLogic();
        }
        else if (CanReload){
            ReloadTimer = true;
            ReloadIndicator.enabled = true;
            ReloadIndicator.fillAmount = 0;
        }
    }
    public void GunInputUp() {

        if (ReloadTimer) {
            ReloadTimer = false;
            ReloadIndicator.fillAmount = 0;
            ReloadIndicator.enabled = false;
        }
        TriggerPulled = false;
        AnimationLogic.ReleaseTrigger();
    }

	// Update is called once per frame
	void Update () {
        if (MagSpinning) {
            Collider[] mags = Physics.OverlapSphere(transform.position, 0.1f, 1 << LayerMask.NameToLayer("Mag"), QueryTriggerInteraction.Collide);
            if (mags.Length > 0) {
                ReloadMag.transform.SetParent(MagSlot);
                ReloadMag.transform.DOLocalMove(Vector3.zero, 0.25f);
                ReloadMag.transform.DOLocalRotate(Vector3.zero, 0.25f).OnComplete(CompleteReload);
                ReloadMag.transform.DOScale(Vector3.zero, 0.25f);
                MagSpinning = false;
                Destroy(ReloadMag.GetComponent<Animator>());
            }
            //if (Physics.Raycast(PlayerInput.transform.position, PlayerInput.transform.forward, 10f, 1<< LayerMask.NameToLayer("Mag"), QueryTriggerInteraction.Collide)) {

            //}
            //_ReloadMagAngle += Time.deltaTime * 480f;
            //ReloadMag.transform.localEulerAngles = Vector3.right * _ReloadMagAngle;
        }
 
        if (IsAiming) {
            //float zRotCache = transform.localEulerAngles.z;
            transform.LookAt(GazeTarget.transform, Vector3.up);
            //transform.localRotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, zRotCache);
            RaycastHit hit;
            if (Physics.Raycast(LaserRod.transform.position, LaserRod.transform.forward, out hit,  30f, LaserCollideLayers,  QueryTriggerInteraction.Ignore))  {

                float dist = Vector3.Distance(hit.point, LaserRod.transform.position);

                LaserRod.transform.localScale = new Vector3(1, 1, dist);
            }
            else {

                LaserRod.transform.localScale = new Vector3(1,1, 30);
            }


            if (ReloadTimer) {
                ReloadIndicator.fillAmount += Time.deltaTime * 2;
                if (ReloadIndicator.fillAmount >= 1) {
                    ReloadTimer = false;
                    PopMag();
                }
            }

        }

	}
}
