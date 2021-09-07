using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperativesControl : MonoBehaviour
{
    const int OPERATIVES_COUNT = 4;
    public GameObject[] operatives;
    protected Unit[] units = new Unit[OPERATIVES_COUNT];
    public int currentOperativeIdx;

    void Start()
    {
        currentOperativeIdx = 0;
        for (int i = 0; i < OPERATIVES_COUNT; i++)
        {
            GameObject operative = operatives[i];
            Unit unit = operative.GetComponent<Unit>();
            units[i] = unit;
        }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && (Input.GetKeyDown(KeyCode.Alpha1)))
        {
            SwitchCurrentOperative(0);
        }
        else if (Input.GetKey(KeyCode.LeftShift) && (Input.GetKeyDown(KeyCode.Alpha2)))
        {
            SwitchCurrentOperative(1);
        }
        else if (Input.GetKey(KeyCode.LeftShift) && (Input.GetKeyDown(KeyCode.Alpha3)))
        {
            SwitchCurrentOperative(2);
        }
        else if (Input.GetKey(KeyCode.LeftShift) && (Input.GetKeyDown(KeyCode.Alpha4)))
        {
            SwitchCurrentOperative(3);
        }
    }

    void SwitchCurrentOperative(int activeIdx)
    {
        units[currentOperativeIdx].SetCurrent(false);
        currentOperativeIdx = activeIdx;
        units[currentOperativeIdx].SetCurrent(true);
    }

    public GameObject GetCurrentOperative()
    {
        return operatives[currentOperativeIdx];
    }

    public GameObject GetOperative(int index)
    {
        return operatives[index];
    }
}