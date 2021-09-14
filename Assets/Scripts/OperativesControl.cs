using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class OperativesControl : MonoBehaviour
{
    public GameObject[] operativesObjects;
    protected Unit[] operatives;
    public int currentOperativeIdx;

    void Start()
    {
        currentOperativeIdx = 0;
        operatives = Utils.GetComponentArray<Unit>(operativesObjects);
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
        operatives[currentOperativeIdx].SetCurrent(false);
        currentOperativeIdx = activeIdx;
        operatives[currentOperativeIdx].SetCurrent(true);
    }

    public GameObject GetCurrentOperative()
    {
        return operativesObjects[currentOperativeIdx];
    }

    public GameObject GetOperative(int index)
    {
        return operativesObjects[index];
    }

    public Unit[] GetAliveOperatives()
    {
        List<Unit> alive = new List<Unit>(operatives);
        alive.Where(x => x.isAlive);
        return alive.ToArray();
    }

    public Unit[] GetAliveShuffledOperatives()
    {
        System.Random rand = new System.Random();

        List<Unit> alive = new List<Unit>(operatives);
        alive.Where(x => x.isAlive);
        List<Unit> aliveShufled = alive.OrderBy(x => rand.Next()).ToList();
        return aliveShufled.ToArray();
    }

    public Unit[] GetOperatives()
    {
        return operatives;
    }
}