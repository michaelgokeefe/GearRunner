using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankController : MonoBehaviour {

    public GameObject target;
    public GameObject projectile;
    public float projectileSpeed;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
	}

    void Shoot()
    {
        Transform barrell = transform.GetChild(1).GetChild(0).transform;
        Quaternion rot = transform.GetChild(1).transform.rotation;
        GameObject bullet = GameObject.Instantiate(projectile, barrell.position, rot);
        bullet.GetComponent<Rigidbody>().AddForce(barrell.forward * projectileSpeed);
    }

    void TargetAt(GameObject target) { }
}
