using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Unit : MonoBehaviour
{
    public NavMeshAgent navAgent;
    public GameObject screens;
    public bool isCurrent;
    public int serialNumber;
    public FieldOfView fieldOfView;
    public bool isAlive;

    [HideInInspector]
    public bool isRotateNeeded { get; set; }
    protected Vector3 destination;
    protected Vector3 targetRotation;
    protected ScreenControll screenControll;
    protected GameObject toInteract;
    protected Coroutine moveAndRotateCoroutine;

    void Start()
    {
        destination = transform.position;
        targetRotation = transform.forward;
        screenControll = screens.GetComponent<ScreenControll>();
        isAlive = true;
    }

    public void OnTriggerStay(Collider other)
    {
        if (toInteract == other.transform.parent.gameObject)
        {
            Interactable item = other.GetComponentInParent<Interactable>();
            item.Interact(gameObject);
            toInteract = null;
        }
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

    public void SetDestination(Vector3 _destination)
    {
        destination = _destination;
    }
    public bool IsSameAsDestination(Vector3 target)
    {
        return Vector3.Distance(destination, target) <= 1;
    }
    public void SetTargetRotation(Vector3 _targetRotation)
    {
        targetRotation = _targetRotation;
    }
    public void MoveAndRotate()
    {
        navAgent.isStopped = false;
        navAgent.SetDestination(destination);
        StopMoveOrRotateCoroutine();
        moveAndRotateCoroutine = StartCoroutine(MoveAndRotateCoroutine());
    }
    public void Rotate()
    {
        navAgent.isStopped = true;
        StopMoveOrRotateCoroutine();
        moveAndRotateCoroutine = StartCoroutine(RotateCoroutine());
    }
    void StopMoveOrRotateCoroutine()
    {
        if (moveAndRotateCoroutine != null)
        {
            StopCoroutine(moveAndRotateCoroutine);
        }
    }
    public IEnumerator MoveAndRotateCoroutine()
    {
        while (!IsDestinationReached())
        {
            yield return new WaitForSeconds(0.3f);
        }
        navAgent.isStopped = true;

        if (isRotateNeeded)
        {
            Rotate();
        }
    }
    public IEnumerator RotateCoroutine()
    {
        Quaternion lookRotation = Quaternion.identity;
        while (!IsRotationFinished(lookRotation))
        {
            targetRotation.y = transform.position.y;
            Vector3 dir = (targetRotation - transform.position).normalized;
            lookRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5);

            yield return new WaitForSeconds(0.01f);
        }
    }
    public void Die()
    {
        navAgent.Warp(new Vector3(130, 2, -20));
        navAgent.isStopped = true;
        screenControll.ShutDown(serialNumber);
        fieldOfView.Die();
        isAlive = false;
    }

    public void InteractWith(GameObject obj, Vector3 position)
    {
        position.y = transform.position.y;
        SetDestination(position);
        isRotateNeeded = false;
        MoveAndRotate();
        toInteract = obj;
    }
    public bool IsDestinationReached()
    {
        if (navAgent.pathPending) 
        { 
            return false; 
        }
        return navAgent.remainingDistance < .5;
    }
    public bool IsRotationFinished(Quaternion lookRotation)
    {
        return Quaternion.Angle(transform.rotation, lookRotation) < 1e-6;
    }

}
