using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TurretTile : AttackableTile
{
    [SerializeField] private float timeToAttack = 5;
    [SerializeField] private bool IsEnemy;

    private float timer;

    private Vector3Int gridPosition;
    public int Health { get; private set; } = 120;

    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
        if (TileManager.HasInstance)
            RegisterOnUpdate();
        else
            TileManager.OnReady += RegisterOnUpdate;

        gridPosition = position;

        Health = 120;

        return base.StartUp(position, tilemap, go);
    }

    private void RegisterOnUpdate()
    {
        TileManager.Instance.OnUpdate += Update;
    }

    public override void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            TileManager.Instance.OnUpdate -= Update;

            if (IsEnemy)
                GameManager.Instance.enemyTilemap.SetTile(gridPosition, null);
            else
                GameManager.Instance.buildingTilemap.SetTile(gridPosition, null);
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= timeToAttack)
        {
            Vector3 position = TileManager.Instance.CellToWorld(gridPosition);
            Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 5);

            foreach (Collider2D collider in colliders)
            {
                if (collider.TryGetComponent(out Worker worker))
                {
                    if (worker.IsEnemy != IsEnemy)
                        worker.TakeDamage(5);
                }
            }

            timer = 0;
        }
    }

#if UNITY_EDITOR
    // The following is a helper that adds a menu item to create a TrainingCenterTile Asset
    [MenuItem("Assets/Create/TurretTile")]
    public static void CreateTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Turret Tile", "New Turret Tile", "asset", "Save Turret Tile", "Assets");
        if (path == "")
            return;
        AssetDatabase.CreateAsset(CreateInstance<TurretTile>(), path);
    }
#endif
}
