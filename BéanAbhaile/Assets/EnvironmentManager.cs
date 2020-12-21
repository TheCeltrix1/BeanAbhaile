using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using TMPro;

[DefaultExecutionOrder(-20)]
public class EnvironmentManager : MonoBehaviour
{
    public static EnvironmentManager instance;
    public static bool overrideFogSettings;
    public static Color baseFogColor;

    [Header("Basement Atmosphere")]
    public float fogMax = 0.4f;
    public float fogStartY;
    public float fogMaxY;
    public float areaSize;

    [Header("Upstairs")]
    public Transform staircaseUpperPoint;
    public float distanceThreshold;
    public float minSaturation;
    public float maxSaturation;
    public PostProcessVolume volume;
    public LayerMask lineCastMask;
    public Player playerReference;

    private ColorGrading _grading;
    private Grain _grain;

    [Header("UI Effects")]
    public Canvas generalCanvas;
    public TextMeshProUGUI pickupText;

    [Header("Banshee Feedback")]
    public Transform bansheeReference;
    public float feedbackDistanceThreshold;
    public LayerMask feedbackMask;
    public AudioSource feedbackSource;
    [Range(0f, 1f)] public float maxVolume = 1f;
    public float volumeChangeRate = 100f;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    private void OnEnable()
    {
        if (volume)
        {
            volume.profile.TryGetSettings(out _grading);
            volume.profile.TryGetSettings(out _grain);
        }
        baseFogColor = RenderSettings.fogColor;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector3(transform.position.x, fogStartY, transform.position.z), new Vector3(areaSize, 0f, areaSize));
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(new Vector3(transform.position.x, fogMaxY, transform.position.z), new Vector3(areaSize, 0f, areaSize));

        Gizmos.color = Color.green;
        Gizmos.DrawLine(Player.PlayerPosition, staircaseUpperPoint.position);
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(Player.PlayerPosition, bansheeReference.position);
#if UNITY_EDITOR
        UnityEditor.Handles.Label(Player.PlayerPosition + Vector3.up * 1.5f, $"Staircase Dist: {Vector3.Distance(Player.PlayerPosition, staircaseUpperPoint.position)}\n" +
            $"Banshee Dist: {Vector3.Distance(Player.PlayerPosition, bansheeReference.position)}\n" +
            $"{Map(Vector3.Distance(Player.PlayerPosition, bansheeReference.position), 1f, feedbackDistanceThreshold, 0f, maxVolume)}");
#endif
    }


    private void Update()
    {
        UpdateBansheeFeedback();
        if (!overrideFogSettings)
        {
            RenderSettings.fogColor = baseFogColor;
            if (Player.PlayerPosition.y > fogStartY)
                RenderSettings.fog = false;
            else
            {
                RenderSettings.fog = true;
                RenderSettings.fogDensity = Map(Player.PlayerPosition.y, fogStartY, fogMaxY, 0f, fogMax);
            }
        }

        if (Physics.Linecast(Player.PlayerPosition, staircaseUpperPoint.position, out RaycastHit hit, lineCastMask)) { playerReference.StopBlink(); return; }

        float distance = Vector3.Distance(Player.PlayerPosition, staircaseUpperPoint.position);
        if (distance <= distanceThreshold)
        {
            playerReference.Blink();
            float saturation = Map(distance, distanceThreshold, 0.5f, minSaturation, maxSaturation);
            if (_grading)
                _grading.saturation.value = saturation;
            if (_grain)
                _grain.intensity.value = Map(distance, distanceThreshold, 0.5f, 0f, 1f);
        }
        else
            playerReference.StopBlink();
    }

    private float Map(float value, float s1, float e1, float s2, float e2) {
        return s2 + (e2 - s2) * ((value - s1) / (e1 - s1));
    }

    public static void CreateScreenMessage(string text) {
        if (!instance) return;
        TextMeshProUGUI t = Instantiate(instance.pickupText);
        t.transform.SetParent(instance.generalCanvas.transform, false);
        t.text = text;
        Destroy(t.gameObject, 6f);
    }

    private void UpdateBansheeFeedback()
    {
        float dist = Vector3.Distance(playerReference.transform.position, bansheeReference.position);
        if (Physics.Linecast(playerReference.transform.position, bansheeReference.position, out RaycastHit hit, feedbackMask)) {
            if (feedbackSource)
                feedbackSource.volume = Mathf.MoveTowards(feedbackSource.volume, 0f, volumeChangeRate * Time.deltaTime);
            return;
        }
        if (dist <= feedbackDistanceThreshold)
        {
            if (feedbackSource)
                feedbackSource.volume = Mathf.MoveTowards(feedbackSource.volume, Map(dist, feedbackDistanceThreshold + 1f, 0f, 0f, maxVolume), volumeChangeRate * Time.deltaTime);
            else
            {
                if (feedbackSource)
                    feedbackSource.volume = 0f;
            }
        }
    }
}
