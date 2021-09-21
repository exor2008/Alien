using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ApproachableObject : MonoBehaviour
{
    public GameObject[] approaches;
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
