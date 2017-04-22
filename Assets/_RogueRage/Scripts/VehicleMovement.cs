using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleMovement : MonoBehaviour {
    public float MaxVelocity;
    public float VehicleVelocity;
    public float StrafingVelocity;
    public bool ExemptFromTriggerDespawn;
    public bool AdvancedMovement;
    public float VehicleLength;
    public float VehicleWidth;

    public bool CanChangeLaneLeft;
    public bool CanChangeLaneRight;
    public LayerMask VehicleLayerMask;

    public Collider AheadVehicleCollider;
    public VehicleMovement AheadVehicleLogic;
    float _LaneChangeTime = 1f;

    public Vector3 origin;
    //public Vector3 hitPoint;
    public float distanceToVehicle;
    public float slowdownDistance;

    public float targetSpeedDifference;
    float _CachedAheadVehicleVelocity;
    //public float IdealDecel;
    public float IdealBreakingDecel;
    public float Accel;
    //public float decel;

    public float DistanceToKeepBetweenVehicles;
    public bool IsChangingLanes;
    public bool PassingLeft;
    public bool PassingRight;

    public int LaneIndex;
    public int LastValidLaneIndex;
    public int TargetLaneIndex;
    public bool GoingForward;


	// Use this for initialization
	void Start () {
        VehicleVelocity = MaxVelocity;
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(new Vector3(StrafingVelocity, 0, VehicleVelocity) * Time.deltaTime);// Vector3.forward * Time.deltaTime * VehicleVelocity);
        //AvoidForwardCollisions();

        if (IsChangingLanes) {
            float xDestination = 3.75f - (TargetLaneIndex * 2.5f);

            if (PassingLeft) {
                if (transform.position.x < xDestination) {
                    transform.position = new Vector3(xDestination, 0, transform.position.z);
                    PassingLeft = false;
                    IsChangingLanes = false;
                    LaneIndex = TargetLaneIndex;
                    StrafingVelocity = 0;
                }

            }
            else if (PassingRight) {
                if (transform.position.x > xDestination) {
                    transform.position = new Vector3(xDestination, 0, transform.position.z);
                    PassingRight = false;
                    IsChangingLanes = false;
                    LaneIndex = TargetLaneIndex;
                    StrafingVelocity = 0;
                }
            }
            else {
                Debug.LogWarning("We should either be passing left or right");
            }




        }
    }

    void EvaluateSlowdownDifference(float distanceToVehicle) {


        ////We're too close
        //if (distanceToVehicle < idealSlowdownDistance) {
        //    slowdownDistance = distanceToVehicle;

        //    decel = Mathf.Abs(Mathf.Pow(targetSpeedDifference, 2f) / (2 * (distanceToVehicle - 5)));
        //    Debug.LogWarning("we're too close! " + distanceToVehicle + " is smaller than " + idealSlowdownDistance );
        //}

        //else {
        //    slowdownDistance = idealSlowdownDistance;
        //    decel = MinBreakingDecel;
        //}





    }

    void AvoidForwardCollisions() {

        RaycastHit hit;


        Vector3 origin = new Vector3(transform.position.x, 1, transform.position.z + VehicleLength / 2);
        if (Physics.Raycast(origin, Vector3.forward, out hit, VehicleVelocity, VehicleLayerMask)) {
            if (AheadVehicleCollider != hit.collider) {
                AheadVehicleCollider = hit.collider;
                AheadVehicleLogic = hit.collider.gameObject.GetComponent<VehicleMovement>();

            }

            distanceToVehicle = hit.point.z - origin.z;

            //This Vehicle is slower
            if (AheadVehicleLogic.VehicleVelocity <= VehicleVelocity) {
                //everytime the ahead vehicles speed changes we need to re-evaluate the slowdown distance
                if (_CachedAheadVehicleVelocity != AheadVehicleLogic.VehicleVelocity) {
                    _CachedAheadVehicleVelocity = AheadVehicleLogic.VehicleVelocity;
                    slowdownDistance = Mathf.Pow(VehicleVelocity - AheadVehicleLogic.VehicleVelocity, 2f) / (2 * IdealBreakingDecel) + DistanceToKeepBetweenVehicles;
                }
                if (distanceToVehicle <= DistanceToKeepBetweenVehicles) {
                    VehicleVelocity = AheadVehicleLogic.VehicleVelocity;
                }
                //Either slowdown or pass once we are within range
                else if (distanceToVehicle <= slowdownDistance) {
                    if (!IsChangingLanes) {
                        bool canPass = true;
                        //Slow Down
                        if (canPass) {
                            IsChangingLanes = true;

                            //Right Lane Pass
                            if (LaneIndex == LastValidLaneIndex) {
                                Debug.LogWarning("Right Lane Pass");
                                TargetLaneIndex = LaneIndex - 1;
                                PassingRight = true;
                                StrafingVelocity = 3f;
                            }
                            //Left Lane pass
                            else {
                                Debug.LogWarning("Left Lane Pass");
                                TargetLaneIndex = LaneIndex + 1;
                                PassingLeft = true;
                                StrafingVelocity = -3f;
                            }
                        }
                        else {
                            VehicleVelocity -= Time.deltaTime * IdealBreakingDecel;
                            if (VehicleVelocity < AheadVehicleLogic.VehicleVelocity) {
                                VehicleVelocity = AheadVehicleLogic.VehicleVelocity;
                            }
                        }
                    }
                }
            }

            else {
                if (VehicleVelocity < MaxVelocity) {
                    VehicleVelocity = Mathf.Min(VehicleVelocity + (Time.deltaTime * Accel), MaxVelocity);
                }
            }

        }
    }
}
