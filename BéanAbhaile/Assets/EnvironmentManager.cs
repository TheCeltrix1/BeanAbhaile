using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    public float fogMax = 0.4f;
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
        if (Player.PlayerPosition.y > fogStartY)
        {
            RenderSettings.fog = false;
            return;
        }
        else {
            RenderSettings.fog = true;
            RenderSettings.fogDensity = Map(Player.PlayerPosition.y, fogStartY, fogMaxY, 0f, fogMax);
        }
    }

    private float Map(float value, float s1, float e1, float s2, float e2) {
        return s2 + (e2 - s2) * ((value - s1) / (e1 - s1));
    }
}
