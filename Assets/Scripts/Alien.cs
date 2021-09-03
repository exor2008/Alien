using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Alien : MonoBehaviour
{
    public GameObject[] operatives;
    public GameObject[] spawners;
    public NavMeshAgent navMeshAgent;
    public float runSpeed;
    public float jumpSpeed;
    public float escapeSpeed;
    public float jumpDistance;
    public float killDistance;
    public bool isSpawned;

    SpawnerControll spawnerController;
    StateManager stateManager;
    GameObject target;

    void Start()
    {
        stateManager = new StateManager(new IdleState(this));
    }

    void Update()
    {
        stateManager.Updtae();
    }

    public void MoveToClosestObject(GameObject[] objects)
    {
        if (FindClosestObject(out target, objects))
        {
            navMeshAgent.SetDestination(target.transform.position);
        }
    }

    public void MoveToClosestTarget()
    {
        MoveToClosestObject(operatives);
    }

    public void MoveToClosestSpawner()
    {
        MoveToClosestObject(spawners);
    }

    public bool FindClosestObject(out GameObject closest, GameObject[] objects)
    {
        float minDist = float.MaxValue;
        closest = null;

        foreach (GameObject obj in objects)
        {
            float dist = Vector3.Distance(transform.position, obj.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = obj;
            }
        }
        if (closest == null)
            return false;
        else
        {
            return true;
        }
    }

    public bool FindClosestTarget(out GameObject closest)
    {
        return FindClosestObject(out closest, operatives);
    }

    public bool FindClosestSpawner(out GameObject closest)
    {
        return FindClosestObject(out closest, spawners);
    }

    public void Spawn(Vector3 position)
    {
        navMeshAgent.Warp(position);
        isSpawned = true;
        SwitchState(new HuntState(this));
    }

    public void Hide()
    {
        navMeshAgent.Warp(new Vector3(130, 0, -20));
        isSpawned = false;
        spawnerController?.UpdateTimeSpawn();
    }

    public void SwitchState(State state)
    {
        stateManager.SwitchState(state);
    }

    public bool isTargetVisible(out RaycastHit hit)
    {
        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            return hit.collider == target.GetComponent<Collider>();
        }
        return false;
    }

    public void SetJumpSpeed()
    {
        navMeshAgent.speed = jumpSpeed;
        navMeshAgent.acceleration = 8;
    }

    public void SetRunSpeed()
    {
        navMeshAgent.speed = runSpeed;
        navMeshAgent.acceleration = 32;
    }

    public void SetEscapeSpeed()
    {
        navMeshAgent.speed = escapeSpeed;
        navMeshAgent.acceleration = 16;
    }

    public void SetPawnController(SpawnerControll _spawnerController)
    {
        spawnerController = _spawnerController;
    }
}
