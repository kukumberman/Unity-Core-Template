using UnityEngine;
#if UNITY_EDITOR
using System;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEditor;
#endif

public sealed class SceneDropdownAttribute : PropertyAttribute { }

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SceneDropdownAttribute))]
public class SceneDropdownAttributeDrawer : PropertyDrawer
{
    private static readonly string MessageUnsupportedType =
        "Unsupported property type (use String or Integer)";

    private readonly string[] _sceneNames;

    public SceneDropdownAttributeDrawer()
    {
        int count = SceneManager.sceneCountInBuildSettings;
        _sceneNames = new string[count];

        for (int i = 0; i < _sceneNames.Length; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = Path.GetFileNameWithoutExtension(scenePath);
            _sceneNames[i] = sceneName;
        }
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        label = EditorGUI.BeginProperty(position, label, property);
        var contentRect = EditorGUI.PrefixLabel(position, label);

        if (property.propertyType == SerializedPropertyType.String)
        {
            DrawStringTypeProperty(property, contentRect);
        }
        else if (property.propertyType == SerializedPropertyType.Integer)
        {
            DrawIntTypeProperty(property, contentRect);
        }
        else
        {
            EditorGUI.LabelField(position, ""); // draw default label
            EditorGUI.HelpBox(contentRect, MessageUnsupportedType, MessageType.Error);
        }

        EditorGUI.EndProperty();
    }

    private void DrawStringTypeProperty(SerializedProperty property, Rect contentRect)
    {
        int index = Array.IndexOf(_sceneNames, property.stringValue);
        if (index == -1)
        {
            index = 0;
        }
        int selectedIndex = EditorGUI.Popup(contentRect, index, _sceneNames);
        property.stringValue = _sceneNames[selectedIndex];
    }

    private void DrawIntTypeProperty(SerializedProperty property, Rect contentRect)
    {
        int index = EditorGUI.Popup(contentRect, property.intValue, _sceneNames);
        if (index < 0 || index >= _sceneNames.Length)
        {
            index = 0;
        }
        property.intValue = index;
    }
}
#endif
