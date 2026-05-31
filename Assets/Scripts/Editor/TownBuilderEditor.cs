using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TownBuilder))]
public class TownBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TownBuilder townBuilder = (TownBuilder)target;

        if (GUILayout.Button("äXÇçÏÇÈ"))
        {
            townBuilder.BuildTown();
        }
        if (GUILayout.Button("äXÇè¡Ç∑"))
        {
            townBuilder.ClearTown();
        }
    }
}