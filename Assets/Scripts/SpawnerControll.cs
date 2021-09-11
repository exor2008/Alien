using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerControll : MonoBehaviour
{
    public GameObject[] spawners;
    public GameObject[] operatives;
    public GameObject alienObj;
    public float spawnDelay;
    
    public float timeSinceSpawn;
    Alien alien;

    void Start()
    {
        alien = alienObj.GetComponent<Alien>();
        alien.SetSpawnController(this);
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
        int spawnerIdx = Random.Range(0, spawners.Length - 1);
        return spawners[spawnerIdx];
    }
}
