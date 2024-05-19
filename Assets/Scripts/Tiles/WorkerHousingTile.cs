using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorkerHousingTile : Tile
{
    [SerializeField] private bool IsEnemy;

    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
        if (!Application.isPlaying)
            return false;

        if (IsEnemy)
            EnemyWorkers.IncreaseWorkerLimit(2);
        else
            Workers.IncreaseWorkerLimit(2);

        return base.StartUp(position, tilemap, go);
    }

#if UNITY_EDITOR
    // The following is a helper that adds a menu item to create a TrainingCenterTile Asset
    [MenuItem("Assets/Create/WorkerHousingTile")]
    public static void CreateTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Worker Housing Tile", "New Worker Housing Tile", "asset", "Save Worker Housing Tile", "Assets");
        if (path == "")
            return;
        AssetDatabase.CreateAsset(CreateInstance<WorkerHousingTile>(), path);
    }
#endif
}
