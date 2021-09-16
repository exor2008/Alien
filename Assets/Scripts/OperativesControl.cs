using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class OperativesControl : MonoBehaviour
{
    public GameObject[] operativesObjects;
    protected Operative[] operatives;
    public int currentOperativeIdx;

    void Start()
    {
        currentOperativeIdx = 0;
        operatives = Utils.GetComponentArray<Operative>(operativesObjects);
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

    public Operative[] GetAliveOperatives()
    {
        List<Operative> alive = new List<Operative>(operatives);
        alive.Where(x => x.isAlive);
        return alive.ToArray();
    }

    public GameObject[] GetAliveOperativesObjects()
    {
        List<GameObject> alive = new List<GameObject>(operativesObjects);
        alive.Where(x => x.GetComponent<Operative>().isAlive);
        return alive.ToArray();
    }

    public Operative[] GetAliveShuffledOperatives()
    {
        System.Random rand = new System.Random();

        List<Operative> alive = new List<Operative>(operatives);
        alive.Where(x => x.isAlive);
        List<Operative> aliveShufled = alive.OrderBy(x => rand.Next()).ToList();
        return aliveShufled.ToArray();
    }

    public Operative[] GetOperatives()
    {
        return operatives;
    }
}