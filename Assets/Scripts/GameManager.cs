using System;
using System.Threading.Tasks;
using NavMeshPlus.Components;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Transform player;

    public event Action<Vector2> OnChooseMove;
    public event Action<Vector2> OnShowContextMenu;

    public Tilemap groundTilemap;
    public Tilemap unwalkableTilemap;
    public Tilemap buildingTilemap;
    public Tilemap enemyTilemap;
    public Grid grid;

    [HideInInspector] public Camera cachedCameraMain;

    [SerializeField] private ContextMenuHandler contextMenuHandler;
    [SerializeField] private NavMeshSurface navMesh;
    [SerializeField] private float maximumContextMenuDistance = 2.0f;
    [SerializeField] private WinLooseScreen winLooseScreen;

    [field: SerializeField] public Worker workerPrefab { get; private set; }
    [field: SerializeField] public Worker enemyWorkerPrefab { get; private set; }
    [field: SerializeField] public Transform workerParent { get; private set; }

    [field: SerializeField] public TrainingCenterTile enemyTrainingCenterTile { get; private set; }
    [field: SerializeField] public WorkerHousingTile enemyWorkerHousingTile { get; private set; }
    [field: SerializeField] public TurretTile enemyTurretTile { get; private set; }

    private bool recalcNavMesh = true;

    private void Awake()
    {
        Instance = this;
        cachedCameraMain = Camera.main;

        Tilemap.tilemapTileChanged += OnTilemapTileChanged;
        
        Time.timeScale = 0.0f;
    }

    public void Lose()
    {
        Debug.Log("Lost :(");
        winLooseScreen.Lose();
        Time.timeScale = 0.0f;
    }

    public void Win()
    {
        Debug.Log("WIN!!");
        winLooseScreen.Win();
        Time.timeScale = 0.0f;
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

    private async Task UpdateNavMesh()
    {
        Debug.Log("Updating NavMesh");
        await navMesh.UpdateNavMesh(navMesh.navMeshData);
    }

    public void StartGame()
    {
        Time.timeScale = 1.0f;
    }
}
