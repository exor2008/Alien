using System;
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
        return UnityEngine.Random.value;
    }

    public static void Shuffle<T>(T[] array)
    {
        System.Random rnd = new System.Random();
        int n = array.Length;
        while (n > 1)
        {
            int k = rnd.Next(n--);
            T temp = array[n];
            array[n] = array[k];
            array[k] = temp;
        }
    }

    public static T[] GetComponentArray<T>(GameObject[] objects)
    {
        T[] components = new T[objects.Length];
        for (int i = 0; i < objects.Length; i++)
        {
            components[i] = objects[i].GetComponent<T>();
        }
        return components;
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
        if(navAgent.CalculatePath(dest, path))
        {
            return Find.PathLength(path);
        }
        return Mathf.Infinity;
    }
}

public enum MouseButon
{
    Up,
    Down
}

static class Find
{
    static bool _ClosestObject(out GameObject closest, GameObject[] objects, BaseDistanceResolver distance)
    {
        float minDist = float.MaxValue;
        closest = null;

        foreach (GameObject obj in objects)
        {
            float dist = distance.Get(obj.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = obj;
            }
        }
        if (closest == null)
            return false;
        else
        {
            return true;
        }
    }
    static bool _Distances(GameObject[] objects, BaseDistanceResolver distance, out float[] distances)
    {
        bool isAnyDist = false;
        distances = new float[objects.Length];
        for (int i = 0; i < objects.Length; i++)
        {
            distances[i] = distance.Get(objects[i].transform.position);
            if(!float.IsInfinity(distances[i]))
            {
                isAnyDist = true;
            }
        }
        return isAnyDist;
    }

    public static bool Distances(Vector3 from, NavMeshAgent navMeshAgent, GameObject[] objects, out float[] distances)
    {
        BaseDistanceResolver dist = new DistanceResolver(from, navMeshAgent);
        return _Distances(objects, dist, out distances);
    }

    public static bool ReachableDistances(Vector3 from, NavMeshAgent navMeshAgent, GameObject[] objects, out float[] distances)
    {
        BaseDistanceResolver dist = new ReachableDistanceResolver(from, navMeshAgent);
        return _Distances(objects, dist, out distances);
    }

    public static bool ClosestReachableObject(Vector3 from, NavMeshAgent navMeshAgent, GameObject[] objects, out GameObject closest)
    {
        BaseDistanceResolver dist = new ReachableDistanceResolver(from, navMeshAgent);
        return _ClosestObject(out closest, objects, dist);
    }

    public static bool ClosestObject(Vector3 from, NavMeshAgent navMeshAgent, GameObject[] objects, out GameObject closest)
    {
        BaseDistanceResolver dist = new DistanceResolver(from, navMeshAgent);
        return _ClosestObject(out closest, objects, dist);
    }
    public static float PathLength(NavMeshPath path)
    {
        float distance = 0.0f;

        if ((path.status == NavMeshPathStatus.PathInvalid) || (path.status == NavMeshPathStatus.PathPartial))
        {
            return Mathf.Infinity;
        }
        if (path.corners.Length > 1)
        {
            for (int i = 1; i < path.corners.Length; ++i)
            {
                distance += Vector3.Distance(path.corners[i - 1], path.corners[i]);
            }
        }
        return distance;
    }

    static bool _OneOfNClosest(
        int n, Vector3 from, NavMeshAgent navMeshAgent, GameObject[] objects, BaseDistanceResolver dist,  out GameObject closest)
    {
        bool found = false;
        closest = null;

        List<GameObjectDist> foundObjects = new List<GameObjectDist>(objects.Length);
        float[] distances;
        if (Find._Distances(objects, dist, out distances))
        {
            for (int i = 0; i < objects.Length; i++)
            {
                if (!float.IsInfinity(distances[i]))
                {
                    foundObjects.Add(new GameObjectDist(objects[i], distances[i]));
                    found = true;
                }
            }
            if (!found)
            {
                return false;
            }
            foundObjects.Sort((x, y) => x.distance.CompareTo(y.distance));

            int maxRandomIdx = Mathf.Min(n, foundObjects.Count - 1);

            int chosenSpawnerIdx = UnityEngine.Random.Range(0, maxRandomIdx);
            closest = foundObjects[chosenSpawnerIdx].obj;
        }
        return found;
    }
    public static bool OneOfNClosest(
        int n, Vector3 from, NavMeshAgent navMeshAgent, GameObject[] objects, out GameObject closest)
    {
        BaseDistanceResolver dist = new DistanceResolver(from, navMeshAgent);
        return _OneOfNClosest(n, from, navMeshAgent, objects, dist, out closest);
    }
    public static bool OneOfNReachableClosest(
    int n, Vector3 from, NavMeshAgent navMeshAgent, GameObject[] objects, out GameObject closest)
    {
        BaseDistanceResolver dist = new ReachableDistanceResolver(from, navMeshAgent);
        return _OneOfNClosest(n, from, navMeshAgent, objects, dist, out closest);
    }
    public static int ArgMin(float[] array)
    {
        if (array.Length == 0)
        {
            throw new ArgumentException("List is empty.", "self");
        }

        int minIndex = 0;
        float min = array[0];
        for (int i = 1; i < array.Length; i++)
        {
            if (array[i] < min)
            {
                min = array[i];
                minIndex = i;
            }
        }
        return minIndex;
    }
}

public class GameObjectDist
{
    public GameObject obj;
    public float distance;

    public GameObjectDist(GameObject _obj, float _distance)
    {
        obj = _obj;
        distance = _distance;
    }
}