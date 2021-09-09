using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
