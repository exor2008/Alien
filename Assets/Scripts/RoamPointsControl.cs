using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoamPointsControl : MonoBehaviour
{
    RoamPoint[] roamPoints;
    GameObject[] roamPointsObjs;
    void Start()
    {
        roamPoints = GetComponentsInChildren<RoamPoint>();

        roamPointsObjs = new GameObject[roamPoints.Length];
        for (int i = 0; i < roamPoints.Length; i++)
        {
            roamPointsObjs[i] = roamPoints[i].gameObject;
        }

    }

    public GameObject[] GetPointsObjects()
    {
        return roamPointsObjs;
    }
}
