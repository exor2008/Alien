using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorControl : MonoBehaviour
{
    Door[] doors;
    void Start()
    {
        doors = GetComponentsInChildren<Door>();
        foreach (Door door in doors)
        {
            if (door.gameObject.name == "DoorPrefab (2)" || door.gameObject.name == "DoorPrefab (3)")
            {
                door.SetAutomate();
                continue;
            }
            OpenRandomDoor(door, .5f);
            RandomDoorAutomate(door, .5f);
            UnpowerRandomDoor(door, .2f);
        }
    }
    void OpenRandomDoor(Door door, float ratio)
    {
        if (Random.value < ratio)
        {
            door.Open();
        }
    }
    void RandomDoorAutomate(Door door, float ratio)
    {
        if (Random.value < ratio)
        {
            door.SetAutomate();
        }
        else
        {
            door.SetManual();
        }
    }
    void UnpowerRandomDoor(Door door, float ratio)
    {
        if (Random.value < ratio)
        {
            door.SetUnpowered();
        }
    }

    public GameObject[] GetDoorsObjects()
    {
        GameObject[] doorsObjs = new GameObject[doors.Length];
        for (int i = 0; i < doors.Length; i++)
        {
            doorsObjs[i] = doors[i].gameObject;
        }
        return doorsObjs;
    }
    public Door[] GetDoors()
    {
        return doors;
    }
}