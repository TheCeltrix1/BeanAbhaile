using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Player))]
public class PlayerInspector : Editor
{
    private void OnEnable()
    {

    }

    public override void OnInspectorGUI()
    {
        GlobalEditor.UwuifyInspector(serializedObject);
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 48;
        style.fontStyle = FontStyle.Bold;
        style.richText = true;
        style.alignment = TextAnchor.MiddleCenter;
        style.normal.textColor = Color.HSVToRGB(GlobalEditor.hue, 1f, 1f);
        GUILayout.Label($"UwU", style);
    }
}
