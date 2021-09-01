using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Unit : MonoBehaviour
{
    public NavMeshAgent navAgent;
    public GameObject screens;
    public bool isCurrent;

    protected Vector3 destination;
    protected ScreenControll screenControll;

    void Start()
    {
        destination = transform.position;
        screenControll = screens.GetComponent<ScreenControll>();
    }

    void Update()
    {
        if (isCurrent)
        {
            Vector3? dest = screenControll.GetDestinationByClick();
            if (dest != null)
            { 
                destination = (Vector3)dest; 
            }
            
        }
        navAgent.SetDestination(destination);
    }

    public void SetCurrent(bool current)
    {
        isCurrent = current;
    }

    public void Die()
    {
        navAgent.Warp(new Vector3(130, 2, -20));
        navAgent.isStopped = true;
    }
}
