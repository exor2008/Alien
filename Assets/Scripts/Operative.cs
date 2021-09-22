using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.OperativeStatesNamespace;


public class Operative : Unit
{
    public DeadBodiesControl deadBodiesControl;
    public OperativesControl operativeControll;
    public ScreenControll screenControll;
    public GameObject screens;
    public bool isCurrent;
    public int serialNumber;
    public FieldOfView fieldOfView;
    public bool isAlive;

    [HideInInspector]
    public bool isRotateNeeded { get; set; }
    
    protected GameObject toInteract;

    void Start()
    {
        stateManager = new StateManager(new IdleState(this));
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

    public void SetCurrent(bool current)
    {
        isCurrent = current;
    }

    public void SmoothLookAt(Quaternion lookRotation)
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5);
    }    
    public void Die()
    {
        deadBodiesControl.SpawnDeadBody(transform);
        navAgent.Warp(new Vector3(130, 2, -20));
        StopNav();
        screenControll.ShutDown(serialNumber);
        fieldOfView.Die();
        isAlive = false;
    }
    public string Name()
    {
        return string.Format("Operative {0}", serialNumber);
    }

}
