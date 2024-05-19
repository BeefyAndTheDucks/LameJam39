using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FactionCentreTile : Tile
{
    public static FactionCentreTile Instance { get; private set; }
    public static FactionCentreTile EnemyInstance { get; private set; }

    [SerializeField] private bool IsEnemy;

    public Vector3Int gridPosition { get; private set; }
    public Vector3 worldPosition { get; private set; }

    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
        if (IsEnemy)
            EnemyInstance = this;
        else
            Instance = this;

        gridPosition = position;

        if (TileManager.HasInstance)
            RegisterWorldPosition();
        else
            TileManager.OnReady += RegisterWorldPosition;

        return base.StartUp(position, tilemap, go);
    }

    private void RegisterWorldPosition()
    {
        worldPosition = TileManager.Instance.CellToWorld(gridPosition);
    }

#if UNITY_EDITOR
    // The following is a helper that adds a menu item to create a TrainingCenterTile Asset
    [MenuItem("Assets/Create/FactionCentreTile")]
    public static void CreateTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Faction Centre Tile", "New Faction Centre Tile", "asset", "Save Faction Centre Tile", "Assets");
        if (path == "")
            return;
        AssetDatabase.CreateAsset(CreateInstance<FactionCentreTile>(), path);
    }
#endif
}
