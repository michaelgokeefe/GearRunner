using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankController : MonoBehaviour {

    public GameObject target;
    public GameObject projectile;
    public float projectileSpeed;
    public int hitPoints;
    public float distToTarget;
    public float timePerShot;

    private float timeCount;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (hitPoints <= 0)
        {
            Destroy(this.gameObject);
        }

        if (timeCount >= timePerShot)
        {
            Vector3 distance = target.transform.position - transform.position;
            if (distance.magnitude <= distToTarget)
            {
                Shoot();
            }
                
            timeCount = 0;
        }
        else
        {
            timeCount += Time.deltaTime;
        }

		if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }

        TargetAt(target);        
	}

    void Shoot()
    {
        Transform barrellEnd = transform.GetChild(0).GetChild(0).GetChild(0).GetChild(1);  //spawn at barrell end
        Transform cannon = transform.GetChild(0).GetChild(0).GetChild(0);
        Quaternion rot = cannon.rotation;
        GameObject bullet = GameObject.Instantiate(projectile, barrellEnd.position, rot);

        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * projectileSpeed;
    }

    void TargetAt(GameObject target)
    {
        Transform barrell = transform.GetChild(0).GetChild(0).GetChild(0);
        barrell.LookAt(target.transform);        
    }

    private void OnDestroy()
    {
        Debug.Log("Tank is destroyed");
    }
}
