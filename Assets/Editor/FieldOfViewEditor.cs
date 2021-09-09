using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FieldOfView))]
public class FieldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        FieldOfView fow = (FieldOfView)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fow.transform.position, Vector3.up, Vector3.forward, 360, fow.viewRaius);

        float unitAngle = fow.transform.eulerAngles.y;
        Vector3 viewAngleA = Utils.DirFromAngle(unitAngle, -fow.viewAngle / 2, false);
        Vector3 viewAngleB = Utils.DirFromAngle(unitAngle, fow.viewAngle / 2, false);

        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleA * fow.viewRaius);
        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleB * fow.viewRaius);

        Handles.color = Color.green;

        foreach (Transform visTarget in fow.GetVisibleTargets())
        {
            Handles.DrawLine(fow.transform.position, visTarget.position);
        }
    }
}
