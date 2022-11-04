using UnityEngine;
using Mirror;

public class CameraFP : NetworkBehaviour
{
    [SerializeField] private float mouseSensitivity = 3f;
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private Camera cam;

    private float mouseX;
    private float mouseY;

    public static float xRotation;
    public static float yRotation;

    private void Start()
    {
        if (!isLocalPlayer)
        {
            cam.GetComponent<AudioListener>().enabled = false;
            cam.GetComponent<Camera>().enabled = false;
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (!isLocalPlayer) return;

        InputControls();
        cameraHolder.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        orientation.transform.localRotation = Quaternion.Euler(0, yRotation, 0f);
    }

    private void InputControls()
    {
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");

        yRotation += mouseX * mouseSensitivity;
        xRotation -= mouseY * mouseSensitivity;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
    }
}
