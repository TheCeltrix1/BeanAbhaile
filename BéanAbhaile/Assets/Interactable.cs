using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [Header("Interactable Settings")]
    [ColorUsage(true, true)] public Color selectedColor;
    protected MeshRenderer meshRenderer;
    protected Color initialColor;
    public bool selectionColorChangeEnabled = true;
    public string interactableName;

    public virtual void OnEnable()
    {
        if(!(meshRenderer = GetComponent<MeshRenderer>()))
            meshRenderer = GetComponentInChildren<MeshRenderer>();

        if (meshRenderer)
            initialColor = meshRenderer.material.color;
    }

    private void OnInteractableSelectedInternal()
    {
        OnInteractableSelected();
        if (!selectionColorChangeEnabled) return;
        if (meshRenderer)
            meshRenderer.material.color = selectedColor;
    }

    private void OnInteractableDeselectedInternal()
    {
        OnInteractableDeselected();
        if (!selectionColorChangeEnabled) return;
        if (meshRenderer)
            meshRenderer.material.color = initialColor;
    }

    public virtual void OnInteractableSelected() { }
    public virtual void OnInteractableDeselected() { }
    public abstract void OnUse();
}

public enum Direction {
    Forward,
    Backwards,
    Left,
    Right,
    Up,
    Down
}