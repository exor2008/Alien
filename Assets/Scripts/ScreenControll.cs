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
    public GameObject operativesControllObj;
    public GameObject[] screens;
    public Material[] screenMaterials;

    public Camera topCamera;
    public Camera viewCamera;
    
    public Collider screenCollider;

    OperativesControl operativesControl;
    MeshRenderer mainScreenMR;
    bool isControllable;
    Vector3 minBounds;
    Vector3 maxBounds;
    Door door;

    void Start()
    {
        isControllable = true;
        InitScreens();
        mainScreenMR = screens[(int)Screens.Main].GetComponent<MeshRenderer>();
        minBounds = screenCollider.bounds.min;
        maxBounds = screenCollider.bounds.max;
        operativesControl = operativesControllObj.GetComponent<OperativesControl>();
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
        OnClick();
    }

    public void OnClick()
    {
        RaycastHit hitScreen;
        if (Input.GetMouseButtonUp(0))
        {
            Ray clickRay = viewCamera.ScreenPointToRay(Input.mousePosition);

            // check if we hit the main screen area
            if (Physics.Raycast(clickRay, out hitScreen) && (hitScreen.collider == screenCollider) && isControllable)
            {
                ClickReaction reaction = GetReaction(hitScreen);
                reaction.React();
            }
        }
    }

    ClickReaction GetReaction(RaycastHit hitScreen)
    {
        
        RaycastHit hitObject;

        // resolve the point we clicked on the screen 
        Vector3 screenPoint = CameraToScreen(hitScreen);

        // convert point on the screen to world coordinates
        Ray viewRay = topCamera.ScreenPointToRay(screenPoint);

        // cast ray to see what object we hit
        if (Physics.Raycast(viewRay, out hitObject))
        {
            if ((door = hitObject.collider.GetComponent<Door>()) != null)
            {
                return new DoorReaction(hitObject);
            }
            else
            {
                return new FloorReaction(hitObject, operativesControl);
            }
        }
        return new NoReaction(hitObject);
    }

    Vector3 CameraToScreen(RaycastHit hit)
    {
        float coefX = (hit.point.z - minBounds.z) / (maxBounds.z - minBounds.z);
        float coefY = (hit.point.y - minBounds.y) / (maxBounds.y - minBounds.y);

        float newX = Screen.width * coefX;
        float newZ = Screen.height * coefY;

        return new Vector3(newX, newZ, 0);
    }

    public void ShutDown(int index)
    {
        //TODO: raise up noise
    }
}

public abstract class ClickReaction
{
    protected RaycastHit hitInfo;
    public ClickReaction(RaycastHit _hitInfo)
    {
        hitInfo = _hitInfo;
    }
    public abstract void React();
}

public class NoReaction : ClickReaction
{
    public NoReaction(RaycastHit hitInfo) : base(hitInfo) { }
    public override void React() { }
}

public class DoorReaction: ClickReaction
{
    public DoorReaction(RaycastHit hitInfo) : base(hitInfo) { }
    public override void React()
    {
        Door door = hitInfo.collider.GetComponent<Door>();
        door.Switch();
    }
}

public class FloorReaction : ClickReaction
{
    Camera topCamera;
    OperativesControl operativesControl;

    public FloorReaction(
        RaycastHit hitInfo, 
        OperativesControl _operativesControl) : base(hitInfo) 
    {
        operativesControl = _operativesControl;
    }
    public override void React()
    {
        Vector3 destination = hitInfo.point;
        destination.y = 2;
        GameObject operative = operativesControl.GetCurrentOperative();
        Unit unit = operative.GetComponent<Unit>();
        unit.SetDestination(destination);
    }
}