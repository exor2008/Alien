using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ApproachableObject : MonoBehaviour
{
    public GameObject[] approaches;
    public bool GetClosestApproach(Vector3 position, NavMeshAgent navAgent, out GameObject closest)
    {
        return Find.ClosestReachableObject(position, navAgent, approaches, out closest);
    }
}
