/*
Link: https://gist.github.com/kukumberman/94d82270f5c4ed1fc18ff1236a8fbb8c
Inspired by: https://nevzatarman.com/2014/12/19/unity-editor-scripting-quick-setactive/
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class HierarchyQuickSetActive
{
    private static int size = 16;

    static HierarchyQuickSetActive()
    {
        EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItem;
    }

    private static void HierarchyWindowItem(int instanceID, Rect selectionRect)
    {
        if (EditorUtility.InstanceIDToObject(instanceID) is GameObject gameObject)
        {
            float x = selectionRect.x - size * 1.75f;
            Rect rect = new Rect(x, selectionRect.y, size, size);

            EditorGUI.BeginChangeCheck();

            bool value = GUI.Toggle(rect, gameObject.activeSelf, string.Empty);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(gameObject, $"Active state - {gameObject.name}");
                gameObject.SetActive(value);
            }
        }
    }

    private static void DrawRect(Rect rect, Color color, float alpa = 0.2f, string text = "")
    {
        color.a = alpa;
        GUI.color = color;
        GUI.Box(rect, text);
    }
}