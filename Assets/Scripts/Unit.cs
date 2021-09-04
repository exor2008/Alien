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

    protected Vector3 destination;
    protected ScreenControll screenControll;

    void Start()
    {
        destination = transform.position;
        screenControll = screens.GetComponent<ScreenControll>();
    }

    void Update()
    {

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
    }
}
