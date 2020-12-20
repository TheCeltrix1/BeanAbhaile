using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MeshAssetGeneratorEditor : Editor
{
    [MenuItem("CONTEXT/MeshFilter/Create Mesh Asset")]
    public static void CreateMeshAsset(MenuCommand command) {
        MeshFilter mf = command.context as MeshFilter;
        Mesh m = mf.sharedMesh;
        MeshUtility.Optimize(m);

        string path = EditorUtility.SaveFilePanel("Create New Mesh Asset...", "Assets/Meshes", mf.gameObject.name, "asset");
        if (string.IsNullOrEmpty(path)) return;
        path = FileUtil.GetProjectRelativePath(path);

        AssetDatabase.CreateAsset(m, path);
        AssetDatabase.SaveAssets();
        Debug.Log("Saved mesh " + mf.gameObject.name + " to " + path);
    }
}
