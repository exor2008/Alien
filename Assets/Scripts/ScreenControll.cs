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

    Vector3 minBounds;
    Vector3 maxBounds;
    Door door;
    ActiveScreen activeScreen;

    const int FIRST_OPERATIVE = 0;
    const int SECOND_OPERATIVE = 1;
    const int THIRD_OPERATIVE = 2;
    const int FOURTH_OPERATIVE = 3;

    void Start()
    {
        InitScreens();
        mainScreenMR = screens[(int)Screens.Main].GetComponent<MeshRenderer>();
        minBounds = screenCollider.bounds.min;
        maxBounds = screenCollider.bounds.max;
        operativesControl = operativesControllObj.GetComponent<OperativesControl>();
        SetMinimapActiveScreen();
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
            SetOperativeActiveScreen(FIRST_OPERATIVE);
        }
        else if (!Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha2))
        {
            mainScreenMR.material = screenMaterials[(int)Screens.Operative2];
            SetOperativeActiveScreen(SECOND_OPERATIVE);
        }
        else if (!Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha3))
        {
            mainScreenMR.material = screenMaterials[(int)Screens.Operative3];
            SetOperativeActiveScreen(THIRD_OPERATIVE);
        }
        else if (!Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha4))
        {
            mainScreenMR.material = screenMaterials[(int)Screens.Operative4];
            SetOperativeActiveScreen(FOURTH_OPERATIVE);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            mainScreenMR.material = screenMaterials[(int)Screens.Main];
            SetMinimapActiveScreen();
        }
        if (Input.GetMouseButtonUp(0))
        {
            OnClick();
        }
    }

    public void SetActiveScreen(ActiveScreen _activeScreen)
    {
        activeScreen = _activeScreen;
    }

    public void SetMinimapActiveScreen()
    {
        ActiveScreen screen = new MinimapActiveScreen(topCamera, operativesControl);
        SetActiveScreen(screen);
    }

    public void SetOperativeActiveScreen(int operativeIdx)
    {
        GameObject operativeObj = operativesControl.GetOperative(operativeIdx);
        Transform cameraTransform = operativeObj.transform.Find("Camera");
        Camera camera = cameraTransform.GetComponent<Camera>();
        Unit operative = operativeObj.GetComponent<Unit>();
        ActiveScreen screen = new OperativeActiveScreen(camera, operative);
        SetActiveScreen(screen);
    }

    public void OnClick()
    {
        RaycastHit hitScreen;
        Ray clickRay = viewCamera.ScreenPointToRay(Input.mousePosition);

        // check if we hit the main screen area
        if (Physics.Raycast(clickRay, out hitScreen) && (hitScreen.collider == screenCollider))
        {
            // resolve the point we clicked on the screen 
            Vector3 screenPoint = CameraToScreen(hitScreen);

            // convert point on the screen to world coordinates
            Ray viewRay = activeScreen.ScreenPointToRay(screenPoint);

            ClickReaction reaction = activeScreen.GetReaction(viewRay);
            reaction.React();
        }
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

public abstract class ActiveScreen
{
    protected Camera camera;
    public ActiveScreen(Camera _camera)
    {
        camera = _camera;
    }
    protected RaycastHit hitObject;
    public abstract ClickReaction GetReaction(Ray viewRay);

    public Ray ScreenPointToRay(Vector3 screenPoint)
    {
        // convert point on the screen to world coordinates
        return camera.ScreenPointToRay(screenPoint);
    }
}

public class MinimapActiveScreen: ActiveScreen
{
    OperativesControl operativesControl;
    public MinimapActiveScreen(Camera _camera, OperativesControl _operativesControl)
        : base(_camera)
    {
        operativesControl = _operativesControl;
    }
    public override ClickReaction GetReaction(Ray viewRay)
    {
        Door door;

        // cast ray to see what object we hit
        if (Physics.Raycast(viewRay, out hitObject))
        {
            if ((door = hitObject.collider.GetComponent<Door>()) != null)
            {
                if (door.isPowered)
                {
                    if (door.isAutotomate)
                    {
                        return new AutomaticDoorReaction(hitObject);
                    }
                    else
                    {
                        GameObject operativeObj = operativesControl.GetCurrentOperative();
                        Unit operative = operativeObj.GetComponent<Unit>();
                        return new ManualDoorReaction(hitObject, operative);
                    }
                }
            }
            else
            {
                return new FloorReaction(hitObject, operativesControl);
            }
        }
        return new NoReaction(hitObject);
    }
}

public class OperativeActiveScreen : ActiveScreen
{
    Unit operative;
    public OperativeActiveScreen(Camera _camera, Unit _operative) : base(_camera) 
    {
        operative = _operative;
    }
    public override ClickReaction GetReaction(Ray viewRay)
    {
        Alien alien;
        Door door;

        // cast ray to see what object we hit
        if (Physics.Raycast(viewRay, out hitObject))
        {
            if ((alien = hitObject.collider.GetComponent<Alien>()) != null)
            {
                // TODO: implement shooting
                return new AlienFleeReaction(hitObject);
            }
            else if ((door = hitObject.collider.GetComponent<Door>()) != null)
            {
                if (door.isPowered)
                {
                    return new ManualDoorReaction(hitObject, operative);
                }
            }
        }
        return new NoReaction(hitObject);
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

public class AutomaticDoorReaction: ClickReaction
{
    public AutomaticDoorReaction(RaycastHit hitInfo) : base(hitInfo) { }
    public override void React()
    {
        Door door = hitInfo.collider.GetComponent<Door>();
        door.Switch();
    }
}

public class ManualDoorReaction : ClickReaction
{
    Unit operative;
    public ManualDoorReaction(RaycastHit hitInfo, Unit _operative) : base(hitInfo) 
    {
        operative = _operative;
    }
    public override void React()
    {
        operative.InteractWith(hitInfo.collider.gameObject, hitInfo.point);
    }
}

public class FloorReaction : ClickReaction
{
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

public class AlienFleeReaction : ClickReaction
{
    public AlienFleeReaction(
        RaycastHit hitInfo) : base(hitInfo) { }

    public override void React()
    {
        Alien alien = hitInfo.collider.gameObject.GetComponent<Alien>();
        alien.SwitchState(new EscapeState(alien));
    }
}