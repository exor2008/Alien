using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class Utils
{
    public static Vector3 DirFromAngle(float angle, float angleDeg, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleDeg += angle;
        }

        float x = Mathf.Sin(angleDeg * Mathf.Deg2Rad);
        float z = Mathf.Cos(angleDeg * Mathf.Deg2Rad);
        return new Vector3(x, 0, z);
    }

    public static Quaternion FacePoint(Rigidbody rb, Transform transform, Vector3 target)
    {
        Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);
        rb.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 7f * Time.fixedDeltaTime);
        return targetRotation;
    }

    public static float Rand()
    {
        return Random.value;
    }
}

public abstract class BaseDistanceResolver
{
    protected Vector3 src;
    protected NavMeshAgent navAgent;
    public BaseDistanceResolver(Vector3 _src, NavMeshAgent _navAgent)
    {
        src = _src;
        navAgent = _navAgent;
    }
    public abstract float Get(Vector3 dest);
}

public class DistanceResolver : BaseDistanceResolver
{
    public DistanceResolver(Vector3 _src, NavMeshAgent _navAgent)
        : base(_src, _navAgent) { }

    public override float Get(Vector3 dest)
    {
        return Vector3.Distance(src, dest);
    }
}

public class ReachableDistanceResolver : BaseDistanceResolver
{
    NavMeshPath path;
    public ReachableDistanceResolver(Vector3 _src, NavMeshAgent _navAgent)
        : base(_src, _navAgent) 
    {
        path = new NavMeshPath();
    }

    public override float Get(Vector3 dest)
    {
        if(navAgent.CalculatePath(dest, path) && path.status == NavMeshPathStatus.PathComplete)
        {
            return Vector3.Distance(src, dest);
        }
        return Mathf.Infinity;
    }
}

public enum MouseButon
{
    Up,
    Down
}