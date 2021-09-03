using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Screens
{
    Main,
    Operative1,
    Operative2,
    Operative3,
    Operative4,
}
public class ScreenControll : MonoBehaviour
{
    public GameObject[] screens;
    public Material[] screenMaterials;

    public Camera topCamera;
    public Camera viewCamera;
    
    public Collider screenCollider;

    MeshRenderer mainScreenMR;
    bool isControllable;

    void Start()
    {
        isControllable = true;
        InitScreens();
        mainScreenMR = screens[(int)Screens.Main].GetComponent<MeshRenderer>();
    }
    void InitScreens()
    {
        MeshRenderer meshRenderer;
        for (int i = 0; i < screenMaterials.Length; i++)
        {
            meshRenderer = screens[i].GetComponent<MeshRenderer>();
            meshRenderer.material = screenMaterials[i];
        }
    }

    void Update()
    {
        if (!Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha1))
        {
            mainScreenMR.material = screenMaterials[(int)Screens.Operative1];
            isControllable = false;
        }
        else if (!Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha2))
        {
            mainScreenMR.material = screenMaterials[(int)Screens.Operative2];
            isControllable = false;
        }
        else if (!Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha3))
        {
            mainScreenMR.material = screenMaterials[(int)Screens.Operative3];
            isControllable = false;
        }
        else if (!Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha4))
        {
            mainScreenMR.material = screenMaterials[(int)Screens.Operative4];
            isControllable = false;
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            mainScreenMR.material = screenMaterials[(int)Screens.Main];
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

    public void ShutDown(int index)
    {
        //TODO: raise up noise
    }
}
