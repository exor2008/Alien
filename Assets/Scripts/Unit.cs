using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    protected Vector3 destination;
    protected Vector3 targetRotation;
    protected StateManager stateManager;

    void Start()
    {
        destination = transform.position;
        targetRotation = transform.forward;
        StopNav();
    }

    public NavMeshAgent navAgent;
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
    public void SwitchState(State state)
    {
        stateManager.SwitchState(state);
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
    public bool IsSameAsDestination(Vector3 target)
    {
        return Vector3.Distance(destination, target) <= 1;
    }
    public void StopNav()
    {
        navAgent.isStopped = true;
    }

    public void StartNav()
    {
        navAgent.isStopped = false;
    }
    public bool isPathComplete()
    {
        return navAgent.path.status == NavMeshPathStatus.PathComplete;
    }
}
