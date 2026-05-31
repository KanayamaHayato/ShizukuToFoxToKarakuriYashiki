using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PathBuilder))]
public class PathBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PathBuilder pathBuilder = (PathBuilder)target;

        if (GUILayout.Button("Q“¹‚ğì‚é"))
            pathBuilder.BuildPath();

        if (GUILayout.Button("Q“¹‚ğÁ‚·"))
            pathBuilder.ClearPath();
    }
}