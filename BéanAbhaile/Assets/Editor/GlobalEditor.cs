using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MonoBehaviour), editorForChildClasses: true)]
public class GlobalEditor : Editor
{
    public const float kChangeRate = 0.01f;
    public const float kFixedDeltaTime = 1 / 60f;
    public static float hue;

    private void OnEnable()
    {
        EditorApplication.update += delegate {
            hue = hue + kChangeRate * kFixedDeltaTime < 1f ? hue + kChangeRate * kFixedDeltaTime : 0f;
            Repaint();
        };

    }

    private void OnDisable()
    {
        EditorApplication.update -= delegate {
            hue = hue + kChangeRate * kFixedDeltaTime < 1f ? hue + kChangeRate * kFixedDeltaTime : 0f;
            Repaint();
        };
    }

    public override void OnInspectorGUI() => UwuifyInspector(serializedObject);   

    public static void GetAllProperties(SerializedProperty entryPoint, ref List<SerializedPropertyGroup> list)
    {
        list.Clear();
        SerializedProperty current = entryPoint.Copy();
        while (current.Next(true))
        {
            if (!current.name.StartsWith("m_") || current.name.StartsWith("m_Script"))
                list.Add(new SerializedPropertyGroup()
                {
                    property = current,
                    name = current.name
                });
        }
    }

    private static string Uwuify(string s) {
        return s = System.Text.RegularExpressions.Regex.Replace(s, "[rl]", "w");
    }

    public static void UwuifyInspector(SerializedObject serializedObject) {
        List<SerializedPropertyGroup> group = new List<SerializedPropertyGroup>();


        Color c = Color.HSVToRGB(hue, 1f, 1f);
        EditorStyles.boldLabel.normal.textColor = c;
        EditorStyles.centeredGreyMiniLabel.normal.textColor = c;
        EditorStyles.label.normal.textColor = c;
        EditorStyles.label.fontSize = 12;
        EditorStyles.largeLabel.normal.textColor = c;
        EditorStyles.linkLabel.normal.textColor = c;
        EditorStyles.miniBoldLabel.normal.textColor = c;
        EditorStyles.miniLabel.normal.textColor = c;
        //EditorStyles.structHeadingLabel.normal.textColor = c;
        EditorStyles.whiteBoldLabel.normal.textColor = c;
        EditorStyles.wordWrappedLabel.normal.textColor = c;
        EditorStyles.wordWrappedMiniLabel.normal.textColor = c;
        EditorStyles.toolbar.normal.textColor = c;
        EditorStyles.boldLabel.normal.textColor = c;
        EditorStyles.textArea.normal.textColor = c;
        EditorStyles.textField.normal.textColor = c;
        EditorStyles.toolbarTextField.normal.textColor = c;
        EditorStyles.miniTextField.normal.textColor = c;
        EditorStyles.objectField.normal.textColor = c;
        EditorStyles.toolbarDropDown.normal.textColor = c;
        EditorStyles.toolbar.normal.textColor = c;
        EditorStyles.toggle.normal.textColor = c;
        EditorStyles.toggleGroup.normal.textColor = c;
        EditorStyles.miniButton.normal.textColor = c;
        EditorStyles.miniButtonLeft.normal.textColor = c;
        EditorStyles.miniButtonRight.normal.textColor = c;
        EditorStyles.miniButtonMid.normal.textColor = c;
        EditorStyles.foldoutHeader.normal.textColor = c;
        EditorStyles.foldout.normal.textColor = c;
        EditorStyles.foldoutPreDrop.normal.textColor = c;

        GetAllProperties(serializedObject.GetIterator(), ref group);
        foreach (SerializedPropertyGroup g in group)
        {
            SerializedProperty p = serializedObject.FindProperty(g.name);
            if (p != null)
                EditorGUILayout.PropertyField(p, new GUIContent(Uwuify(p.displayName)));
        }
        //GUI.color = c;
        serializedObject.ApplyModifiedProperties();
    }
}

[System.Serializable]
public class SerializedPropertyGroup
{
    public SerializedProperty property;
    public string name;
}