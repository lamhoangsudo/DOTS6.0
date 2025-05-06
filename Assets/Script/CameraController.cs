using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float moveSpeed, rotationSpeed, zoomSpeed;
    [SerializeField] private CinemachineCamera cinemachineCamera;
    private void Update()
    {
        if(Input.GetKey(KeyCode.W))
        {
            transform.position = Vector3.Lerp(transform.position, transform.position + transform.forward * moveSpeed, Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position = Vector3.Lerp(transform.position, transform.position - transform.forward * moveSpeed, Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position = Vector3.Lerp(transform.position, transform.position - transform.right * moveSpeed, Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position = Vector3.Lerp(transform.position, transform.position + transform.right * moveSpeed, Time.deltaTime);
        }
        if(Input.GetKey(KeyCode.Q))
        {
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, transform.eulerAngles - new Vector3(0, rotationSpeed, 0), Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, transform.eulerAngles + new Vector3(0, rotationSpeed, 0), Time.deltaTime);
        }
        if(Input.mouseScrollDelta.y > 0)
        {
            cinemachineCamera.Lens.FieldOfView = Mathf.Lerp(cinemachineCamera.Lens.FieldOfView, cinemachineCamera.Lens.FieldOfView - zoomSpeed, Time.deltaTime);
            cinemachineCamera.Lens.FieldOfView = Mathf.Clamp(cinemachineCamera.Lens.FieldOfView, 20, 60);
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            cinemachineCamera.Lens.FieldOfView = Mathf.Lerp(cinemachineCamera.Lens.FieldOfView, cinemachineCamera.Lens.FieldOfView + zoomSpeed, Time.deltaTime);
            cinemachineCamera.Lens.FieldOfView = Mathf.Clamp(cinemachineCamera.Lens.FieldOfView, 20, 60);
        }
    }
}
