using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Board))]
[CanEditMultipleObjects]
public class BoardEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        DrawDefaultInspector();
        if (GUILayout.Button("Do Something, Anything!"))
        {
            //
            Board board = (Board)target;
            //board.InitializeValidBoard();
            Debug.Log("Board block count: " + board.BlockList.Count);
        }
    }
}
