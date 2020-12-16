using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Key : Interactable
{
    public bool PickedUp { get; private set; }
    private TextMeshProUGUI _text;

    public override void OnEnable()
    {
        base.OnEnable();
        _text = GetComponentInChildren<TextMeshProUGUI>();
    }

    public override void OnInteractableSelected()
    {
        if (PickedUp) return;
        if (_text)
            _text.text = $"Press <color=yellow>{Player.instance.interactKey.ToString()}</color> for <color=yellow>{interactableName}</color>";
    }

    public override void OnInteractableDeselected()
    {
        if (PickedUp) return;
        if (_text)
            _text.text = "";
    }

    public override void OnUse()
    {
        if (PickedUp) return;
        PickedUp = true;
        if (_text)
            _text.text = "";
        if (meshRenderer)
            meshRenderer.enabled = false;
        EnvironmentManager.CreateScreenMessage($"{interactableName} acquired.");
    }
}
