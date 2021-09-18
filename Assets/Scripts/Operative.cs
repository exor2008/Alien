using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Game.OperativeStatesNamespace;


public class Operative : MonoBehaviour
{
    public NavMeshAgent navAgent;
    public GameObject screens;
    public bool isCurrent;
    public int serialNumber;
    public FieldOfView fieldOfView;
    public bool isAlive;
    public Vector3 TargetRotation 
    { 
        get => targetRotation; 
        set 
        {
            targetRotation = value;
            targetRotation.y = transform.position.y;
        } 
    }
    public Vector3 Destination 
    { 
        get => destination; 
        set
        {
            destination = value;
            destination.y = transform.position.y;
            navAgent.SetDestination(destination);
        }
    }

    [HideInInspector]
    public bool isRotateNeeded { get; set; }
    protected Vector3 destination;
    protected Vector3 targetRotation;
    protected ScreenControll screenControll;
    protected GameObject toInteract;
    protected Coroutine moveAndRotateCoroutine;
    protected StateManager stateManager;

    void Start()
    {
        destination = transform.position;
        targetRotation = transform.forward;
        screenControll = screens.GetComponent<ScreenControll>();
        stateManager = new StateManager(new IdleState(this));
        StopNav();
        isAlive = true;
    }
    void Update()
    {
        stateManager.Updtae();
    }
    public void OnTriggerStay(Collider other)
    {
        if (toInteract && toInteract == other.transform.parent?.gameObject)
        {
            SwitchState(new InteractState(this, toInteract));
        }
    }
    public void SetObjectToInteract(GameObject obj)
    {
        toInteract = obj;
    }
    public float Distance(Transform other)
    {
        Vector3 from = Vector3.Scale(transform.position, new Vector3(1, 0, 1));
        Vector3 to = Vector3.Scale(other.position, new Vector3(1, 0, 1));
        return Vector3.Distance(from, to);
    }

    public void SetCurrent(bool current)
    {
        isCurrent = current;
    }

    public bool IsSameAsDestination(Vector3 target)
    {
        return Vector3.Distance(destination, target) <= 1;
    }

    public void SmoothLookAt(Quaternion lookRotation)
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5);
    }
    public void SwitchState(State state)
    {
        stateManager.SwitchState(state);
    }
    
    public void Die()
    {
        navAgent.Warp(new Vector3(130, 2, -20));
        StopNav();
        screenControll.ShutDown(serialNumber);
        fieldOfView.Die();
        isAlive = false;
    }

    public bool IsDestinationReached()
    {
        if (navAgent.pathPending) 
        { 
            return false; 
        }
        return navAgent.remainingDistance < .5;
    }
    public bool IsRotationFinished(out Vector3 dir)
    {
        dir = (TargetRotation - transform.position).normalized;
        return dir == Vector3.zero;
    }
    public void StopNav()
    {
        navAgent.isStopped = true;
    }

    public void StartNav()
    {
        navAgent.isStopped = false;
    }

    public string Name()
    {
        return string.Format("Operative {0}", serialNumber);
    }

}
