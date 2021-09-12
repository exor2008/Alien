using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Door : MonoBehaviour, Interactable
{
    public Material automateMaterial;
    public Material manualMaterial;
    public Material unpoweredMaterial;
    public GameObject[] approaches;

    public bool isAutotomate;
    public bool isPowered;
    public bool isOpened;

    MeshRenderer meshRenderer;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        
    }

    void MoveTriggers(Vector3 value)
    {
        foreach(GameObject approach in approaches)
        {
            approach.transform.position += value;
        }
    }
    public void Open()
    {
        if (!isOpened)
        {
            transform.position += Vector3.up * 4;
            MoveTriggers(-Vector3.up * 4);
            isOpened = true;
        }
    }

    public void Close()
    {
        if (isOpened)
        {
            transform.position -= Vector3.up * 4;
            MoveTriggers(Vector3.up * 4);
            isOpened = false;
        }
    }

    public void Switch()
    {
        if (isOpened)
        {
            Close();
        }
        else 
        {
            Open();
        }
    }

    public void Interact(GameObject who)
    {
        Switch();
    }

    public void SetAutomate()
    {
        isAutotomate = true;
        isPowered = true;
        meshRenderer.material = automateMaterial;
    }

    public void SetManual()
    {
        isAutotomate = false;
        isPowered = true;
        meshRenderer.material = manualMaterial;
    }

    public void SetUnpowered()
    {
        isAutotomate = false;
        isPowered = false;
        meshRenderer.material = unpoweredMaterial;
    }

    public void SetPowered()
    {
        isPowered = true;
        if (isAutotomate)
        {
            SetAutomate();
        }
        else
        {
            SetManual();
        }
    }

    public GameObject GetClosestApproach(Vector3 position, NavMeshAgent navAgent)
    {
        GameObject closest;
        if (Find.ClosestReachableObject(position, navAgent, approaches, out closest))
        {
            return closest;
        }
        return null;
    }
}