using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(FactionCentreTile))]
public class FactionCentreTileEditor : TileEditor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }
}
