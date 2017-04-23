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
        Transform barrell = transform.GetChild(0).GetChild(0).GetChild(1).transform;  //spawn at barrell end
        Quaternion rot = transform.GetChild(0).GetChild(0).transform.rotation;  //rotation barrell
        rot = new Quaternion(rot.x, rot.y, rot.z + 90, rot.w);
        GameObject bullet = GameObject.Instantiate(projectile, barrell.position, rot);
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * projectileSpeed;
    }

    void TargetAt(GameObject target) { }
}
