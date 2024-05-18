using System;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class ContextMenuHandler : MonoBehaviour
{
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap unwalkableTilemap;
    [SerializeField] private Tilemap buildingTilemap;
    [SerializeField] private Tilemap enemyTilemap;
    [SerializeField] private Tilemap resourceTilemap;
    [SerializeField] private Grid grid;
    [SerializeField] private RectTransform contextMenuTransform;
    [SerializeField] private Button contextActionPrefab;
    [SerializeField] private TMP_Text titleText;

    public bool IsContextMenuEnabled => contextMenuTransform.gameObject.activeSelf;

    private void Start()
    {
        GameManager.Instance.OnShowContextMenu += OnShowContextMenu;
        GameManager.Instance.OnChooseMove += OnChooseMove;

        HideContextMenu();
    }

    private void OnChooseMove(Vector2 vector)
    {
        HideContextMenu();
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
            return;
        }

        // There will always be ground. But a defensive check can't hurt
        if (!groundTilemap.HasTile(cellCoordinate))
        {
            Debug.Log($"Ground tilemap doesn't have a tile at {cellCoordinate}");
            return;
        }

        CreateBuildContextMenu(cellCoordinate);
    }

    private void FinishContextMenu(Vector3Int cellCoordinate)
    {
        CreateCancelButton();

        //Vector3 position = grid.CellToWorld(cellCoordinate);
        //Vector3 screenPosition = RectTransformUtility.WorldToScreenPoint(GameManager.Instance.cachedCameraMain, position);
        Vector3 screenPosition = Input.mousePosition;
        contextMenuTransform.position = screenPosition;
        contextMenuTransform.position += (Vector3)contextMenuTransform.sizeDelta / 1.5f;

        contextMenuTransform.gameObject.SetActive(true);
    }

    private void BeginContextMenu(string title)
    {
        foreach (Transform child in contextMenuTransform)
        {
            if (child != titleText.transform)
                Destroy(child.gameObject);
        }

        titleText.text = title;
    }

    private void HideContextMenu()
    {
        contextMenuTransform.gameObject.SetActive(false);
    }

    private void ButtonActionWrapper(Action action)
    {
        action?.Invoke();
        HideContextMenu();
    }

    private void CreateCancelButton() => CreateButton(HideContextMenu, "Cancel", Color.red);

    private Button CreateButton(Action buttonAction, string buttonText, Color buttonColor)
    {
        Button createdButton = Instantiate(contextActionPrefab, contextMenuTransform);
        createdButton.image.color = buttonColor;
        createdButton.GetComponentInChildren<TMP_Text>().text = buttonText;
        createdButton.onClick.AddListener(buttonAction.Invoke);
        return createdButton;
    }

    private void CreateBuildContextMenu(Vector3Int cellCoordinate)
    {
        BeginContextMenu("Build");
        FinishContextMenu(cellCoordinate);
    }

    private void CreateMineContextMenu(Vector3Int cellCoordinate, Tilemap tilemap)
    {
        BeginContextMenu("Resource");
        FinishContextMenu(cellCoordinate);
    }

    private void CreateBuildingConfigurationContextMenu(Vector3Int cellCoordinate, Tilemap tilemap)
    {
        BeginContextMenu("Configure");
        FinishContextMenu(cellCoordinate);
    }

    private void CreateAttackContextMenu(Vector3Int cellCoordinate, Tilemap tilemap)
    {
        BeginContextMenu("Enemy");
        FinishContextMenu(cellCoordinate);
    }

    private void CreateDestructionContextMenu(Vector3Int cellCoordinate, Tilemap tilemap)
    {
        BeginContextMenu("Obstacle");
        CreateButton(() => ButtonActionWrapper(() => tilemap.SetTile(cellCoordinate, null)), "Deconstruct", Color.green);
        FinishContextMenu(cellCoordinate);
    }
}
