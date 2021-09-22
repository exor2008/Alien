using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerControll : MonoBehaviour
{
    public GameObject[] spawners;
    public OperativesControl operativesControl;
    public GameObject alienObj;
    public float spawnDelay;
    
    public float timeSinceSpawn;
    Operative[] operatives;
    Alien alien;

    void Start()
    {
        alien = alienObj.GetComponent<Alien>();
        alien.SetSpawnController(this);
        operatives = operativesControl.GetOperatives();
    }

    void Update()
    {
        if ((Time.time - timeSinceSpawn > spawnDelay) && !alien.isSpawned)
        {
            SpawnAlien();
            timeSinceSpawn = Time.time;
        }
    }

    public void UpdateTimeSpawn()
    {
        timeSinceSpawn = Time.time;
    }

    public void SpawnAlien()
    {
        GameObject spawnerObj = ChooseSpawner();
        Spawner spawner = spawnerObj.GetComponent<Spawner>();
        spawner.SpawnAlien();
    }

    GameObject ChooseSpawner()
    {
        const int N_CLOSEST_SPAWNERS = 3;
        GameObject spawner = null;
        
        // try to find closest reachable spawner
        foreach (Operative operative in operativesControl.GetAliveShuffledOperatives())
        {
            if(Find.OneOfNReachableClosest(
                N_CLOSEST_SPAWNERS,
                operative.transform.position,
                operative.navAgent,
                spawners,
                out spawner))
            {
                return spawner;
            }
        }
        // find any closest spawner
        foreach (Operative operative in operativesControl.GetAliveShuffledOperatives())
        {
            if (Find.OneOfNClosest(
                N_CLOSEST_SPAWNERS,
                operative.transform.position,
                operative.navAgent,
                spawners,
                out spawner))
            {
                return spawner;
            }
        }

        return spawners[0];
    }
}