﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(FighterInfo))]
public class FighterInfoEditor : PropertyDrawer
{
    private float height = 20f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PropertyField(position, property, label, true);
        FighterInfo info = fieldInfo.GetValue(property.serializedObject.targetObject) as FighterInfo;
        if (property.isExpanded)
        {
            if (GUI.Button(new Rect(position.xMin + 30f, position.yMax - 20f, position.width - 30f, 20f), "Load From Text Asset"))
            {
                info.LoadFromTextAsset();
            }
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.isExpanded)
            return EditorGUI.GetPropertyHeight(property) + height;
        return EditorGUI.GetPropertyHeight(property);
    }
}
