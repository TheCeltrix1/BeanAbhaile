using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MonoBehaviour), editorForChildClasses: true)]
public class GlobalEditor : Editor
{
    public override void OnInspectorGUI() => UwuifyInspector(serializedObject);   

    public static void GetAllProperties(SerializedProperty entryPoint, ref List<SerializedPropertyGroup> list)
    {
        list.Clear();
        SerializedProperty current = entryPoint.Copy();
        while (current.Next(true))
        {
            if (!current.name.StartsWith("m_"))
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
        GetAllProperties(serializedObject.GetIterator(), ref group);
        foreach (SerializedPropertyGroup g in group)
        {
            SerializedProperty p = serializedObject.FindProperty(g.name);
            if (p != null)
                EditorGUILayout.PropertyField(p, new GUIContent(Uwuify(p.displayName)));
        }
        serializedObject.ApplyModifiedProperties();
    }
}

[System.Serializable]
public class SerializedPropertyGroup
{
    public SerializedProperty property;
    public string name;
}