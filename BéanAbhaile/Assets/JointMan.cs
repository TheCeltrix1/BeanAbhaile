using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class JointMan : MonoBehaviour
{
    [Header("Gizmos")]
    public Color jointColor = Color.red;
    public float jointSize = 0.5f;

    [Header("Settings")]
    public string jointNamingConvention = "joint_";

    private Transform[] _joints;

    [ExecuteInEditMode]
    private void OnEnable() => _joints = GetComponentsInChildren<Transform>();

    private void OnDrawGizmos()
    {
        if (_joints == null) return;
        Color old = Gizmos.color;
        Gizmos.color = jointColor;
        foreach (Transform t in _joints)
        {
            if(t.name.StartsWith(jointNamingConvention))
                Gizmos.DrawSphere(t.position, jointSize);
        }

        Gizmos.color = old;
    }
}
