using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorkerHousingTile : AttackableTile
{
    [SerializeField] private bool IsEnemy;
    public int Health { get; private set; } = 50;

    private Vector3Int gridPosition;

    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {

        if (!Application.isPlaying)
            return false;

        gridPosition = position;

        if (IsEnemy)
            EnemyWorkers.IncreaseWorkerLimit(2);
        else
            Workers.IncreaseWorkerLimit(2);

        Health = 50;

        return base.StartUp(position, tilemap, go);
    }

    public override void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            if (IsEnemy)
            {
                EnemyWorkers.DecreaseWorkerLimit(2);
                GameManager.Instance.enemyTilemap.SetTile(gridPosition, null);
            }
            else
            {
                Workers.DecreaseWorkerLimit(2);
                GameManager.Instance.buildingTilemap.SetTile(gridPosition, null);
            }
        }
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
