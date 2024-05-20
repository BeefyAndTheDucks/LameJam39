using UnityEngine;

public class RedFactionController : MonoBehaviour
{
    private bool createHousing = false;
    private float actionTimer;
    private float actionTimerMax = 5.0f;

    private int currentXOffset = 0;
    private int currentYOffset = 0;

    private int maxArea = 10;

    [SerializeField] private TrainingCenterTile enemyTrainingCenterTile;
    [SerializeField] private WorkerHousingTile enemyWorkerHousingTile;
    [SerializeField] private TurretTile enemyTurretTile;

    private void Awake()
    {
        currentXOffset = -maxArea;
        currentYOffset = -maxArea;
    }

    private void Update()
    {
        actionTimer += Time.deltaTime;

        if (actionTimer >= actionTimerMax)
        {
            actionTimer = 0;
            if (EnemyWorkers.HasAvailableWorkers())
                PickAction();
            else
                createHousing = true;
        }
    }

    private void PickAction()
    {
        if (EnemyWorkers.HasAvailableWorkers(5))
        {
            for (int i = 0; i < 5; i++)
            {
                bool success = EnemyWorkers.TryRequestWorker(out Worker worker);
                if (!success)
                {
                    Debug.LogError("Failed to request worker");
                    return;
                }

                worker.Goto(FactionCentreTile.Instance.worldPosition, () => worker.Attack(FactionCentreTile.Instance, () => EnemyWorkers.ReturnWorker(worker)));
            }
        }

        if (!EnemyWorkers.HasAvailableWorkers())
        {
            createHousing = true;
            return; // No more workers available...........
        }

        if (createHousing || Random.Range(0, 10) <= 3)
        {
            createHousing = false;

            bool success = EnemyWorkers.TryRequestWorker(out Worker worker);
            if (!success)
            {
                Debug.LogError("Failed to request worker");
                return;
            }

            success = PickPosition(out Vector3Int cellPos);
            if (!success)
            {
                Debug.LogError("Failed to find position");
                return;
            }

            worker.Goto(GameManager.Instance.grid.CellToWorld(cellPos), () => WorkerReleaseWrapper(() => GameManager.Instance.enemyTilemap.SetTile(cellPos, enemyWorkerHousingTile), worker));

            return;
        }

        if (EnemyWorkers.GetWorkerCount() < EnemyWorkers.GetWorkerLimit() || Random.Range(0, 10) <= 1)
        {
            bool success = EnemyWorkers.TryRequestWorker(out Worker worker);
            if (!success)
            {
                Debug.LogError("Failed to request worker");
                return;
            }

            success = PickPosition(out Vector3Int cellPos);
            if (!success)
            {
                Debug.LogError("Failed to find position");
                return;
            }

            worker.Goto(GameManager.Instance.grid.CellToWorld(cellPos), () => WorkerReleaseWrapper(() => GameManager.Instance.enemyTilemap.SetTile(cellPos, enemyTrainingCenterTile), worker));

            return;
        }

        // Create Turret

        bool _success = EnemyWorkers.TryRequestWorker(out Worker _worker);
        if (!_success)
        {
            Debug.LogError("Failed to request worker");
            return;
        }

        _success = PickPosition(out Vector3Int _cellPos);
        if (!_success)
        {
            Debug.LogError("Failed to find position");
            return;
        }

        _worker.Goto(GameManager.Instance.grid.CellToWorld(_cellPos), () => WorkerReleaseWrapper(() => GameManager.Instance.enemyTilemap.SetTile(_cellPos, enemyTurretTile), _worker));

        return;
    }

    private void WorkerReleaseWrapper(System.Action action, Worker worker)
    {
        action?.Invoke();
        EnemyWorkers.ReturnWorker(worker);
    }

    private bool PickPosition(out Vector3Int pos)
    {
        currentXOffset += 2;
        if (currentXOffset >= maxArea)
        {
            currentYOffset += 2;
            currentXOffset = -maxArea;
        }

        if (currentYOffset >= maxArea)
        {
            Debug.LogError("No more area.");

            pos = FactionCentreTile.EnemyInstance.gridPosition;

            return false;
        }

        if (currentXOffset == 0 && currentYOffset == 0)
        {
            currentXOffset = 2;
        }

        pos = FactionCentreTile.EnemyInstance.gridPosition + new Vector3Int(currentXOffset, currentYOffset, 0);

        return true;
    }
}
