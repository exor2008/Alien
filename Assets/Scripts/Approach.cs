using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Approach : MonoBehaviour
{
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(1, 1, 1));
    }
}
