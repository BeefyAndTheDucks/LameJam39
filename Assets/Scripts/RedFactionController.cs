using UnityEngine;

public class RedFactionController : MonoBehaviour
{
    private bool createHousing = false;
    private float actionTimer;
    private float actionTimerMax = 2.0f;

    private int currentXOffset = 0;
    private int currentYOffset = 0;

    private int maxArea = 10;

    [SerializeField] private TrainingCenterTile enemyTrainingCenterTile;
    [SerializeField] private WorkerHousingTile enemyWorkerHousingTile;

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
        if (createHousing)
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

            return;
        }

        if (EnemyWorkers.GetWorkerCount() < EnemyWorkers.GetWorkerLimit())
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

        pos = FactionCentreTile.EnemyInstance.gridPosition + new Vector3Int(currentXOffset, currentYOffset, 0);

        return true;
    }
}
