using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Game.AlienStatesNamespace;

public class Alien : MonoBehaviour
{
    //public GameObject[] operativesObj;
    public OperativesControl operativesControl;
    public DoorControl doorControl;
    public GameObject[] spawners;
    public NavMeshAgent navMeshAgent;
    public float runSpeed;
    public float jumpSpeed;
    public float escapeSpeed;
    public float jumpDistance;
    public float killDistance;
    public bool isSpawned;
    public float angryLevel;
    public float chillCooldown;

    public Vector3 Destination
    {
        get => destination;
        set
        {
            destination = value;
            destination.y = transform.position.y;
            navMeshAgent.SetDestination(destination);
        }
    }
    public GameObject ToBreak { get; set; }

    Vector3 destination;
    [HideInInspector]
    public ReactionResolver reactionResolver;
    Operative[] operatives = new Operative[4];
    SpawnerControll spawnerController;
    StateManager stateManager;
    GameObject target;
    

    void Start()
    {
        stateManager = new StateManager(new IdleState(this));
        reactionResolver = new ReactionResolver(this);
        //for (int i = 0; i < operativesObj.Length; i++)
        //{
        //    operatives[i] = operativesObj[i].GetComponent<Operative>();
        //}
        StartCoroutine(ChillOverTime(chillCooldown));
    }

    void Update()
    {
        stateManager.Updtae();
    }
    public void OnTriggerStay(Collider other)
    {
        if (ToBreak == other.transform.parent.gameObject)
        {
            SwitchState(new BreakState(this, ToBreak));
        }
    }
    public void MoveToClosestReachableTarget()
    {
        GameObject[] operativesObj = operativesControl.GetAliveOperativesObjects();
        MoveToClosestReachableObject(operativesObj);
    }

    public void MoveToClosestReachableSpawner()
    {
        MoveToClosestReachableObject(spawners);
    }
    public void MoveToClosestReachableObject(GameObject[] objects)
    {
        BaseDistanceResolver dist = new ReachableDistanceResolver(transform.position, navMeshAgent);
        if (Find.ClosestReachableObject(transform.position, navMeshAgent, objects, out target))
        {
            navMeshAgent.SetDestination(target.transform.position);
        }
    }

    public bool FindClosestReachableTarget(out GameObject closest)
    {
        return Find.ClosestReachableObject(
            transform.position, navMeshAgent, operativesControl.operativesObjects, out closest);
    }

    public bool FindClosestReachableSpawner(out GameObject closest)
    {
        return Find.ClosestReachableObject(
            transform.position, navMeshAgent, spawners, out closest);
    }

    public bool FindClosestReachableClosedDoor(out GameObject closestDoor)
    {
        List<Door> doors = doorControl.GetDoors().ToList();
        doors = doors.Where(x => !x.isOpened).ToList();
        GameObject[] doorObjects = doors.Select(door => door.gameObject).ToArray();

        return Find.ClosestObject(
            transform.position,
            navMeshAgent,
            doorObjects,
            out closestDoor);
    }

    public void Spawn(Vector3 position)
    {
        // TODO: сдклать SpawnState вместо этого
        navMeshAgent.Warp(position);
        isSpawned = true;
        GameObject closestDoor;
        if (FindClosestReachableClosedDoor(out closestDoor))
        {
            Door door = closestDoor.GetComponent<Door>();
            GameObject approach = door.GetClosestApproach(transform.position, navMeshAgent);
            if (approach)
            {
                SwitchState(new GoToBreakState(this, closestDoor, approach.transform.position));
                return;
            }
        }

        SwitchState(new ChooseVictim(this));

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
            return hit.collider == target?.GetComponent<Collider>();
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
        foreach(Operative operative in operatives)
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

    IEnumerator ChillOverTime(float cooldown)
    {
        Chill();
        yield return new WaitForSeconds(cooldown);
    }
}