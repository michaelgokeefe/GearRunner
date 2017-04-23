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

            Transform head = transform.GetChild(0);
            head.LookAt(target.transform);
            Quaternion headRotation = Quaternion.Euler(-90, 0, head.eulerAngles.z);
            head.rotation = headRotation;

            //Transform barrel = head.GetChild(0);
            //barrel.LookAt(target.transform);

        }
	}

    void Shoot()
    {
        Transform barrell = transform.GetChild(0).GetChild(0).GetChild(1).transform;  //spawn at barrell end
        Quaternion rot = transform.GetChild(0).GetChild(0).transform.rotation;  //rotation barrell
        rot = Quaternion.Euler(rot.eulerAngles.x + 90, rot.eulerAngles.y, rot.eulerAngles.z + 90);
        GameObject bullet = GameObject.Instantiate(projectile, barrell.position, rot);

        bullet.transform.forward = transform.GetChild(0).GetChild(0).forward;
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * projectileSpeed;
    }

    void TargetAt(GameObject target)
    {

    }
}
