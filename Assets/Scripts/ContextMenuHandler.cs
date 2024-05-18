using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ContextMenuHandler : MonoBehaviour
{
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap unwalkableTilemap;
    [SerializeField] private Tilemap buildingTilemap;
    [SerializeField] private Tilemap enemyTilemap;
    [SerializeField] private Tilemap resourceTilemap;
    [SerializeField] private Grid grid;

    private void Start()
    {
        GameManager.Instance.OnShowContextMenu += OnShowContextMenu;
    }

    private void OnShowContextMenu(Vector2 clickedPosition)
    {
        // Get cell coordinates
        Vector3Int cellCoordinate = grid.WorldToCell(new Vector3(clickedPosition.x, clickedPosition.y, 0));

        // Check if there is a unwalkable tile there
        if (unwalkableTilemap.HasTile(cellCoordinate))
        {
            // There is a unwalkable tile. Create a destruction context menu
            CreateDestructionContextMenu(cellCoordinate, unwalkableTilemap);
            return;
        }

        // Check for enemy
        if (enemyTilemap.HasTile(cellCoordinate))
        {
            // There is a enemy tile. Create a attack context menu
            CreateAttackContextMenu(cellCoordinate, enemyTilemap);
            return;
        }

        // Check for building
        if (buildingTilemap.HasTile(cellCoordinate))
        {
            // There is a building tile. Create a configure building context menu
            CreateBuildingConfigurationContextMenu(cellCoordinate, buildingTilemap);
            return;
        }

        if (resourceTilemap.HasTile(cellCoordinate))
        {
            // There is a resource tile. Create a mine context menu.
            CreateMineContextMenu(cellCoordinate, resourceTilemap);
        }

        // There will always be ground. But a defensive check can't hurt
        if (!groundTilemap.HasTile(cellCoordinate))
        {
            Debug.Log($"Ground tilemap doesn't have a tile at {cellCoordinate}");
            return;
        }

        CreateBuildContextMenu(cellCoordinate);
    }

    private void CreateBuildContextMenu(Vector3Int cellCoordinate)
    {
        throw new NotImplementedException();
    }

    private void CreateMineContextMenu(Vector3Int cellCoordinate, Tilemap resourceTilemap)
    {
        throw new NotImplementedException();
    }

    private void CreateBuildingConfigurationContextMenu(Vector3Int cellCoordinate, Tilemap buildingTilemap)
    {
        throw new NotImplementedException();
    }

    private void CreateAttackContextMenu(Vector3Int cellCoordinate, Tilemap enemyTilemap)
    {
        throw new NotImplementedException();
    }

    private void CreateDestructionContextMenu(Vector3Int cellCoordinate, Tilemap unwalkableTilemap)
    {
        throw new NotImplementedException();
    }
}
