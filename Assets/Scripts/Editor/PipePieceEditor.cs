using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

[CustomEditor(typeof(PipePiece))]
[CanEditMultipleObjects]
public class PipePieceEditor : Editor
{
    private SerializedProperty _isPadProperty;

    private SerializedProperty _pipeNProperty;
    private SerializedProperty _pipeEProperty;
    private SerializedProperty _pipeSProperty;
    private SerializedProperty _pipeWProperty;

    void OnEnable()
    {
        _isPadProperty= serializedObject.FindProperty("isPad");

        _pipeNProperty = serializedObject.FindProperty("pipeN");
        _pipeEProperty = serializedObject.FindProperty("pipeE");
        _pipeSProperty = serializedObject.FindProperty("pipeS");
        _pipeWProperty = serializedObject.FindProperty("pipeW");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        CreateHorizontalToggle(_pipeNProperty, 0);
        CreateHorizontalToggle(_pipeEProperty, 1);
        CreateHorizontalToggle(_pipeSProperty, 2);
        CreateHorizontalToggle(_pipeWProperty, 3);

        EditorGUILayout.PropertyField(_isPadProperty);
        if (serializedObject.hasModifiedProperties)
            EditorUtility.SetDirty(target);

        serializedObject.ApplyModifiedProperties();
    }

    private void CreateHorizontalToggle(SerializedProperty prop, int direction)
    {
        const float buttonWidth = 90;
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.PropertyField(prop, GUILayout.Width(170f));

        if (prop.objectReferenceValue == null)
        {
            if (GUILayout.Button("Add Pipe", GUILayout.Width(buttonWidth)))
            {
                CreatePipe(direction);
            }
        }
        else
        {
            if (GUILayout.Button("Remove", GUILayout.Width(buttonWidth)))
            {
                RemoveConnection(direction);
            }
        }

        EditorGUILayout.EndHorizontal();
    }

    private void RemoveConnection(int direction)
    {
        PipePiece pipe = (PipePiece) target;
        PipePiece connection = pipe.RemoveConnection(direction);

        pipe.UpdateRender();
        connection?.UpdateRender();

        EditorUtility.SetDirty(pipe);
        EditorUtility.SetDirty(connection);
    }

    private void CreatePipe(int direction)
    {
        // Get the current selected pipe
        PipePiece pipe = (PipePiece) target;
        PipePiece connection = pipe.AddConnection(direction);

        // Check to see if the connection succeeded
        if (connection == null)
        {
            // If it failed, Create a new pipe at the location, and try connecting again
            // calculate the location of the new pipe
            Vector3 pos = pipe.transform.position;
            Vector3 nextPos = pos;

            if (direction == 0)
                nextPos += Vector3.up;
            else if (direction == 1)
                nextPos += Vector3.right;
            else if (direction == 2)
                nextPos += Vector3.down;
            else if (direction == 3)
                nextPos += Vector3.left;
            else
            {
                Debug.LogError("Invalid Direction");
                return;
            }

            PipePiece prefab = Resources.Load<PipePiece>("Prefabs/pipe");
            connection = PrefabUtility.InstantiatePrefab(prefab, pipe.transform.parent) as PipePiece;
            if (connection == null)
                return;

            connection.transform.position = nextPos;

            // Hypothetically should not fail now
            connection = pipe.AddConnection(direction);
        }

        // Set the next pipe to be the selected game object
        if(connection != null)
            Selection.activeGameObject = connection.gameObject;

        pipe.UpdateRender();
        connection?.UpdateRender();

        EditorUtility.SetDirty(pipe);
        EditorUtility.SetDirty(connection);
    }

    [MenuItem("Tools/Fix Pipes")]
    public static void FixPipes()
    {
        PipePiece.CalculateNeighborsOfAllPipes();
        foreach (PipePiece p in FindObjectsOfType<PipePiece>())
        {
            p.UpdateRender();
            EditorUtility.SetDirty(p);
        }
    }
}
