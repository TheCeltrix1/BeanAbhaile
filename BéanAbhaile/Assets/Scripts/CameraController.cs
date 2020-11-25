using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Limitations")]
    public float minYRotation = -85f;
    public float maxYRotation = 85f;

    [Header("Sensitivity")]
    public float mouseSensitivity = 1.0f;

    [Header("Input")]
    public string horizontalAxis = "Mouse X";
    public string verticalAxis = "Mouse Y";
    public bool smoothInput = false;
    public bool reverseX;
    public bool reverseY;

    private Vector3 _cameraRotation;

    private void OnEnable() => SetCursorState(false);

    void Update()
    {
        float mouseMovementX = (smoothInput ? Input.GetAxis(horizontalAxis) : Input.GetAxisRaw(horizontalAxis)) * mouseSensitivity * (reverseX ? -1f : 1f);
        float mouseMovementY = -(smoothInput ? Input.GetAxis(verticalAxis) : Input.GetAxisRaw(verticalAxis)) * mouseSensitivity * (reverseY ? -1f : 1f);
        _cameraRotation = new Vector3(Mathf.Clamp(_cameraRotation.x + mouseMovementY, minYRotation, maxYRotation), _cameraRotation.y + mouseMovementX, _cameraRotation.z);
        transform.eulerAngles = _cameraRotation;
    }

    public static void SetCursorState(bool state)
    {
        Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = state;
    }
}