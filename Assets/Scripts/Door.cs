using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Door : ApproachableObject, Interactable, Breakable
{
    public Material automateMaterial;
    public Material manualMaterial;
    public Material unpoweredMaterial;
    
    public GameObject door;

    public bool isAutotomate;
    public bool isPowered;
    public bool isOpened;

    MeshRenderer meshRenderer;

    void Start()
    {
        meshRenderer = door.GetComponent<MeshRenderer>();
    }

    void Update()
    {
        
    }
    public void Open()
    {
        if (!isOpened)
        {
            door.transform.position += Vector3.up * 4;
            isOpened = true;
        }
    }

    public void Close()
    {
        if (isOpened)
        {
            door.transform.position -= Vector3.up * 4;
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

    public void Break(GameObject who)
    {
        Open();
        SetUnpowered();
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
}