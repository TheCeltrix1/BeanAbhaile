using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    public float fogMax;
    public float fogStartY;
    public float fogMaxY;
    public float areaSize;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector3(transform.position.x, fogStartY, transform.position.z), new Vector3(areaSize, 0f, areaSize));
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(new Vector3(transform.position.x, fogMaxY, transform.position.z), new Vector3(areaSize, 0f, areaSize));
    }

    private void Update()
    {
        
    }
}
