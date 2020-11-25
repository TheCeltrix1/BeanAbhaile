using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Player))]
public class PlayerInspector : Editor
{
    public const float kChangeRate = 0.1f;
    public const float kFixedDeltaTime = 1 / 60f;
    private float _h;

    private void OnEnable()
    {
        EditorApplication.update += delegate {
            _h = _h + kChangeRate * kFixedDeltaTime < 1f ? _h + kChangeRate * kFixedDeltaTime : 0f;
            Repaint();
        };
    }

    public override void OnInspectorGUI()
    {
        GlobalEditor.UwuifyInspector(serializedObject);
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 48;
        style.fontStyle = FontStyle.Bold;
        style.richText = true;
        style.alignment = TextAnchor.MiddleCenter;
        style.normal.textColor = Color.HSVToRGB(_h, 1f, 1f);
        GUILayout.Label($"UwU", style);

    }
}
