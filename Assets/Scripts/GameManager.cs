using System;
using NavMeshPlus.Components;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;

[DefaultExecutionOrder(-102)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Transform player;

    public event Action<Vector2> OnChooseMove;
    public event Action<Vector2> OnShowContextMenu;

    [HideInInspector] public Camera cachedCameraMain;

    [SerializeField] private ContextMenuHandler contextMenuHandler;
    [SerializeField] private NavMeshSurface navMesh;
    [SerializeField] private float maximumContextMenuDistance = 2.0f;

    [field: SerializeField] public Worker workerPrefab { get; private set; }
    [field: SerializeField] public Worker enemyWorkerPrefab { get; private set; }
    [field: SerializeField] public Transform workerParent { get; private set; }

    private bool recalcNavMesh = true;

    private void Awake()
    {
        Instance = this;
        cachedCameraMain = Camera.main;

        Tilemap.tilemapTileChanged += OnTilemapTileChanged;
    }

    private void OnTilemapTileChanged(Tilemap tilemap, Tilemap.SyncTile[] arg2) => recalcNavMesh = true;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !IsMouseOverUI)
        {
            Vector2 clickedPosition = cachedCameraMain.ScreenToWorldPoint(Input.mousePosition);
            float distance = Vector2.Distance(player.position, clickedPosition);
            if (distance > maximumContextMenuDistance)
                OnChooseMove?.Invoke(clickedPosition);
            else if (!contextMenuHandler.IsContextMenuEnabled)
                OnShowContextMenu?.Invoke(clickedPosition);
        }

        if (recalcNavMesh)
        {
            UpdateNavMesh();
            //navMesh.BuildNavMesh();
            recalcNavMesh = false;
        }
    }

    public static bool IsMouseOverUI
    {
        get
        {
            // [Only works well while there is not PhysicsRaycaster on the Camera)
            EventSystem eventSystem = EventSystem.current;
            return eventSystem != null && eventSystem.IsPointerOverGameObject();

            // [Works with PhysicsRaycaster on the Camera. Requires New Input System. Assumes mouse.)
            // if (EventSystem.current == null)
            // {
            //     return false;
            // }
            // RaycastResult lastRaycastResult = ((InputSystemUIInputModule)EventSystem.current.currentInputModule).GetLastRaycastResult(Mouse.current.deviceId);
            // const int uiLayer = 5;
            // return lastRaycastResult.gameObject != null && lastRaycastResult.gameObject.layer == uiLayer;
        }
    }

    private async void UpdateNavMesh()
    {
        Debug.Log("Updating NavMesh");
        await navMesh.UpdateNavMesh(navMesh.navMeshData);
    }
}
