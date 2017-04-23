using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DroneChild {
    public Transform DronePath;
    //public bool DronePathOccupied;
    public Drone Drone;

}


public class DroneParent : MonoBehaviour {

    public Transform PlayerRoot;
    public List<DroneChild> DroneChildren;
    public float SpawnTimer;
    public float MinTimeToSpawn;
    public float MaxTimeToSpawn;
    public int CurrentSpawnedDrones;
    public int MaxSpawnedDrones;
    public GameObject DronePrefab;
    public List<int> AvailableDronePaths = new List<int>();
	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        transform.position = new Vector3(0, 2, PlayerRoot.position.z);




        SpawnTimer -= Time.deltaTime;

        if (SpawnTimer < 0) {
            SpawnTimer = Random.Range(MinTimeToSpawn, MaxTimeToSpawn);
            AttemptToSpawnDrone();
        }

    }

    public void AttemptToSpawnDrone() {
        if (CurrentSpawnedDrones < MaxSpawnedDrones) {
            //int pathIndex;
            AvailableDronePaths.Clear();
            for (int i = 0; i < DroneChildren.Count; i++) {
                if (DroneChildren[i].Drone == null) {
                    AvailableDronePaths.Add(i);
                }
            }
            int rand = Random.Range(0, AvailableDronePaths.Count);
            int pathIndex = AvailableDronePaths[rand];


            GameObject drone = GameObject.Instantiate(DronePrefab, DroneChildren[pathIndex].DronePath) as GameObject;
            drone.transform.localPosition = new Vector3(0, Random.Range(0, 1f), -50f);
            CurrentSpawnedDrones++;
            Drone droneLogic = drone.GetComponent<Drone>();
            DroneChildren[pathIndex].Drone = droneLogic;
            droneLogic.Index = pathIndex;
            droneLogic.ParentLogic = this;
            droneLogic.PlayerRoot = PlayerRoot;
            //SpawnedVehicles.Add(vehicle.GetComponent<VehicleMovement>());
            //WorldRecycler.DynamicEntities.Add(vehicle.gameObject);
            //_CurrentSpawnedVehicles++;
        }
    }
}
