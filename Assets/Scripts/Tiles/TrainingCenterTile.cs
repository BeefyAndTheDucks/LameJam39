using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TrainingCenterTile : Tile
{
    [SerializeField] private float timeToSpawnWorker = 5;
    [SerializeField] private bool IsEnemy;

    private float timer;

    private Vector3Int gridPosition;

    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
        if (TileManager.HasInstance)
            RegisterOnUpdate();
        else
            TileManager.OnReady += RegisterOnUpdate;

        gridPosition = position;

        return base.StartUp(position, tilemap, go);
    }

    private void RegisterOnUpdate()
    {
        TileManager.Instance.OnUpdate += Update;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= timeToSpawnWorker)
        {
            Vector3 position = TileManager.Instance.CellToWorld(gridPosition);
            if (IsEnemy)
                EnemyWorkers.CreateWorker(position);
            else
                Workers.CreateWorker(position);
            timer = 0;
        }
    }

#if UNITY_EDITOR
    // The following is a helper that adds a menu item to create a TrainingCenterTile Asset
    [MenuItem("Assets/Create/TrainingCenterTile")]
    public static void CreateTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Training Center Tile", "New Training Center Tile", "asset", "Save Training Center Tile", "Assets");
        if (path == "")
            return;
        AssetDatabase.CreateAsset(CreateInstance<TrainingCenterTile>(), path);
    }
#endif
}