using UnityEngine;

public class MouseWorldPositionManager : MonoBehaviour
{
    public static MouseWorldPositionManager mouseWorldPositionManager;
    public void Awake()
    {
        mouseWorldPositionManager = this;
    }
    public Vector3 GetMousePosition()
    {
        Ray mouseCameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new(Vector3.up, Vector3.zero);
        if (plane.Raycast(mouseCameraRay, out float distance))
        {
            return mouseCameraRay.GetPoint(distance);
        }
        else
        {
            return Vector3.zero;
        }
    }
}
