using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : Interactable
{
    public bool PickedUp { get; private set; }

    public override void OnUse()
    {
        if (PickedUp) return;
        PickedUp = true;
        if(meshRenderer)
            meshRenderer.enabled = false;
    }
}
