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

        serializedObject.ApplyModifiedProperties();
    }

    private void CreateHorizontalToggle(SerializedProperty prop, int direction)
    {
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.PropertyField(prop);

        if (prop.objectReferenceValue == null)
        {
            if (GUILayout.Button("Add Pipe?"))
            {
                CreatePipe(prop, direction);
            }
        }

        EditorGUILayout.EndHorizontal();
    }

    private void CreatePipe(SerializedProperty originalProp, int direction)
    {
        // Get the current selected pipe
        PipePiece pipe = (PipePiece) target;

        // Find the location of the next pipe
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

        // Determine if a pipe exists at this position;
        PipePiece nextPipe = null;
        foreach (PipePiece p in FindObjectsOfType<PipePiece>())
        {
            if (p == pipe)
                continue;

            Vector3 displacement = p.transform.position - nextPos;
            if (displacement.sqrMagnitude < 0.01f) // close enough to be the at the new location
            {
                nextPipe = p;
                break;
            }
        }

        // If there is no pipe at the next location, create it
        if (nextPipe == null)
        {
            PipePiece prefab = Resources.Load<PipePiece>("Prefabs/pipe");
            nextPipe = Instantiate(prefab, nextPos, Quaternion.identity);
        }

        // Join the nextPipe with this pipe
        if (direction == 0)
        {
            pipe.pipeN = nextPipe;
            nextPipe.pipeS = pipe;
        }
        else if (direction == 1)
        {
            pipe.pipeE = nextPipe;
            nextPipe.pipeW = pipe;
        }
        else if (direction == 2)
        {
            pipe.pipeS = nextPipe;
            nextPipe.pipeN = pipe;
        }
        else if (direction == 3)
        {
            pipe.pipeW = nextPipe;
            nextPipe.pipeE = pipe;
        }

        // Set the next pipe to be the selected game object
        Selection.activeGameObject = nextPipe.gameObject;
    }
}
