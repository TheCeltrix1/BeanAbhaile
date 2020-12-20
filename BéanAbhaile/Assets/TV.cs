using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TV : Interactable
{
    private GameObject _screen;

    public override void OnEnable()
    {
        base.OnEnable();
        _screen = transform.GetChild(0).gameObject;
    }

    public override void OnUse()
    {
        _screen.SetActive(!_screen.activeInHierarchy);
    }
}
