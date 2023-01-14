using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ShapeData),false)]
[CanEditMultipleObjects]
[System.Serializable]
public class ShapeDataDrawer : Editor
{
    private ShapeData shapeDatainstance => target as ShapeData;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        ClearBoardButton();
        EditorGUILayout.Space();

        DrawColumnInputField();
        EditorGUILayout.Space();
        if (shapeDatainstance.board != null && shapeDatainstance.columns > 0 && shapeDatainstance.rows > 0)
        {
            DrawBoardTable();
        }

        serializedObject.ApplyModifiedProperties();

        if(GUI.changed)
        {
            EditorUtility.SetDirty(shapeDatainstance);
        }
    }

    private void ClearBoardButton()
    {
        if (GUILayout.Button("Clear Board"))
        {
            shapeDatainstance.Clear();
        } 
    }

    private void DrawColumnInputField()
    {
        var columnsTemp = shapeDatainstance.columns;
        var rowsTemp = shapeDatainstance.rows;
        shapeDatainstance.columns = EditorGUILayout.IntField("Columns", shapeDatainstance.columns);
        shapeDatainstance.rows = EditorGUILayout.IntField("Rows", shapeDatainstance.rows);
        if ((shapeDatainstance.columns != columnsTemp || shapeDatainstance.rows != rowsTemp) && 
            shapeDatainstance.columns > 0 &&
            shapeDatainstance.rows > 0)
        { shapeDatainstance.CreateNewBoard(); }
    }

    private void DrawBoardTable()
    {
        var tableStyle = new GUIStyle("box");
        tableStyle.padding = new RectOffset(10, 10, 10, 10);
        tableStyle.margin.left = 32;

        var headColumnStyle = new GUIStyle();
        headColumnStyle.fixedWidth = 65;
        headColumnStyle.alignment = TextAnchor.MiddleCenter;

        var rowStyle = new GUIStyle();
        rowStyle.fixedHeight = 35;
        rowStyle.alignment = TextAnchor.MiddleCenter;

        var dataFieldStyle = new GUIStyle(EditorStyles.miniButtonMid);
        dataFieldStyle.normal.background = Texture2D.grayTexture;
        dataFieldStyle.onNormal.background = Texture2D.whiteTexture;

        for(var row=0; row<shapeDatainstance.rows; row++)
        {
            EditorGUILayout.BeginHorizontal(headColumnStyle);
            for(var column=0; column<shapeDatainstance.columns; column++)
            {
                EditorGUILayout.BeginHorizontal(rowStyle);
                var data = EditorGUILayout.Toggle(shapeDatainstance.board[row].column[column], dataFieldStyle);
                shapeDatainstance.board[row].column[column] = data;
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndHorizontal();
        }

    }
}
