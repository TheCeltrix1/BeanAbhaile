using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    private float _mouseSensitivity = 100;
    public Transform playerBody;
    private float xRotation = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * _mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * _mouseSensitivity;

        xRotation -= mouseY;

        transform.localRotation = Quaternion.Euler(xRotation,0,0);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
