using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoamPoint : MonoBehaviour
{
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, new Vector3(1, 1, 1));
    }
}
