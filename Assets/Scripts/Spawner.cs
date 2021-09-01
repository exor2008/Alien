using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject alienObj;
    Alien alien;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(1, 1, 1));
    }

    public void Start()
    {
        alien = alienObj.GetComponent<Alien>();
    }

    public void SpawnAlien()
    {
        Debug.Log(string.Format("Alien spawned at {0}", Time.time));
        alien.Spawn(transform.position);
    }
}
