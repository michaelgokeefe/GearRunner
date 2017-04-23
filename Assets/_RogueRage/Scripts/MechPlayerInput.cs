using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.VR;

public class MechPlayerInput : MonoBehaviour {
    public Transform BikeTransform;
    public float InteractionRange = 30f;
    public LayerMask GazeTrackableSurfaces;
    public GameObject GazeTracker;
    public GameObject FocusedObject = null;
    public GameObject HeldObject = null;
    public Transform GunAimTransform;
    public bool AreGunDoorsOpen;
    public bool IsPlayerAiming;
    public bool RightGunDoorFocus;
    public Transform YawTransform;
    public float YawOffset;
    public float Yaw;
    public GameObject BikeCollisionGameObject;
    public GunLogic GunLogic;
    public Transform[] MagBoxes;
    void Start() {
    }

    void CompensateWithGunAngle() {
        if (transform.localEulerAngles.x < 270) {
            GunAimTransform.localPosition = new Vector3(0.25f, Mathf.Min( -.15f + transform.localEulerAngles.x /60, .15f) , transform.localEulerAngles.x > 20 ? Mathf.Max(.4f + (20 - transform.localEulerAngles.x) * .01f,0) : .4f);
            GunAimTransform.localRotation = Quaternion.Euler(transform.localEulerAngles.x > 20 ? Mathf.Max( (20- transform.localEulerAngles.x) * 2 , -150) : 0, 0, 0);
        }
    }
    void HandleGunAiming() {

    }

    void HandleGazePosition() {

        if (HeldObject != null) {
            RaycastHit hit;
            bool hitBike = false;
            //Send a raycast out from the camera, in the direction the player is looking, from a distance of InteractionRange, and hit any surface colliders marked as GazeTrackable
            //We don't want to collide with triggers here, only hard colliders
            if (Physics.Raycast(transform.position, transform.forward, out hit, InteractionRange, GazeTrackableSurfaces, QueryTriggerInteraction.Ignore)) {
                //GazeTracker.transform.position = hit.point;
                if (hit.collider.gameObject == BikeCollisionGameObject) {
                    hitBike = true;
                }
                //Debug.LogWarning("Hit");
            }
            //If we didnt hit anything, keep our GazeCursor out in front of the player a distance of InteractionRange away. 
            //This ensures we won't loose track of the center of the player's vision, but the cursor won't get needlessly far away.
            //else {

            //    //Debug.Log("Miss");
            //}

            GazeTracker.transform.position = transform.position + transform.forward * 30;
            if (IsPlayerAiming) {

                if (hitBike) {

                    Debug.Log("Stop aiming");
                    IsPlayerAiming = false;
                    HeldObject.GetComponent<GunLogic>().StopAiming();
                }
            }
            else {
                if (!hitBike) {
                    Debug.Log("Start Aiming");
                    IsPlayerAiming = true;
                    HeldObject.GetComponent<GunLogic>().StartAiming();
                }
            }

        }

    }

    //void HandleInteractableFocus() {
    //    RaycastHit hit;

    //    // Use layermask to reduce the set of tested objects.
    //    if (Physics.Raycast(transform.position, transform.forward, out hit, range, layersIntersected, QueryTriggerInteraction.Collide)) {
    //        GameObject struck = hit.collider.gameObject;
    //        if (struck != FocusedObject) {
    //            // If tested layers were organised in such a way that it would be guaranteed that 
    //            // detected objects would always be capable of responding to the "OnLook" messages
    //            // then the message requirement could be made stricter.
    //            if (null != FocusedObject) {
    //                FocusedObject.SendMessage("OnLookExit", SendMessageOptions.DontRequireReceiver);
    //                //HasObjectFocused = false;
    //            }
    //            struck.SendMessage("OnLookEnter", SendMessageOptions.DontRequireReceiver);
    //            FocusedObject = struck;
    //            //HasObjectFocused = true;
    //        }
    //    }
    //    else {
    //        if (null != FocusedObject) {
    //            FocusedObject.SendMessage("OnLookExit", SendMessageOptions.DontRequireReceiver);
    //            FocusedObject = null;
    //            //HasObjectFocused = false;
    //        }
    //    }
    //}

    public void MainInputDown() {
        Debug.Log("MainInputDown");
        //if (AreGunDoorsOpen) {
        //    if (RightGunDoorFocus) {
        //        RightGunDoor.UseDoor();
        //    }
        //    else {
        //        LeftGunDoor.UseDoor();
        //    }
        //}
        //else if (GunLogic !=null && IsPlayerAiming) {
        //    GunLogic.GunInputDown();
        //}
    }
    public void MainInputUp() {
        //if (GunLogic != null && IsPlayerAiming) {
        //    GunLogic.GunInputUp();
        //}
        Debug.Log("MainInputUp");
    }
    //public bool HasObjectFocused;
    void Update() {
        //        Debug.Log(transform.localEulerAngles.x);
        Yaw = YawTransform.localEulerAngles.y + YawOffset;// > 180 ? YawTransform.localEulerAngles.y - 360f : YawTransform.localEulerAngles.y;
        if (Yaw > 180) {
            Yaw -= 360;
        }

        //Debug.Log(Yaw);
        //CheckIfCanAim();
        CompensateWithGunAngle();
        //GunDoorHingeManagement();
        //GunDoorHighlightManagement();
        //Move our Gaze Cursor
        HandleGazePosition();
        HandleGunAiming();

        //HandleInteractableFocus();
    }

    public void EquipGun(bool isRightGun) {
        HeldObject.transform.SetParent(GunAimTransform);
        HeldObject.transform.DOLocalMove(Vector3.zero, 0.5f);
        HeldObject.transform.DOLocalRotate(Vector3.zero, 0.5f);

        foreach (Transform child in HeldObject.transform.GetChild(0)) {
            if (child.GetComponent<MeshRenderer>()) {
                child.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.black);
            }
        }

        if (transform.localEulerAngles.x < 15 || transform.localEulerAngles.x >= 270) {
            IsPlayerAiming = true;
            HeldObject.GetComponent<GunLogic>().StartAiming();
        }
        GunLogic = HeldObject.GetComponent<GunLogic>();
        GunLogic.PlayerInput = this;
        //if (isRightGun) {
        //    GunLogic.MagBoxTransform = MagBoxes[1];
        //}
        //else {
        //    GunLogic.MagBoxTransform = MagBoxes[0];
        //}

    }
    public void UnequipGun() {
        GunLogic = null;
        HeldObject = null;
        IsPlayerAiming = false;
    }
    #region Editor methods

    //void OnDrawGizmos() {
    //    DrawRange();
    //}

    //void OnDrawGizmosSelected() {
    //    DrawRange();
    //}

    ///// <summary>
    ///// Shows a ray starting from this objects location out to the limit of its raycasting range.
    ///// If this script happens to be attached to the main camera then the scene view will show which
    ///// object ought to be caught by the raycast test in Update.
    ///// </summary>
    //void DrawRange() {
    //    Vector3 to = transform.position + transform.forward * range;
    //    Gizmos.DrawLine(transform.position, to);
    //}

    #endregion
}