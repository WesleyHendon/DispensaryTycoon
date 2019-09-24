using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleManager : MonoBehaviour
{
    public GameObject tempObject;

    public List<VehicleSpawn> vehicleSpawns = new List<VehicleSpawn>();

    public float spawnTimer = .25f;
    public float timeSpawned = 0.0f;
    void Update()
    {
        if (Time.time - timeSpawned >= spawnTimer)
        {
            SpawnVehicle();
        }
    }

    public void SpawnVehicle()
    {
        timeSpawned = Time.time;
        VehicleSpawn randomVehicleSpawn = vehicleSpawns[Random.Range(0, vehicleSpawns.Count)];
        GameObject newVehicle = Instantiate(tempObject);
        newVehicle.gameObject.SetActive(true);
        newVehicle.transform.position = randomVehicleSpawn.transform.position;
        newVehicle.transform.eulerAngles = randomVehicleSpawn.transform.eulerAngles;
        Vehicle newVehicleComponent = newVehicle.GetComponent<Vehicle>();
        newVehicleComponent.currentDirection = randomVehicleSpawn.startDirection;
        newVehicleComponent.OnSpawn(randomVehicleSpawn.startDirection, randomVehicleSpawn.startRoad);
        float randomSpeed = Random.Range(5f, 7.85f);
        newVehicleComponent.maxSpeed = randomSpeed;
        newVehicleComponent.speed = 0;

    }
}
