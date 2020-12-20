using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sink : Interactable
{
    public float flowRate;
    public GameObject water;
    public Color underwaterFogColor;
    [ColorUsage(true, true)] public Color underwaterLightColor;
    public float floodSpeed;
    public float drainSpeed;
    public float maxY;
    public float density;
    public bool memeMode;
    public float areaSize = 1f;

    public Material waterMat;

    private float _waterFlowOffset;
    private Animator _anim;
    private bool _flowing;
    private Vector3 _startPosition;
    private Color _oldAmbientLight;

    public override void OnEnable()
    {
        base.OnEnable();
        _anim = GetComponentInChildren<Animator>();
        if(water)
            _startPosition = water.transform.localPosition;
        _oldAmbientLight = RenderSettings.ambientLight;
    }

    public override void OnUse()
    {
        _flowing = !_flowing;
        if (_anim)
            _anim.SetBool("Flowing", _flowing);
    }

    private void Update()
    {
        if(memeMode)
            EnvironmentManager.overrideFogSettings = _flowing;

        if (waterMat)
            waterMat.mainTextureOffset = new Vector2(waterMat.mainTextureOffset.x, _waterFlowOffset = _waterFlowOffset < 1f ? _waterFlowOffset + (flowRate * Time.deltaTime) : 0f);

        if (water) {
            if (_flowing)
            {
                water.transform.localPosition = Vector3.MoveTowards(water.transform.localPosition, new Vector3(water.transform.localPosition.x, maxY / 100f, water.transform.localPosition.z), floodSpeed * Time.deltaTime);
            }
            else
                water.transform.localPosition = Vector3.MoveTowards(water.transform.localPosition, new Vector3(water.transform.localPosition.x, _startPosition.y, water.transform.localPosition.z), drainSpeed * Time.deltaTime);
            if (!memeMode) return;

            RenderSettings.fog = water.transform.position.y > Player.PlayerPosition.y;
            if (RenderSettings.fog)
            {
                RenderSettings.fogColor = underwaterFogColor;
                RenderSettings.fogDensity = density;
                RenderSettings.ambientLight = underwaterLightColor;
            }
            else
                RenderSettings.ambientLight = _oldAmbientLight;
        }
    }

    private void OnDrawGizmos()
    {
        if (!water) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(water.transform.parent.TransformPoint(water.transform.localPosition.x, maxY / 100f, water.transform.localPosition.z), new Vector3(areaSize, 0f, areaSize));
    }
}
