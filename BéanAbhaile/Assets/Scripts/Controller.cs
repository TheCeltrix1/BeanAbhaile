using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public float noise;
    private Vector2 _moveDir;
    private float _moveSpeed = 2;
    private Rigidbody _rb;
    private Transform _camera;
    private float _maxCameraAngle = 15;
    private float _cameraSensitivity = 100f;

    private float xRotation = 0f;

    // Start is called before the first frame update
    void Start()
    {
        _camera = this.transform.GetChild(0);
        if (!GetComponent<Rigidbody>()) this.gameObject.AddComponent<Rigidbody>();
        _rb = GetComponent<Rigidbody>();
        _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;
        //_rb.isKinematic = true;
        _rb.useGravity = true;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        AdjustCamera();
    }

    private void Move()
    {
        _moveDir.x = Input.GetAxis("Horizontal");
        _moveDir.y = Input.GetAxis("Vertical");
        _moveDir *= _moveSpeed * Time.deltaTime;

        _rb.velocity = new Vector3(_moveDir.x,0,_moveDir.y);
    }

    private void AdjustCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * _cameraSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * _cameraSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //this.transform.rotation = Quaternion.Euler(0, this.transform.rotation.eulerAngles.y + mouseX, 0);
        this.transform.Rotate(Vector3.up * mouseX);
        
        _camera.transform.rotation = Quaternion.Euler(xRotation, _camera.transform.rotation.eulerAngles.y, 0);
    }
}
