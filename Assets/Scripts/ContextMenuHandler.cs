using System;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class ContextMenuHandler : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private RectTransform contextMenuTransform;
    [SerializeField] private Button contextActionPrefab;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TrainingCenterTile trainingCenterTile;
    [SerializeField] private WorkerHousingTile workerHousingTile;

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
        if (GameManager.Instance.unwalkableTilemap.HasTile(cellCoordinate))
        {
            // There is a unwalkable tile. Create a destruction context menu
            CreateDestructionContextMenu(cellCoordinate, GameManager.Instance.unwalkableTilemap);
            return;
        }

        // Check for enemy
        if (GameManager.Instance.enemyTilemap.HasTile(cellCoordinate))
        {
            // There is a enemy tile. Create a attack context menu
            CreateAttackContextMenu(cellCoordinate, GameManager.Instance.enemyTilemap);
            return;
        }

        // Check for building
        if (GameManager.Instance.buildingTilemap.HasTile(cellCoordinate))
            return;

        // There will always be ground. But a defensive check can't hurt
        if (!GameManager.Instance.groundTilemap.HasTile(cellCoordinate))
        {
            Debug.Log($"Ground tilemap doesn't have a tile at {cellCoordinate}");
            return;
        }

        CreateBuildContextMenu(cellCoordinate);
    }

    private void FinishContextMenu(Vector3Int cellCoordinate)
    {
        CreateCancelButton();

        Vector3 screenPosition = Input.mousePosition;
        contextMenuTransform.position = screenPosition;
        //contextMenuTransform.position += (Vector3)contextMenuTransform.sizeDelta / 1.5f;

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

    private void WorkerReleaseWrapper(Action action, Worker worker)
    {
        action?.Invoke();
        Workers.ReturnWorker(worker);
    }

    private void CreateCancelButton() => CreateButton(HideContextMenu, "Cancel", Color.red);

    private Button CreateButton(Action buttonAction, string buttonText, Color buttonColor, bool enabled = true)
    {
        Button createdButton = Instantiate(contextActionPrefab, contextMenuTransform);
        createdButton.image.color = buttonColor;
        createdButton.GetComponentInChildren<TMP_Text>().text = buttonText;
        createdButton.onClick.AddListener(buttonAction.Invoke);
        createdButton.interactable = enabled;
        return createdButton;
    }

    private void CreateBuildingButton(TileBase tile, Vector3Int position, Tilemap tilemap, string friendlyTileName) => CreateButton(() => BuildingButtonWrapper(tile, position, tilemap), friendlyTileName, Color.white);

    private void BuildingButtonWrapper(TileBase tile, Vector3Int position, Tilemap tilemap) => ButtonActionWrapper(() =>
    {
        bool success = Workers.TryRequestWorker(out Worker worker);
        if (!success)
        {
            Debug.LogError("Failed to request worker");
            return;
        }
        worker.Goto(grid.CellToWorld(position), () => WorkerReleaseWrapper(() => tilemap.SetTile(position, tile), worker));
    });

    private void CreateBuildContextMenu(Vector3Int cellCoordinate)
    {
        BeginContextMenu("Build");
        CreateBuildingButton(trainingCenterTile, cellCoordinate, GameManager.Instance.buildingTilemap, "Training Center");
        CreateBuildingButton(workerHousingTile, cellCoordinate, GameManager.Instance.buildingTilemap, "Worker Housing");
        FinishContextMenu(cellCoordinate);
    }

    private void CreateAttackButton(int workersToSend, Tilemap tilemap, Vector3Int cellCoordinate)
    {
        CreateButton(() => ButtonActionWrapper(() =>
        {
            TileBase tile = tilemap.GetTile(cellCoordinate);

            if (tile is not AttackableTile)
            {
                Debug.LogError("Tried to attack non-attackable tile!");
                return;
            }

            AttackableTile attackableTile = tile as AttackableTile;

            for (int i = 0; i < workersToSend; i++)
            {
                bool success = Workers.TryRequestWorker(out Worker worker);
                if (!success)
                {
                    Debug.LogError("Failed to request worker");
                    return;
                }

                worker.Goto(grid.CellToWorld(cellCoordinate), () =>
                {
                    worker.Attack(attackableTile, () => Workers.ReturnWorker(worker));
                });
            }
        }), $"Attack ({workersToSend} worker(s))", Color.yellow, Workers.HasAvailableWorkers(workersToSend));
    }

    private void CreateAttackContextMenu(Vector3Int cellCoordinate, Tilemap tilemap)
    {
        BeginContextMenu("Enemy");
        CreateAttackButton(1, tilemap, cellCoordinate);
        CreateAttackButton(2, tilemap, cellCoordinate);
        CreateAttackButton(5, tilemap, cellCoordinate);
        CreateAttackButton(10, tilemap, cellCoordinate);
        FinishContextMenu(cellCoordinate);
    }

    private void CreateDestructionContextMenu(Vector3Int cellCoordinate, Tilemap tilemap)
    {
        BeginContextMenu("Obstacle");
        CreateButton(() => ButtonActionWrapper(() =>
        {
            bool success = Workers.TryRequestWorker(out Worker worker);
            if (!success)
            {
                Debug.LogError("Failed to request worker");
                return;
            }
            worker.Goto(grid.CellToWorld(cellCoordinate), () => WorkerReleaseWrapper(() => tilemap.SetTile(cellCoordinate, null), worker));
        }), "Deconstruct", Color.green, Workers.HasAvailableWorkers());
        FinishContextMenu(cellCoordinate);
    }
}
