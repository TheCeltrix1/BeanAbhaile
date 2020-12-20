using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class MeshCombiner : MonoBehaviour
{
    public Material[] sharedMaterials;
    public void CombineMeshes()
    {
        Vector3 pos = transform.position;
        Quaternion rot = transform.rotation;    

        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        MeshFilter f = GetComponent<MeshFilter>();
        if (!f) {
            Debug.LogWarning("There is no MeshFilter component attached to " + name + " gameobject!");
            return;
        }
        MeshFilter[] filters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combination = new CombineInstance[filters.Length];

        for (int i = 0; i < filters.Length; i++) {
            MeshFilter filter = filters[i];
            if (filter.gameObject == gameObject) continue;
            combination[i].mesh = filter.sharedMesh;
            combination[i].transform = transform.worldToLocalMatrix * filter.transform.localToWorldMatrix;
            filter.gameObject.SetActive(false);
        }

        transform.GetComponent<MeshFilter>().sharedMesh = new Mesh();
        transform.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combination);
        Mesh m = transform.GetComponent<MeshFilter>().sharedMesh;

        transform.GetComponent<MeshCollider>().sharedMesh = m;
        if (sharedMaterials == null && sharedMaterials.Length > 0) {
            transform.GetComponent<MeshRenderer>().materials[0] = sharedMaterials[0];
            Debug.Log("Added " + sharedMaterials.Length + "material" + (sharedMaterials.Length != 1 ? "s" : "") + " to " + name + " gameobject.");
        }
        transform.gameObject.SetActive(true);

        transform.position = pos;
        transform.rotation = rot;

        Debug.Log("Combined " + combination.Length + " meshes in " + name + " gameobject and added necessary mesh collider references.");
        DestroyImmediate(this);
    }
}
