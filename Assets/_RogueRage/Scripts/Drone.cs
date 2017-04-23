using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Drone : MonoBehaviour, IDamageable {
    public Vector3 LocalDestination;
    public Transform DroneMeshTransform;
    public float Speed;
    public float Health;
    public GameObject ExplosionPrefab;
    public GameObject HitEffectPrefab;
    public DroneParent ParentLogic;
    public int Index;
    public bool ImDead;
    public Transform PlayerRoot;
    public SphereCollider HitSphere;
	// Use this for initialization


	void Start () {
        PickNewDestination();
	}
    public void Damage(float damage) {
        Health -= damage;



        if (Health <= 0) {
            GameObject explosion = GameObject.Instantiate(ExplosionPrefab, transform.position, transform.rotation, transform) as GameObject;

            DroneMeshTransform.gameObject.SetActive(false);
            Invoke("HandleDeath", 2f);
            Destroy(HitSphere);
            ImDead = true;
        }
        else {
            GameObject hitEffect = GameObject.Instantiate(HitEffectPrefab, transform.position, transform.rotation, transform) as GameObject;

        }
    }
    public void HandleDeath() {
        ParentLogic.DroneChildren[Index].Drone = null;
        ParentLogic.CurrentSpawnedDrones--;
        Destroy(gameObject);
    }

    public void PickNewDestination() {
        if (ImDead) {
            return;
        }
        LocalDestination = new Vector3(0, Random.Range(0, 1f), Random.Range(-2f, 30f));

        float dist = Vector3.Distance(transform.localPosition, LocalDestination);

        transform.DOLocalMove(LocalDestination, dist / Speed).OnComplete(WaitRandomTime) ;
    }
    public void WaitRandomTime() {
        Invoke("PickNewDestination", Random.Range(1f, 6f));
    }

    void Update() {
        DroneMeshTransform.LookAt(PlayerRoot);
    }
	// Update is called once per frame
	//void Update () {
 //       if (LocalDestination.z > transform.localPosition.z) {
 //           transform.Translate(Vector3.forward * Time.deltaTime * Velocity);
 //           if (LocalDestination.z <= transform.localPosition.z) {
 //               PickNewDestination();
 //           }
 //       }
 //       else {
 //           transform.Translate(-Vector3.forward * Time.deltaTime * Velocity);
 //           if (LocalDestination.z >= transform.localPosition.z) {
 //               PickNewDestination();
 //           }
 //       }
	//}
}
