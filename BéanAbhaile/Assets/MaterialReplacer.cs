using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MaterialReplacer : MonoBehaviour
{
    public Material material;

    public void Replace() {
        if (!material) return;
        foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>()) {
            mr.sharedMaterial = material;
        }
    }

    public void AddMeshColliders()
    {
        foreach (MeshFilter mf in GetComponentsInChildren<MeshFilter>())
        {
            if (!mf) continue;
            Mesh m = mf.sharedMesh;
            MeshCollider mc = mf.gameObject.GetComponent<MeshCollider>();
            if (!mc)
                mc = mf.gameObject.AddComponent<MeshCollider>();
            mc.sharedMesh = m;
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MaterialReplacer))]
public class MaterialReplacerInspector : Editor {
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        MaterialReplacer mr = (MaterialReplacer)target;
        if (!mr) return;
        if (GUILayout.Button("Generate Colliders"))
            mr.AddMeshColliders();

        if (!mr.material) return;
        if (GUILayout.Button("Replace")) {
            mr.Replace();
        }
    }
}
#endif