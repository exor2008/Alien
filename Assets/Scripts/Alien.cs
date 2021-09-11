using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Alien : MonoBehaviour
{
    public GameObject[] operativesObj;
    public GameObject[] spawners;
    public NavMeshAgent navMeshAgent;
    public float runSpeed;
    public float jumpSpeed;
    public float escapeSpeed;
    public float jumpDistance;
    public float killDistance;
    public bool isSpawned;
    public float angryLevel;

    [HideInInspector]
    public ReactionResolver reactionResolver;
    Unit[] operatives = new Unit[4];
    SpawnerControll spawnerController;
    StateManager stateManager;
    GameObject target;
    


    void Start()
    {
        stateManager = new StateManager(new IdleState(this));
        reactionResolver = new ReactionResolver(this);
        for (int i = 0; i < operativesObj.Length; i++)
        {
            operatives[i] = operativesObj[i].GetComponent<Unit>();
        }
    }

    void Update()
    {
        stateManager.Updtae();
    }

    public void MoveToClosestTarget()
    {
        MoveToClosestObject(operativesObj);
    }

    public void MoveToClosestSpawner()
    {
        MoveToClosestObject(spawners);
    }
    public void MoveToClosestObject(GameObject[] objects)
    {
        BaseDistanceResolver dist = new DistanceResolver(transform.position, navMeshAgent);
        if (FindClosestObject(out target, objects, dist))
        {
            navMeshAgent.SetDestination(target.transform.position);
        }
    }

    public bool FindClosestTarget(out GameObject closest)
    {
        BaseDistanceResolver dist = new DistanceResolver(transform.position, navMeshAgent);
        return FindClosestObject(out closest, operativesObj, dist);
    }

    public bool FindClosestSpawner(out GameObject closest)
    {
        BaseDistanceResolver dist = new DistanceResolver(transform.position, navMeshAgent);
        return FindClosestObject(out closest, spawners, dist);
    }

    public void MoveToClosestReachableTarget()
    {
        MoveToClosestReachableObject(operativesObj);
    }

    public void MoveToClosestReachableSpawner()
    {
        MoveToClosestReachableObject(spawners);
    }
    public void MoveToClosestReachableObject(GameObject[] objects)
    {
        BaseDistanceResolver dist = new ReachableDistanceResolver(transform.position, navMeshAgent);
        if (FindClosestObject(out target, objects, dist))
        {
            navMeshAgent.SetDestination(target.transform.position);
        }
    }

    public bool FindClosestReachableTarget(out GameObject closest)
    {
        BaseDistanceResolver dist = new ReachableDistanceResolver(transform.position, navMeshAgent);
        return FindClosestObject(out closest, operativesObj, dist);
    }

    public bool FindClosestReachableSpawner(out GameObject closest)
    {
        BaseDistanceResolver dist = new ReachableDistanceResolver(transform.position, navMeshAgent);
        return FindClosestObject(out closest, spawners, dist);
    }

    public bool FindClosestObject(out GameObject closest, GameObject[] objects, BaseDistanceResolver distance)
    {
        float minDist = float.MaxValue;
        closest = null;

        foreach (GameObject obj in objects)
        {
            float dist = distance.Get(obj.transform.position);
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
        navMeshAgent.acceleration = 32;
    }

    public void SetRunSpeed()
    {
        navMeshAgent.speed = runSpeed;
        navMeshAgent.acceleration = 8;
    }

    public void SetEscapeSpeed()
    {
        navMeshAgent.speed = escapeSpeed;
        navMeshAgent.acceleration = 16;
    }

    public void SetSpawnController(SpawnerControll _spawnerController)
    {
        spawnerController = _spawnerController;
    }

    public bool isExposed()
    {
        bool exposed = false;
        List<Transform> visibleTargets;
        foreach(Unit operative in operatives)
        {
            visibleTargets = operative.fieldOfView.GetVisibleTargets();
            if(exposed = visibleTargets.Contains(transform)) { break; }
        }
        return exposed;
    }

    public GameObject GetTarget()
    {
        return target;
    }

    public void StopNav()
    {
        navMeshAgent.isStopped = true;
    }

    public void StartNav()
    {
        navMeshAgent.isStopped = false;
    }

    public void Angry(float value = .1f)
    {
        angryLevel += value;
        angryLevel = Mathf.Clamp(angryLevel, 0, 1);
        Debug.Log(string.Format("Alien getting angry {0}", angryLevel));
    }
    public void Chill(float value = .1f)
    {
        angryLevel -= value;
        angryLevel = Mathf.Clamp(angryLevel, 0, 1);
        Debug.Log(string.Format("Alien chill {0}", angryLevel));
    }
}