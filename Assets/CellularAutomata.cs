using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public enum NeighborhoodType
{
    Moore,
    vonNeumann
}

[Serializable]
public struct StateColor
{
    public int id;
    public Color color;
}

[Serializable]
public struct CellRule
{
    public int startingState;
    public int endingState;
    public int minSum;
    public int maxSum;
    public int sumState;
    public NeighborhoodType neighborhoodType;
}

[CustomPropertyDrawer(typeof(CellRule))]
public class CellRule_PropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        float rowHeight = 20f;

        EditorGUIUtility.labelWidth = 40;
        DrawCellularAutomaton cellularAutomaton = (DrawCellularAutomaton)property.serializedObject.targetObject;

        EditorGUI.PropertyField(new Rect(position.x, position.y, position.width / 2 - 35, rowHeight), property.FindPropertyRelative("startingState"), new GUIContent("From"));
        EditorGUI.DrawRect(new Rect(position.x + position.width / 2 - 30, position.y, rowHeight, rowHeight), cellularAutomaton.getStateColor(property.FindPropertyRelative("startingState").intValue));
        EditorGUI.PropertyField(new Rect(position.x + position.width / 2, position.y, position.width / 2 - 35, rowHeight), property.FindPropertyRelative("endingState"), new GUIContent("To"));
        EditorGUI.DrawRect(new Rect(position.x + position.width - 30, position.y, 20, rowHeight), cellularAutomaton.getStateColor(property.FindPropertyRelative("endingState").intValue));

        EditorGUIUtility.labelWidth = 20;
        EditorGUI.PropertyField(new Rect(position.x, position.y + rowHeight + 5, 65, rowHeight), property.FindPropertyRelative("sumState"), new GUIContent("If"));
        EditorGUI.DrawRect(new Rect(position.x + 70, position.y + rowHeight + 5, rowHeight, rowHeight), cellularAutomaton.getStateColor(property.FindPropertyRelative("sumState").intValue));

        EditorGUIUtility.labelWidth = 60;
        EditorGUI.PropertyField(new Rect(position.x + 90 + rowHeight, position.y + rowHeight + 5, 85, rowHeight), property.FindPropertyRelative("minSum"), new GUIContent("appears"));
        EditorGUIUtility.labelWidth = 10;
        EditorGUI.PropertyField(new Rect(position.x + 180 + rowHeight, position.y + rowHeight + 5, 34, rowHeight), property.FindPropertyRelative("maxSum"), new GUIContent("-"));
        EditorGUI.LabelField(new Rect(position.x + 220 + rowHeight, position.y + rowHeight + 5, 60, rowHeight), "times");

        EditorGUIUtility.labelWidth = 20;
        EditorGUI.PropertyField(new Rect(position.x, position.y + rowHeight * 2 + 10, position.width / 2, rowHeight), property.FindPropertyRelative("neighborhoodType"), new GUIContent("In"));
        EditorGUI.LabelField(new Rect(position.x + position.width / 2 + 5, position.y + rowHeight * 2 + 10, position.width / 2, rowHeight), "neighborhood");
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label) + 50;
    }
}
