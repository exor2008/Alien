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

    protected Vector3 destination;
    protected ScreenControll screenControll;
    protected GameObject toInteract;

    void Start()
    {
        destination = transform.position;
        screenControll = screens.GetComponent<ScreenControll>();
    }

    void Update()
    {
        Interact();
    }

    private void Interact()
    {
        if (toInteract != null && Distance(toInteract.transform) < 2)
        {
            Interactable item = toInteract.GetComponent<Interactable>();
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
        navAgent.SetDestination(destination);
    }

    public void Die()
    {
        navAgent.Warp(new Vector3(130, 2, -20));
        navAgent.isStopped = true;
        screenControll.ShutDown(serialNumber);
        fieldOfView.Die();
    }

    public void InteractWith(GameObject obj, Vector3 position)
    {
        position.y = transform.position.y;
        SetDestination(position);
        toInteract = obj;
    }


}
