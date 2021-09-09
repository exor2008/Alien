using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float viewRaius;
    [Range(0, 360)]
    public float viewAngle;
    public LayerMask targetMak;
    public LayerMask obstacleMask; 

    protected List<Transform> visibleTargets = new List<Transform>();

    public void Start()
    {
        StartCoroutine(FindTargetsWithDelay(.3f));
    }

    public IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }
    public void FindVisibleTargets()
    {
        visibleTargets.Clear();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(
            transform.position,
            viewRaius,
            targetMak);

        foreach (Collider ctarget in targetsInViewRadius)
        {
            Transform target = ctarget.transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float distToTarget = Vector3.Distance(transform.position, target.position);
                if (!Physics.Raycast(transform.position, dirToTarget, distToTarget, obstacleMask))
                {
                    visibleTargets.Add(target);
                }
            }
        }
    }
    public List<Transform> GetVisibleTargets()
    {
        return visibleTargets;
    }
}
