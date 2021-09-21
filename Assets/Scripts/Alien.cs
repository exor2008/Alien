using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Game.AlienStatesNamespace;

public class Alien : Unit
{
    public OperativesControl operativesControl;
    public DoorControl doorControl;
    public GameObject[] spawners;
    public RoamPointsControl roamPointsControl;
    public float runSpeed;
    public float jumpSpeed;
    public float escapeSpeed;
    public float jumpDistance;
    public float killDistance;
    public bool isSpawned;
    public float angryLevel;
    public float chillCooldown;

    public GameObject Target 
    { 
        get => _target; 
        set => _target = value; 
    }
    GameObject _target;

    public GameObject EscapeSpawner { get; set; }
    public GameObject ToBreak { get; set; }
    [HideInInspector]
    public ReactionResolver reactionResolver;
    SpawnerControll spawnerController;

    void Start()
    {
        stateManager = new StateManager(new IdleState(this));
        reactionResolver = new ReactionResolver(this);
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
        else if (EscapeSpawner == other.gameObject)
        {
            SwitchState(new HideState(this));
        }
        else if (Target == other.gameObject)
        {
            SwitchState(new KillState(this, Target.GetComponent<Operative>()));
        }
    }

    public bool FindClosestReachableTarget(out GameObject closest)
    {
        return Find.ClosestReachableObject(
            transform.position, navAgent, operativesControl.GetAliveOperativesObjects(), out closest);
    }

    public bool FindClosestReachableSpawner(out GameObject closest)
    {
        return Find.ClosestReachableObject(
            transform.position, navAgent, spawners, out closest);
    }

    public bool FindClosestReachableClosedDoor(out GameObject closestDoor)
    {
        List<Door> doors = doorControl.GetDoors().ToList();
        doors = doors.Where(x => !x.isOpened).ToList();

        GameObject[] approaches = doors.SelectMany(door => door.approaches).ToArray();

        GameObject closestApproach;
        if(Find.ClosestReachableObject(
            transform.position,
            navAgent,
            approaches,
            out closestApproach))
        {
            closestDoor = closestApproach.transform.parent.gameObject;
            return true;
        }
        closestDoor = null;
        return false;
    }

    public void Spawn(Vector3 position)
    {
        navAgent.Warp(position);
        isSpawned = true;
        SwitchState(new SpawnState(this));
    }

    public void Hide()
    {
        navAgent.Warp(new Vector3(130, 0, -20));
        isSpawned = false;
        spawnerController?.UpdateTimeSpawn();
    }

    public bool isTargetVisible(out RaycastHit hit)
    {
        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            return hit.collider == Target?.GetComponent<Collider>();
        }
        return false;
    }

    public void SetJumpSpeed()
    {
        navAgent.speed = jumpSpeed;
        navAgent.acceleration = 32;
    }

    public void SetRunSpeed()
    {
        navAgent.speed = runSpeed;
        navAgent.acceleration = 8;
    }

    public void SetEscapeSpeed()
    {
        navAgent.speed = escapeSpeed;
        navAgent.acceleration = 16;
    }

    public void SetSpawnController(SpawnerControll _spawnerController)
    {
        spawnerController = _spawnerController;
    }

    public bool isExposed()
    {
        bool exposed = false;
        List<Transform> visibleTargets;
        foreach(Operative operative in operativesControl.GetAliveOperatives())
        {
            visibleTargets = operative.fieldOfView.GetVisibleTargets();
            if(exposed = visibleTargets.Contains(transform)) { break; }
        }
        return exposed;
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

    public GameObject[] GetRoamPoints()
    {
        return roamPointsControl.GetPointsObjects();
    }
}