using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenControll : MonoBehaviour
{
    public Material MainScreenMaterial;
    public RenderTexture MainRT;

    public RenderTexture Operative1RT;
    public RenderTexture Operative2RT;
    public RenderTexture Operative3RT;
    public RenderTexture Operative4RT;

    public Camera topCamera;
    public Camera viewCamera;
    
    public Collider screenCollider;

    bool isControllable;

    void Start()
    {
        isControllable = true;
    }

    void Update()
    {
        if (!Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha1))
        {
            MainScreenMaterial.mainTexture = Operative1RT;
            isControllable = false;
        }
        else if (!Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha2))
        {
            MainScreenMaterial.mainTexture = Operative2RT;
            isControllable = false;
        }
        else if (!Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha3))
        {
            MainScreenMaterial.mainTexture = Operative3RT;
            isControllable = false;
        }
        else if (!Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha4))
        {
            MainScreenMaterial.mainTexture = Operative4RT;
            isControllable = false;
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            MainScreenMaterial.mainTexture = MainRT;
            isControllable = true;
        }
    }

    public Vector3? GetDestinationByClick()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Ray clickRay = viewCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(clickRay, out hit) && (hit.collider == screenCollider) && isControllable)
            {
                // we hit the main screen area
                Vector3 min = screenCollider.bounds.min;
                Vector3 max = screenCollider.bounds.max;

                float coefX = (hit.point.z - min.z) / (max.z - min.z);
                float coefY = (hit.point.y - min.y) / (max.y - min.y);

                float newX = Screen.width * coefX;
                float newZ = Screen.height * coefY;

                Vector3 newPoint = new Vector3(newX, newZ, 0);

                Vector3 destination = topCamera.ScreenToWorldPoint(newPoint);
                destination.y = 2;
                return destination;
            }
            return null;
        }
        return null;
    }
}
