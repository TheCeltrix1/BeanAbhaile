using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact : MonoBehaviour
{
    private Transform _camera;
    private RaycastHit _hit;
    public GameObject banshee;
    private GameObject brush;
    private bool _pickedUp = false;

    private void OnEnable()
    {
        _camera = Camera.main.transform;
    }

    private void Update()
    {
        Physics.Raycast(_camera.position, _camera.forward, out _hit);
        if (!_hit.transform) return;
        if (_hit.transform.gameObject.name == "Noose")
        {
            if (Input.GetMouseButtonDown(0))
            {
                Destroy(_hit.transform.gameObject);
                banshee.GetComponent<Banshee>().noose = true;
            }
        }
        else if (_hit.transform.gameObject.name == "Brush")
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!_pickedUp)
                {
                    _pickedUp = true;
                    brush = _hit.transform.gameObject;
                    brush.SetActive(false);
                }
            }
        }
        else if (_hit.transform.gameObject.name == "BrushLocation")
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (_pickedUp)
                {
                    _pickedUp = false;
                    GameObject gab = _hit.transform.gameObject;
                    brush.transform.position = gab.transform.position;
                    brush.transform.rotation = gab.transform.rotation;
                    banshee.GetComponent<Banshee>().brush = true;
                    brush.SetActive(true);
                }
            }
        }
    }
}
